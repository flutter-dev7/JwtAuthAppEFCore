using System;
using System.Net.Mail;
using JwtAuthApp.Application.Common;
using JwtAuthApp.Application.DTOs.Auth.Request;
using JwtAuthApp.Application.DTOs.Auth.Response;
using JwtAuthApp.Application.Interfaces.Repositories;
using JwtAuthApp.Application.Interfaces.Services;
using JwtAuthApp.Domain.Entities;
using JwtAuthApp.Domain.Enum;

namespace JwtAuthApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto login)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetByUserEmailAsync(login.Email);

            if (user == null)
                return Result<LoginResponseDto>.Fail($"Invalid email or password");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                return Result<LoginResponseDto>.Fail("Invalid email or password");

            var token = _unitOfWork.Jwt.GenerateToken(user);

            var result = new LoginResponseDto()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token
            };

            return Result<LoginResponseDto>.Ok(result);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"LOGIN ERROR: {ex.Message}");
            Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
            return Result<LoginResponseDto>.Fail($"Error: {ex.Message}", Domain.Enum.ErrorType.Unknown);
        }
    }

    public async Task<Result<RegisterResponseDto>> RegisterAsync(RegisterDto register)
    {
        if (string.IsNullOrWhiteSpace(register.Username))
            return Result<RegisterResponseDto>.Fail("Username cannot be empty", Domain.Enum.ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(register.Email))
            return Result<RegisterResponseDto>.Fail("Email cannot be empty", Domain.Enum.ErrorType.Validation);

        if (await _unitOfWork.UserRepository.GetByUserEmailAsync(register.Email) != null)
            return Result<RegisterResponseDto>.Fail("Email already exists", Domain.Enum.ErrorType.Validation);

        if (register.Password.Length < 6)
            return Result<RegisterResponseDto>.Fail("Password cannot be less than 6", Domain.Enum.ErrorType.Validation);

        if (register.Password != register.ConfirmPassword)
            return Result<RegisterResponseDto>.Fail("Passwords are not matched", Domain.Enum.ErrorType.Validation);
        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(register.Password);

            var user = new User
            {
                Username = register.Username,
                Email = register.Email,
                Role = UserRole.User,
                PasswordHash = passwordHash
            };

            await _unitOfWork.UserRepository.CreateUserAsync(user);

            return Result<RegisterResponseDto>.Ok(new RegisterResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            });
        }
        catch (System.Exception ex)
        {
            return Result<RegisterResponseDto>.Fail($"Error: {ex.Message}", Domain.Enum.ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> ChangePasswordAsync(string email, ChangePasswordDto request)
    {
        if (request.NewPassword.Length < 6)
            return Result<bool>.Fail("Password must be at least 6 characters", ErrorType.Validation);

        if (request.NewPassword != request.ConfirmPassword)
            return Result<bool>.Fail("Passwords do not match", ErrorType.Validation);
        try
        {
            var user = await _unitOfWork.UserRepository.GetByUserEmailAsync(email);

            if (user == null)
                return Result<bool>.Fail($"User with Id = {user!.Id} not found", ErrorType.NotFound);

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return Result<bool>.Fail("Current password is incorrect", ErrorType.Validation);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _unitOfWork.UserRepository.UpdateUserAsync(user);

            return Result<bool>.Ok(true);
        }
        catch (Exception e)
        {
            return Result<bool>.Fail($"Error: {e.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> SendEmailAsync(SendEmailDto request)
    {
        var user = await _unitOfWork.UserRepository.GetByUserEmailAsync(request.Email);

        if (user == null)
            return Result<bool>.Fail($"User with Email = {request.Email} not found", ErrorType.NotFound);

        var code = new Random().Next(100_000, 999_999).ToString();
        var htmlBody = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='UTF-8'>
        <style>
            body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
            .container {{ max-width: 500px; margin: 40px auto; background: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
            .header {{ background: linear-gradient(135deg, #667eea, #764ba2); padding: 30px; text-align: center; }}
            .header h1 {{ color: white; margin: 0; font-size: 24px; }}
            .body {{ padding: 30px; text-align: center; }}
            .body p {{ color: #555; font-size: 15px; line-height: 1.6; }}
            .code-box {{ display: inline-block; background: #f0f0f0; border: 2px dashed #667eea; border-radius: 8px; padding: 15px 40px; margin: 20px 0; }}
            .code {{ font-size: 36px; font-weight: bold; color: #667eea; letter-spacing: 8px; }}
            .warning {{ color: #999; font-size: 13px; margin-top: 10px; }}
            .footer {{ background: #f9f9f9; padding: 15px; text-align: center; color: #aaa; font-size: 12px; border-top: 1px solid #eee; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h1>🔐 Password Reset</h1>
            </div>
            <div class='body'>
                <p>Hello, <strong>{user.Username}</strong>!</p>
                <p>We received a request to reset your password.<br>Use the code below:</p>
                <div class='code-box'>
                    <div class='code'>{code}</div>
                </div>
                <p class='warning'>⏱ This code expires in <strong>3 minutes</strong>.</p>
                <p class='warning'>If you did not request this, ignore this email.</p>
            </div>
            <div class='footer'>
                © 2026 JwtAuthApp — Do not reply to this email
            </div>
        </div>
    </body>
    </html>";
        try
        {
            var result = await _unitOfWork.Email.SendEmailAsync(request.Email, "Password reset", htmlBody);
            if (!result)
            {
                return Result<bool>.Fail("Failed to send email");
            }
        }
        catch (SmtpException e)
        {
            return Result<bool>.Fail("Failed to send email: " + e.Message);
        }

        var verificationCode = new VerificationCode
        {
            UserId = user.Id,
            Code = code,
            Expiration = DateTime.UtcNow.AddMinutes(3)
        };

        await _unitOfWork.VerificationCodeRepository.AddAsync(verificationCode);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> VerifyCodeAsync(VerifyCodeDto request)
    {
        var user = await _unitOfWork.UserRepository.GetByUserEmailAsync(request.Email);
        if (user == null)
        {
            return Result<bool>.Fail("Email not found", ErrorType.NotFound);
        }

        var lastCode = await _unitOfWork.VerificationCodeRepository.GetLatestVerificationCodeAsync(user.Id);
        if (lastCode == null || lastCode.Code != request.Code || lastCode.Expiration < DateTime.UtcNow)
            return Result<bool>.Fail("Invalid or expired verification code", ErrorType.Validation);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto request)
    {
        var user = await _unitOfWork.UserRepository.GetByUserEmailAsync(request.Email);
        if (user == null)
        {
            return Result<bool>.Fail("Email not found", ErrorType.NotFound);
        }

        if (request.NewPassword != request.ConfirmPassword)
            return Result<bool>.Fail("Password do not match", ErrorType.Validation);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordHash = passwordHash;
        await _unitOfWork.UserRepository.UpdateUserAsync(user);

        return Result<bool>.Ok(true);
    }
}
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
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, IEmailService emailService, IVerificationCodeRepository verificationCodeRepository)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _emailService = emailService;
        _verificationCodeRepository = verificationCodeRepository;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto login)
    {
        try
        {
            var user = await _userRepository.GetByUserEmailAsync(login.Email);

            if (user == null)
                return Result<LoginResponseDto>.Fail($"Invalid email or password");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                return Result<LoginResponseDto>.Fail("Invalid email or password");

            var token = _jwtService.GenerateToken(user);

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

        if (await _userRepository.GetByUserEmailAsync(register.Email) != null)
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

            await _userRepository.CreateUserAsync(user);

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
            var user = await _userRepository.GetByUserEmailAsync(email);

            if (user == null)
                return Result<bool>.Fail($"User with Id = {user!.Id} not found", ErrorType.NotFound);

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return Result<bool>.Fail("Current password is incorrect", ErrorType.Validation);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _userRepository.UpdateUserAsync(user);

            return Result<bool>.Ok(true);
        }
        catch (Exception e)
        {
            return Result<bool>.Fail($"Error: {e.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> SendEmailAsync(SendEmailDto request)
    {
        var user = await _userRepository.GetByUserEmailAsync(request.Email);

        if (user == null)
            return Result<bool>.Fail($"User with Email = {request.Email} not found", ErrorType.NotFound);

        var code = new Random().Next(100_000, 999_999).ToString();
        try
        {
            var result = await _emailService.SendEmailAsync(request.Email, "Password reset", $"<h3>{code} is your password reset code.</h3>");
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

        await _verificationCodeRepository.AddAsync(verificationCode);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> VerifyCodeAsync(VerifyCodeDto request)
    {
        var user = await _userRepository.GetByUserEmailAsync(request.Email);
        if (user == null)
        {
            return Result<bool>.Fail("Email not found", ErrorType.NotFound);
        }

        var lastCode = await _verificationCodeRepository.GetLatestVerificationCodeAsync(user.Id);
        if (lastCode == null || lastCode.Code != request.Code || lastCode.Expiration < DateTime.UtcNow)
            return Result<bool>.Fail("Invalid or expired verification code", ErrorType.Validation);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto request)
    {
        var user = await _userRepository.GetByUserEmailAsync(request.Email);
        if (user == null)
        {
            return Result<bool>.Fail("Email not found", ErrorType.NotFound);
        }

        if (request.NewPassword != request.ConfirmPassword)
            return Result<bool>.Fail("Password do not match", ErrorType.Validation);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordHash = passwordHash;
        await _userRepository.UpdateUserAsync(user);

        return Result<bool>.Ok(true);
    }
}

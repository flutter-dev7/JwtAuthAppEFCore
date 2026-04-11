using System;
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

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
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
}

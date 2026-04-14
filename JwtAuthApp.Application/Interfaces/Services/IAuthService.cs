using System;
using JwtAuthApp.Application.Common;
using JwtAuthApp.Application.DTOs.Auth.Request;
using JwtAuthApp.Application.DTOs.Auth.Response;

namespace JwtAuthApp.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto login);
    Task<Result<RegisterResponseDto>> RegisterAsync(RegisterDto register);
    Task<Result<bool>> ChangePasswordAsync(string email, ChangePasswordDto request);
    Task<Result<bool>> SendEmailAsync(SendEmailDto request);
    Task<Result<bool>> VerifyCodeAsync(VerifyCodeDto request);
    Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto request);
}

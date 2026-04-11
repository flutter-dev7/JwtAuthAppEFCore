using System;
using JwtAuthApp.Application.Common;
using JwtAuthApp.Application.DTOs.Auth.Request;
using JwtAuthApp.Application.DTOs.Auth.Response;

namespace JwtAuthApp.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto login);
    Task<Result<RegisterResponseDto>> RegisterAsync(RegisterDto register);
}

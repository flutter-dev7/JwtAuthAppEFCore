using System;
using JwtAuthApp.Application.DTOs.Users.Response;

namespace JwtAuthApp.Application.DTOs.Auth.Response;

public class LoginResponseDto : GetUserDto
{
    public string Token { get; set; } = string.Empty;
}

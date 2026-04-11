using System;
using JwtAuthApp.Domain.Enum;

namespace JwtAuthApp.Application.DTOs.Users.Request;

public class UpdateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
}

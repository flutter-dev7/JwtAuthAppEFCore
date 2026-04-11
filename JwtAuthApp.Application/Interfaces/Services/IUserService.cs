using System;
using JwtAuthApp.Application.Common;
using JwtAuthApp.Application.DTOs.Auth.Request;
using JwtAuthApp.Application.DTOs.Auth.Response;
using JwtAuthApp.Application.DTOs.Users.Request;
using JwtAuthApp.Application.DTOs.Users.Response;
using JwtAuthApp.Domain.Enum;

namespace JwtAuthApp.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<List<GetUserDto>>> GetAllUsersAsync();
    Task<Result<GetUserDto?>> GetUserByIdAsync(Guid id);
    Task<Result<GetUserDto?>> GetUserByEmailAsync(string email);
    Task<Result<string>> ChangeRoleAsync(Guid id, UserRole role);
    Task<Result<UpdateUserResponseDto>> UpdateUserAsync(Guid id, UpdateUserDto request);
    Task<Result<DeleteUserResponseDto>> DeleteUserAsync(Guid id);
}

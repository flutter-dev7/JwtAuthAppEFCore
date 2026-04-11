using System;
using JwtAuthApp.Application.Common;
using JwtAuthApp.Application.DTOs.Users.Request;
using JwtAuthApp.Application.DTOs.Users.Response;
using JwtAuthApp.Application.Interfaces.Repositories;
using JwtAuthApp.Application.Interfaces.Services;
using JwtAuthApp.Domain.Enum;

namespace JwtAuthApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<GetUserDto>>> GetAllUsersAsync()
    {
        var users = await _repository.GetAllUsersAsync();

        var dto = users.Select(u => new GetUserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Role = u.Role.ToString(),
            CreatedAt = u.CreatedAt
        }).ToList();

        return Result<List<GetUserDto>>.Ok(dto);
    }

    public async Task<Result<GetUserDto?>> GetUserByIdAsync(Guid id)
    {
        var user = await _repository.GetUserByIdAsync(id);

        if (user == null)
            return Result<GetUserDto?>.Fail($"User with Id = {id} not found", Domain.Enum.ErrorType.NotFound);

        return Result<GetUserDto?>.Ok(new GetUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        });
    }

    public async Task<Result<GetUserDto?>> GetUserByEmailAsync(string email)
    {
        var user = await _repository.GetByUserEmailAsync(email);

        if (user == null)
            return Result<GetUserDto?>.Fail($"User with Email = {email} not found", Domain.Enum.ErrorType.NotFound);

        return Result<GetUserDto?>.Ok(new GetUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        });
    }

    public async Task<Result<string>> ChangeRoleAsync(Guid id, UserRole role)
    {
        var user = await _repository.GetUserByIdAsync(id);

        if (user == null)
            return Result<string>.Fail($"User not found", ErrorType.NotFound);

        user.Role = role;

        await _repository.UpdateUserAsync(user);
        
        return Result<string>.Ok($"Role changed to {role}");
    }

    public async Task<Result<UpdateUserResponseDto>> UpdateUserAsync(Guid id, UpdateUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return Result<UpdateUserResponseDto>.Fail("Username cannot be empty", Domain.Enum.ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<UpdateUserResponseDto>.Fail("Email cannot be empty", Domain.Enum.ErrorType.Validation);
        try
        {
            var user = await _repository.GetUserByIdAsync(id);

            if (user == null)
                return Result<UpdateUserResponseDto>.Fail($"User with Id = {id} not found for update", Domain.Enum.ErrorType.NotFound);

            user.Username = request.Username;
            user.Email = request.Email;
            user.Role = request.Role;

            await _repository.UpdateUserAsync(user);

            return Result<UpdateUserResponseDto>.Ok(new UpdateUserResponseDto
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
            return Result<UpdateUserResponseDto>.Fail($"Error: {ex.Message}", Domain.Enum.ErrorType.Unknown);
        }
    }

    public async Task<Result<DeleteUserResponseDto>> DeleteUserAsync(Guid id)
    {
        try
        {
            var user = await _repository.GetUserByIdAsync(id);

            if (user == null)
                return Result<DeleteUserResponseDto>.Fail($"User with Id = {id} not found for delete", Domain.Enum.ErrorType.NotFound);

            await _repository.DeleteUserAsync(user);

            return Result<DeleteUserResponseDto>.Ok(new DeleteUserResponseDto
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
            return Result<DeleteUserResponseDto>.Fail($"Error: {ex.Message}", Domain.Enum.ErrorType.Unknown);
        }
    }
}

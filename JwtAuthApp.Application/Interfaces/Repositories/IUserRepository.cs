using System;
using JwtAuthApp.Domain.Entities;

namespace JwtAuthApp.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetByUserEmailAsync(string email);
    Task CreateUserAsync(User request);
    Task UpdateUserAsync(User request);
    Task DeleteUserAsync(User request);
}

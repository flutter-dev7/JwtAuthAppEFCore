using System;
using JwtAuthApp.Application.Interfaces.Repositories;
using JwtAuthApp.Domain.Entities;
using JwtAuthApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.AsNoTracking().ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

     public async Task<User?> GetByUserEmailAsync(string email)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task CreateUserAsync(User request)
    {
        await _context.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User request)
    {
        _context.Update(request);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(User request)
    {
        _context.Remove(request);
        await _context.SaveChangesAsync();
    }
}

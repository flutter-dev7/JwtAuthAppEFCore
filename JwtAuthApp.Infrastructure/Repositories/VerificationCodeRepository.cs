using System;
using JwtAuthApp.Application.Interfaces.Repositories;
using JwtAuthApp.Domain.Entities;
using JwtAuthApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthApp.Infrastructure.Repositories;

public class VerificationCodeRepository : IVerificationCodeRepository
{
    private readonly AppDbContext _context;

    public VerificationCodeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(VerificationCode code)
    {
        await _context.VerificationCodes.AddAsync(code);
        await _context.SaveChangesAsync();
    }

    public async Task<VerificationCode?> GetLatestVerificationCodeAsync(Guid userId)
    {
        return await _context.VerificationCodes
        .AsNoTracking()
        .Where(v => v.UserId == userId)
        .OrderByDescending(v => v.Expiration)
        .FirstOrDefaultAsync();
    }

    public async Task<VerificationCode?> GetVerificationCodeAsync(int id)
    {
        return await _context.VerificationCodes
        .AsNoTracking()
        .FirstOrDefaultAsync(v => v.Id == id);
    }
}

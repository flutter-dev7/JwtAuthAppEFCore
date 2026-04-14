using System;
using JwtAuthApp.Domain.Entities;

namespace JwtAuthApp.Application.Interfaces.Repositories;

public interface IVerificationCodeRepository
{
    Task AddAsync(VerificationCode code);
    Task<VerificationCode?> GetLatestVerificationCodeAsync(Guid userId);
    Task<VerificationCode?> GetVerificationCodeAsync(int id);
}

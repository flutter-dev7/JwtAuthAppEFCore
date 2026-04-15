using System;
using JwtAuthApp.Application.Interfaces.Services;

namespace JwtAuthApp.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IEmailService Email { get; }
    IJwtService Jwt { get; }
    IVerificationCodeRepository VerificationCodeRepository { get; }

    Task<int> SaveChangeAsync();
}
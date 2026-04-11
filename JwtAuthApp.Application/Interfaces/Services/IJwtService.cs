using System;
using JwtAuthApp.Domain.Entities;

namespace JwtAuthApp.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}

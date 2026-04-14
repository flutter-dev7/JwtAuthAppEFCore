using System;

namespace JwtAuthApp.Application.Interfaces.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
}

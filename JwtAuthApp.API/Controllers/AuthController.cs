using System;
using System.Security.Claims;
using JwtAuthApp.Application.DTOs.Auth.Request;
using JwtAuthApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginDto loginDto)
    {
        var login = await _service.LoginAsync(loginDto);

        return !login.IsSuccess ? HandleError(login) : Ok(login);
    } 

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
    {
        var register = await _service.RegisterAsync(registerDto);

        return !register.IsSuccess ? HandleError(register) : Created("", register);
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var result = await _service.ChangePasswordAsync(email!, request);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpPost("send-password-reset-email")]
    public async Task<IActionResult> SendPasswordResetEmailAsync(SendEmailDto dto)
    {
        var result = await _service.SendEmailAsync(dto);
        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }
    
    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCodeAsync(VerifyCodeDto dto)
    {
        var result = await _service.VerifyCodeAsync(dto);
        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var result = await _service.ResetPasswordAsync(dto);
        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }
}

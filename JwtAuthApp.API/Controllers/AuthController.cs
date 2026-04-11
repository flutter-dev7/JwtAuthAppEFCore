using System;
using JwtAuthApp.Application.DTOs.Auth.Request;
using JwtAuthApp.Application.Interfaces.Services;
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
}

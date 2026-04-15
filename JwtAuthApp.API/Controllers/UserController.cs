using System;
using JwtAuthApp.Application.Common;
using JwtAuthApp.Application.DTOs.Users.Request;
using JwtAuthApp.Application.Interfaces.Services;
using JwtAuthApp.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthApp.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        if (!User.HasPermission(Permission.View))
            return Forbid();
        var users = await _service.GetAllUsersAsync();

        return !users.IsSuccess ? HandleError(users) : Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        if (!User.HasPermission(Permission.View))
            return Forbid();
        var user = await _service.GetUserByIdAsync(id);

        return !user.IsSuccess ? HandleError(user) : Ok(user);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmailAsync(string email)
    {
        if (!User.HasPermission(Permission.View))
            return Forbid();
        var user = await _service.GetUserByEmailAsync(email);

        return !user.IsSuccess ? HandleError(user) : Ok(user);
    }

    [HttpPut("{id:guid}/role")]
    public async Task<IActionResult> ChangeRoleAsync(Guid id, UserRole role)
    {
        if (!User.HasPermission(Permission.Update))
            return Forbid();
        var res = await _service.ChangeRoleAsync(id, role);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateUserDto request)
    {
        if (!User.HasPermission(Permission.Update))
            return Forbid();
        var res = await _service.UpdateUserAsync(id, request);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        if (!User.HasPermission(Permission.Delete))
            return Forbid();
        
        var res = await _service.DeleteUserAsync(id);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }
}

using System;
using System.Security.Claims;
using JwtAuthApp.Domain.Enum;

namespace JwtAuthApp.Application.Common;

public static class PermissionExtensions
{
    public static bool HasPermission(this ClaimsPrincipal user, Permission permission)
    {
        return user.Claims.Any(c => c.Type == "permission" && c.Value == permission.ToString());
    }
}
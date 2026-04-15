using System;
using JwtAuthApp.Domain.Enum;

namespace JwtAuthApp.Application.Common;

public static class RolePermissions
{
    public static List<Permission> GetPermissions(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => Enum.GetValues<Permission>().ToList(),
            UserRole.User => new List<Permission>
            {
                Permission.View,
            },

            _ => new List<Permission>()
        };
    }
}

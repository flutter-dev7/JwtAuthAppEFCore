using JwtAuthApp.Domain.Entities;
using JwtAuthApp.Domain.Enum;
using JwtAuthApp.Infrastructure.Data;

namespace JwtAuthApp.Infrastructure.Seeds;

public class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var adminExists = context.Users.Any(u => u.Role == UserRole.Admin);

        if (adminExists) return;

        var admin = new User
        {
            Username = "Admin",
            Email = "admin@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        await context.AddAsync(admin);
        await context.SaveChangesAsync();

        Console.WriteLine("Admin created: admin@gmail.com / Admin123!");
    }
}

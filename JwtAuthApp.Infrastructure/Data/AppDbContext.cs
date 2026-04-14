using System;
using JwtAuthApp.Domain.Entities;
using JwtAuthApp.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public AppDbContext(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new VerificationCodeConfiguration());
    }
}

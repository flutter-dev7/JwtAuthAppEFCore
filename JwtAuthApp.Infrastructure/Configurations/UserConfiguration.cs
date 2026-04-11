using System;
using JwtAuthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JwtAuthApp.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
        .IsRequired()
        .HasMaxLength(150);

        builder.Property(u => u.Email)
        .IsRequired()
        .HasMaxLength(150);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
        .IsRequired();

        builder.Property(u => u.CreatedAt)
        .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}

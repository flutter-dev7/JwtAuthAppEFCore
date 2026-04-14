using System;
using System.ComponentModel.DataAnnotations;

namespace JwtAuthApp.Application.DTOs.Auth.Request;

public class SendEmailDto
{
    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

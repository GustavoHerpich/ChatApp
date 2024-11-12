using System.ComponentModel.DataAnnotations;

namespace Chat.Models.DTOs;

public class LoginRequest
{
    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }
}

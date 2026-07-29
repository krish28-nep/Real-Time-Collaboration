using System.ComponentModel.DataAnnotations;

namespace RealTimeCollaboration.Modules.User.DTOs;

public class CreateUserDTO
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public IFormFile? AvatarUrl { get; set; }
}

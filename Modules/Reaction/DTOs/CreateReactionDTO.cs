using System.ComponentModel.DataAnnotations;

namespace RealTimeCollaboration.Modules.Reaction.DTOs;

public class CreateReactionDTO
{
    [Required]
    [MaxLength(32)]
    public string Emoji { get; set; } = string.Empty;
}

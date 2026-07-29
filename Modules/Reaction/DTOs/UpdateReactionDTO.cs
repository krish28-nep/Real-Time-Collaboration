using System.ComponentModel.DataAnnotations;

namespace RealTimeCollaboration.Modules.Reaction.DTOs;

public class UpdateReactionDTO
{
    [Required]
    [MaxLength(32)]
    public string OldEmoji { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string NewEmoji { get; set; } = string.Empty;
}

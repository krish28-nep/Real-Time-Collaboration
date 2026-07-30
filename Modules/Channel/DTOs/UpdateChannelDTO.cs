using System.ComponentModel.DataAnnotations;

namespace RealTimeCollaboration.Modules.Channel.DTOs;

public class UpdateChannelDTO
{
    [Required]
    public string Name { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace RealTimeCollaboration.Modules.Message.DTOs;

public class CreateMessageDTO
{
    [MaxLength(4000)]
    public string? Content { get; set; }

    public string[] Images { get; set; } = [];
}

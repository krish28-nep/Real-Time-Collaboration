namespace RealTimeCollaboration.Modules.Message.DTOs;

public class MessageResponseDTO
{
    public int Id { get; set; }
    public int ChannelId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Content { get; set; }
    public string[] Images { get; set; } = [];
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<MessageReactionDTO> Reactions { get; set; } = new();
}

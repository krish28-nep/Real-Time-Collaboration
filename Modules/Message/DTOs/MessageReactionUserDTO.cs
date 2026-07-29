namespace RealTimeCollaboration.Modules.Message.DTOs;

public class MessageReactionUserDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

namespace RealTimeCollaboration.Modules.Message.DTOs;

public class MessageReactionDTO
{
    public string Emoji { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool ReactedByMe { get; set; }
    public List<MessageReactionUserDTO> Users { get; set; } = new();
}

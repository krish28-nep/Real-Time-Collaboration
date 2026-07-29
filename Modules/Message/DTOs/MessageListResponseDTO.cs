namespace RealTimeCollaboration.Modules.Message.DTOs;

public class MessageListResponseDTO
{
    public List<MessageResponseDTO> Items { get; set; } = new();
    public int? NextCursor { get; set; }
    public bool HasMore { get; set; }
}

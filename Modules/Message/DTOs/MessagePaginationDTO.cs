namespace RealTimeCollaboration.Modules.Message.DTOs;

public class MessagePaginationDTO
{
    public int? BeforeMessageId { get; set; }
    public int Limit { get; set; } = 30;
}

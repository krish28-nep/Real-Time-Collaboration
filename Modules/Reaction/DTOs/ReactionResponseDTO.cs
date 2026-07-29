namespace RealTimeCollaboration.Modules.Reaction.DTOs;

public class ReactionResponseDTO
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

namespace RealTimeCollaboration.Modules.Reaction.Models;

using RealTimeCollaboration.Modules.Message.Models;

using RealTimeCollaboration.Modules.User.Models;



public class Reaction
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public Message Message { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public required string Emoji { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}

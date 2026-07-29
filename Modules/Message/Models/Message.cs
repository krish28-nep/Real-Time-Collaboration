namespace RealTimeCollaboration.Modules.Message.Models;

using RealTimeCollaboration.Modules.Channel.Models;
using RealTimeCollaboration.Modules.Reaction.Models;
using RealTimeCollaboration.Modules.User.Models;

public class Message
{
    public int Id { get; set; }
    public int ChannelId { get; set; }
    public Channel Channel { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string? Content { get; set; }

    public string[] Images { get; set; } = [];

    public bool IsEdited { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public List<Reaction> Reactions { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}

namespace RealTimeCollaboration.Modules.WorkSpace.Models;

using RealTimeCollaboration.Modules.User.Models;
using RealTimeCollaboration.Modules.Channel.Models;

public class WorkSpace
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public List <Channel> Channels { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

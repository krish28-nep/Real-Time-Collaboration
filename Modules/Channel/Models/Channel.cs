namespace RealTimeCollaboration.Modules.Channel.Models;
using RealTimeCollaboration.Modules.Message.Models;
using RealTimeCollaboration.Modules.WorkSpace.Models;

public class Channel
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }

    public int WorkSpaceId {get; set;}

    public WorkSpace WorkSpace {get; set;} = null!;

    public List<Message> Messages { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}

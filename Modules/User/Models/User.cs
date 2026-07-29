namespace RealTimeCollaboration.Modules.User.Models;
using RealTimeCollaboration.Modules.Message.Models;
using RealTimeCollaboration.Modules.Reaction.Models;
using RealTimeCollaboration.Modules.WorkSpace.Models;

public class User
{
    public int Id { get; set; }

    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public string? AvatarUrl { get; set; }

    public List <WorkSpace> WorkSpaces { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
    public List<Reaction> Reactions { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

namespace RealTimeCollaboration.Modules.WorkSpace.Models;

using RealTimeCollaboration.Modules.User.Models;
using RealTimeCollaboration.Modules.WorkSpace.Enums;

public class UserWorkSpace
{
    public int UserId { get; set; }

    public User User { get; set; } = null!;
    public int WorkSpaceById { get; set; }

    public WorkSpace WorkSpace { get; set; } = null!;

    public Role Role { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

}
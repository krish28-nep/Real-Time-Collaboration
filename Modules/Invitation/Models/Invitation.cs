namespace RealTimeCollaboration.Modules.Invitation.Models;

using RealTimeCollaboration.Modules.User.Models;
using RealTimeCollaboration.Modules.WorkSpace.Models;

public class Invitation
{
    public int Id { get; set; }
    public int WorkSpaceId { get; set; }

    public WorkSpace WorkSpace { get; set; } = null!;

    public required string Token { get; set; }

    public DateTime ExpireAt { get; set; }
    public DateTime? AcceptAt { get; set; }

    public int InvitedByUserId { get; set; }
    public User InvitedByUser { get; set; } = null!;

    public int? InvitedUserId { get; set; }
    public User? InvitedUser { get; set; }

}

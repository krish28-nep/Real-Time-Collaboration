namespace RealTimeCollaboration.Modules.Invitation.Models;

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

}
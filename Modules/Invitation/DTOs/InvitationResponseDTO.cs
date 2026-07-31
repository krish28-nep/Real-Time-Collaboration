namespace RealTimeCollaboration.Modules.Invitation.DTOs;

public class InvitationResponseDTO
{
    public int Id { get; set; }
    public int WorkSpaceId { get; set; }
    public string? WorkSpaceName { get; set; }
    public int? InvitedUserId { get; set; }
    public int InvitedByUserId { get; set; }
    public string? InvitedByUsername { get; set; }
    public string? InvitedByEmail { get; set; }
    public required string Token { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime? AcceptAt { get; set; }
}

using RealTimeCollaboration.Modules.WorkSpace.Enums;

namespace RealTimeCollaboration.Modules.WorkSpace.DTOs;

public class WorkSpaceUserDTO
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? AvatarUrl { get; set; }
    public Role Role { get; set; }
    public DateTime JoinedAt { get; set; }
}

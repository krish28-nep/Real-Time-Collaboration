namespace RealTimeCollaboration.Modules.WorkSpace.DTOs;

public class WorkSpaceResponseDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public int OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

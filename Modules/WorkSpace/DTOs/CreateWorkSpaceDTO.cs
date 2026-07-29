using System.ComponentModel.DataAnnotations;

namespace RealTimeCollaboration.Modules.WorkSpace.DTOs;

public class CreateWorkSpaceDTO
{
    [Required]
    [MaxLength(50)]
    public string name {get; set; } = string.Empty;
}
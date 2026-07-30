using RealTimeCollaboration.Modules.WorkSpace.DTOs;

namespace RealTimeCollaboration.Modules.WorkSpace.Interfaces;

public interface IWorkSpaceService
{
    Task<IEnumerable<WorkSpaceResponseDTO>> GetAllByUserIdAsync(int userId);

    Task<IEnumerable<WorkSpaceUserDTO>> GetAllUserByWorkspaceIdAsync(int workspaceId);

    Task<WorkSpaceResponseDTO?> GetByIdentifierAsync(string identifier, int userId);

    Task<WorkSpaceResponseDTO> CreateAsync(CreateWorkSpaceDTO workSpace, int userId);

    Task<bool> DeleteAsync(int id, int userId);
}

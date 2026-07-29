using RealTimeCollaboration.Modules.WorkSpace.DTOs;

namespace RealTimeCollaboration.Modules.WorkSpace.Interfaces;

public interface IWorkSpaceService
{
    Task<IEnumerable<Models.WorkSpace>> GetAllByUserIdAsync(int userId);

    Task<IEnumerable<WorkSpaceUserDTO>> GetAllUserByWorkspaceIdAsync(int workspaceId);

    Task<Models.WorkSpace?> GetByIdentifierAsync(string identifier, int userId);

    Task<Models.WorkSpace> CreateAsync(CreateWorkSpaceDTO workSpace, int userId);

    Task<bool> DeleteAsync(int id, int userId);
}

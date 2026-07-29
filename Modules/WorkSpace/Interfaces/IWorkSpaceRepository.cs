namespace RealTimeCollaboration.Modules.WorkSpace.Interfaces;
using RealTimeCollaboration.Modules.WorkSpace.DTOs;

public interface IWorkSpaceRepository
{
    Task<IEnumerable<Models.WorkSpace>> GetAllByUserIdAsync(int userId);

    Task<IEnumerable<WorkSpaceUserDTO>> GetAllUsersByWorkspaceIdAsync(int workspaceId);

    Task<Models.WorkSpace?> GetByIdentifierAsync(string identifier);

    Task<Models.WorkSpace> CreateAsync(Models.WorkSpace workSpace);

    Task<Models.UserWorkSpace> CreateUserWorkSpaceAsync(Models.UserWorkSpace userWorkSpace);

    Task<bool> IsUserMemberAsync(int userId, int workSpaceId);


    Task<bool> DeleteAsync(int id);
}

using RealTimeCollaboration.Common.Utils;
using RealTimeCollaboration.Modules.WorkSpace.DTOs;
using RealTimeCollaboration.Modules.WorkSpace.Interfaces;

namespace RealTimeCollaboration.Modules.WorkSpace;

public class WorkSpaceService : IWorkSpaceService
{
    private readonly IWorkSpaceRepository _workSpaceRepository;

    public WorkSpaceService(IWorkSpaceRepository workSpaceRepository)
    {
        _workSpaceRepository = workSpaceRepository;
    }

    public async Task<IEnumerable<Models.WorkSpace>> GetAllByUserIdAsync(int userId)
    {
        return await _workSpaceRepository.GetAllByUserIdAsync(userId);
    }

    public async Task<Models.WorkSpace?> GetByIdentifierAsync(string identifier, int userId)
    {
        var workSpace = await _workSpaceRepository.GetByIdentifierAsync(identifier);
        if (workSpace is null || workSpace.OwnerId != userId)
        {
            return null;
        }

        return workSpace;
    }

    public async Task<Models.WorkSpace> CreateAsync(CreateWorkSpaceDTO workSpace, int userId)
    {
        var newWorkSpace = new Models.WorkSpace
        {
            Name = workSpace.name,
            Slug = SlugGenerator.Create(workSpace.name),
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        newWorkSpace = await _workSpaceRepository.CreateAsync(newWorkSpace);

        var userWorkSpace = new Models.UserWorkSpace
        {
            UserId = userId,
            WorkSpaceById = newWorkSpace.Id,
            JoinedAt = DateTime.UtcNow,
            Role = Enums.Role.Owner
        };

        await _workSpaceRepository.CreateUserWorkSpaceAsync(userWorkSpace);

        return newWorkSpace;
    }

    public async Task<IEnumerable<WorkSpaceUserDTO>> GetAllUserByWorkspaceIdAsync(int workspaceId)
    {
        return await _workSpaceRepository.GetAllUsersByWorkspaceIdAsync(workspaceId);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var workSpace = await _workSpaceRepository.GetByIdentifierAsync(id.ToString());
        if (workSpace is null || workSpace.OwnerId != userId)
        {
            return false;
        }

        return await _workSpaceRepository.DeleteAsync(id);
    }
}

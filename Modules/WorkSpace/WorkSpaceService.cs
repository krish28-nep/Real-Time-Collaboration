using RealTimeCollaboration.Common.Utils;
using RealTimeCollaboration.Modules.WorkSpace.DTOs;
using RealTimeCollaboration.Modules.WorkSpace.Interfaces;
using ChannelModel = RealTimeCollaboration.Modules.Channel.Models.Channel;

namespace RealTimeCollaboration.Modules.WorkSpace;

public class WorkSpaceService : IWorkSpaceService
{
    private readonly IWorkSpaceRepository _workSpaceRepository;

    public WorkSpaceService(IWorkSpaceRepository workSpaceRepository)
    {
        _workSpaceRepository = workSpaceRepository;
    }

    public async Task<IEnumerable<WorkSpaceResponseDTO>> GetAllByUserIdAsync(int userId)
    {
        var workSpaces = await _workSpaceRepository.GetAllByUserIdAsync(userId);

        return workSpaces.Select(ToResponseDTO);
    }

    public async Task<WorkSpaceResponseDTO?> GetByIdentifierAsync(string identifier, int userId)
    {
        var workSpace = await _workSpaceRepository.GetByIdentifierAsync(identifier);
        if (workSpace is null || workSpace.OwnerId != userId)
        {
            return null;
        }

        return ToResponseDTO(workSpace);
    }

    public async Task<WorkSpaceResponseDTO> CreateAsync(CreateWorkSpaceDTO workSpace, int userId)
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

        var now = DateTime.UtcNow;
        var generalChannel = new ChannelModel
        {
            Name = "general",
            Slug = "general",
            WorkSpaceId = newWorkSpace.Id,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _workSpaceRepository.CreateChannelAsync(generalChannel);

        return ToResponseDTO(newWorkSpace);
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

    private static WorkSpaceResponseDTO ToResponseDTO(Models.WorkSpace workSpace)
    {
        return new WorkSpaceResponseDTO
        {
            Id = workSpace.Id,
            Name = workSpace.Name,
            Slug = workSpace.Slug,
            OwnerId = workSpace.OwnerId,
            CreatedAt = workSpace.CreatedAt,
            UpdatedAt = workSpace.UpdatedAt
        };
    }
}

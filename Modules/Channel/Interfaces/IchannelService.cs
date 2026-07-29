using RealTimeCollaboration.Modules.Channel.DTOs;

namespace RealTimeCollaboration.Modules.Channel.Interfaces;

public interface IChannelService
{
    Task<IEnumerable<Models.Channel>> GetAllByWorkSpaceIdAsync(int workspaceId);

    Task<Models.Channel> CreateAsync(CreateChannelDTO channel, int workspaceId);

    Task<bool> DeleteAsync(int id, int workspaceId);
}

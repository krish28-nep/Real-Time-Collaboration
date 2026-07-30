using RealTimeCollaboration.Modules.Channel.DTOs;

namespace RealTimeCollaboration.Modules.Channel.Interfaces;

public interface IChannelService
{
    Task<IEnumerable<Models.Channel>> GetAllByWorkSpaceIdAsync(int workspaceId);

    Task<Models.Channel> CreateAsync(CreateChannelDTO channel, int workspaceId);

    Task<Models.Channel?> UpdateAsync(int id, int workspaceId, UpdateChannelDTO channel);

    Task<bool> DeleteAsync(int id, int workspaceId);
}

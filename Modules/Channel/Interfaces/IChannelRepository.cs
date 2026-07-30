namespace RealTimeCollaboration.Modules.Channel.Interfaces;

public interface IChannelRepository
{
    Task<IEnumerable<Models.Channel>> GetAllByWorkSpaceIdAsync(int workspaceId);

    Task<Models.Channel> CreateAsync(Models.Channel channel);

    Task<Models.Channel?> UpdateAsync(int id, int workspaceId, string name, string slug);

    Task<bool> DeleteAsync(int id, int workspaceId);
}

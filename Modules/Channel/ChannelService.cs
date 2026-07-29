using RealTimeCollaboration.Common.Utils;
using RealTimeCollaboration.Modules.Channel.DTOs;
using RealTimeCollaboration.Modules.Channel.Interfaces;

namespace RealTimeCollaboration.Modules.Channel;

public class ChannelService : IChannelService
{
    private readonly IChannelRepository _channelRepository;

    public ChannelService(IChannelRepository channelRepository)
    {
        _channelRepository = channelRepository;
    }

    public async Task<IEnumerable<Models.Channel>> GetAllByWorkSpaceIdAsync(int workspaceId)
    {
        return await _channelRepository.GetAllByWorkSpaceIdAsync(workspaceId);
    }

    public async Task<Models.Channel> CreateAsync(CreateChannelDTO channel, int workspaceId)
    {
        var newChannel = new Models.Channel
        {
            Name = channel.Name,
            Slug = SlugGenerator.Create(channel.Name),
            WorkSpaceId = workspaceId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _channelRepository.CreateAsync(newChannel);
    }

    public async Task<bool> DeleteAsync(int id, int workspaceId)
    {
        return await _channelRepository.DeleteAsync(id, workspaceId);
    }
}

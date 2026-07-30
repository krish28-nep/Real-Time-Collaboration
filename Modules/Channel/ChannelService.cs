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
        var name = NormalizeName(channel.Name);
        var newChannel = new Models.Channel
        {
            Name = name,
            Slug = SlugGenerator.Create(name),
            WorkSpaceId = workspaceId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _channelRepository.CreateAsync(newChannel);
    }

    public async Task<Models.Channel?> UpdateAsync(int id, int workspaceId, UpdateChannelDTO channel)
    {
        var name = NormalizeName(channel.Name);
        return await _channelRepository.UpdateAsync(id, workspaceId, name, SlugGenerator.Create(name));
    }

    public async Task<bool> DeleteAsync(int id, int workspaceId)
    {
        return await _channelRepository.DeleteAsync(id, workspaceId);
    }

    private static string NormalizeName(string name)
    {
        var normalizedName = name.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException("Channel name is required.");
        }

        return normalizedName;
    }
}

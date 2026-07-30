using Microsoft.EntityFrameworkCore;
using RealTimeCollaboration.Data;
using RealTimeCollaboration.Modules.Channel.Interfaces;

namespace RealTimeCollaboration.Modules.Channel;

public class ChannelRepository : IChannelRepository
{
    private readonly AppDbContext _context;

    public ChannelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Models.Channel>> GetAllByWorkSpaceIdAsync(int workspaceId)
    {
        return await _context.Channels
            .AsNoTracking()
            .Where(channel => channel.WorkSpaceId == workspaceId)
            .OrderBy(channel => channel.Id)
            .ToListAsync();
    }

    public async Task<Models.Channel> CreateAsync(Models.Channel channel)
    {
        _context.Channels.Add(channel);
        await _context.SaveChangesAsync();

        return channel;
    }

    public async Task<Models.Channel?> UpdateAsync(int id, int workspaceId, string name, string slug)
    {
        var channel = await _context.Channels
            .FirstOrDefaultAsync(channel => channel.Id == id && channel.WorkSpaceId == workspaceId);

        if (channel is null)
        {
            return null;
        }

        channel.Name = name;
        channel.Slug = slug;
        channel.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return channel;
    }

    public async Task<bool> DeleteAsync(int id, int workspaceId)
    {
        var channel = await _context.Channels
            .FirstOrDefaultAsync(channel => channel.Id == id && channel.WorkSpaceId == workspaceId);

        if (channel is null)
        {
            return false;
        }

        _context.Channels.Remove(channel);
        await _context.SaveChangesAsync();

        return true;
    }
}

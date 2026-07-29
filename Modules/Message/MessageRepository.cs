using Microsoft.EntityFrameworkCore;
using RealTimeCollaboration.Data;
using RealTimeCollaboration.Modules.Message.Interfaces;

namespace RealTimeCollaboration.Modules.Message;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Message> CreateAsync(Models.Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<Models.Message?> GetByIdAsync(int id)
    {
        return await _context.Messages.FirstOrDefaultAsync(message => message.Id == id);
    }

    public async Task<Models.Message?> GetByIdWithReactionsAsync(int id)
    {
        return await _context.Messages
            .AsNoTracking()
            .Include(message => message.User)
            .Include(message => message.Reactions)
                .ThenInclude(reaction => reaction.User)
            .FirstOrDefaultAsync(message => message.Id == id);
    }

    public async Task<List<Models.Message>> GetByChannelIdAsync(int channelId, int? beforeMessageId, int limit)
    {
        var query = _context.Messages
            .AsNoTracking()
            .Include(message => message.User)
            .Include(message => message.Reactions)
                .ThenInclude(reaction => reaction.User)
            .Where(message => message.ChannelId == channelId);

        if (beforeMessageId is not null)
        {
            var cursorMessage = await _context.Messages
                .AsNoTracking()
                .FirstOrDefaultAsync(message => message.Id == beforeMessageId && message.ChannelId == channelId);

            if (cursorMessage is null)
            {
                return new List<Models.Message>();
            }

            query = query.Where(message =>
                message.CreatedAt < cursorMessage.CreatedAt
                || (message.CreatedAt == cursorMessage.CreatedAt && message.Id < cursorMessage.Id));
        }

        return await query
            .OrderByDescending(message => message.CreatedAt)
            .ThenByDescending(message => message.Id)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> HasMoreBeforeAsync(int channelId, Models.Message oldestMessage)
    {
        return await _context.Messages
            .AsNoTracking()
            .AnyAsync(message =>
                message.ChannelId == channelId
                && (message.CreatedAt < oldestMessage.CreatedAt
                    || (message.CreatedAt == oldestMessage.CreatedAt && message.Id < oldestMessage.Id)));
    }

    public async Task<bool> SoftDeleteAsync(int id, int channelId, int userId)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(message =>
                message.Id == id
                && message.ChannelId == channelId
                && message.UserId == userId);

        if (message is null)
        {
            return false;
        }

        message.Content = null;
        message.Images = [];
        message.IsDeleted = true;
        message.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}

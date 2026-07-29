using Microsoft.EntityFrameworkCore;
using RealTimeCollaboration.Data;
using RealTimeCollaboration.Modules.Reaction.Interfaces;

namespace RealTimeCollaboration.Modules.Reaction;

public class ReactionRepository : IReactionRepository
{
    private readonly AppDbContext _context;

    public ReactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Reaction?> GetAsync(int messageId, int userId, string emoji)
    {
        return await _context.Reactions
            .FirstOrDefaultAsync(reaction =>
                reaction.MessageId == messageId
                && reaction.UserId == userId
                && reaction.Emoji == emoji);
    }

    public async Task<Models.Reaction> CreateAsync(Models.Reaction reaction)
    {
        _context.Reactions.Add(reaction);
        await _context.SaveChangesAsync();

        return reaction;
    }

    public async Task<bool> DeleteAsync(int messageId, int userId, string emoji)
    {
        var reaction = await GetAsync(messageId, userId, emoji);
        if (reaction is null)
        {
            return false;
        }

        _context.Reactions.Remove(reaction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<Models.Reaction?> UpdateAsync(int messageId, int userId, string oldEmoji, string newEmoji)
    {
        var reaction = await GetAsync(messageId, userId, oldEmoji);
        if (reaction is null)
        {
            return null;
        }

        reaction.Emoji = newEmoji;
        await _context.SaveChangesAsync();

        return reaction;
    }
}

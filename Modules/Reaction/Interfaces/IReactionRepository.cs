namespace RealTimeCollaboration.Modules.Reaction.Interfaces;

public interface IReactionRepository
{
    Task<Models.Reaction?> GetAsync(int messageId, int userId, string emoji);
    Task<Models.Reaction> CreateAsync(Models.Reaction reaction);
    Task<bool> DeleteAsync(int messageId, int userId, string emoji);
    Task<Models.Reaction?> UpdateAsync(int messageId, int userId, string oldEmoji, string newEmoji);
}

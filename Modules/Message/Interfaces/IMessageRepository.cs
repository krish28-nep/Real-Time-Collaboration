namespace RealTimeCollaboration.Modules.Message.Interfaces;

public interface IMessageRepository
{
    Task<Models.Message> CreateAsync(Models.Message message);
    Task<Models.Message?> GetByIdAsync(int id);
    Task<Models.Message?> GetByIdWithReactionsAsync(int id);
    Task<List<Models.Message>> GetByChannelIdAsync(int channelId, int? beforeMessageId, int limit);
    Task<bool> HasMoreBeforeAsync(int channelId, Models.Message oldestMessage);
    Task<bool> SoftDeleteAsync(int id, int channelId, int userId);
}

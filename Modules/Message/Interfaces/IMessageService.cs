using RealTimeCollaboration.Modules.Message.DTOs;

namespace RealTimeCollaboration.Modules.Message.Interfaces;

public interface IMessageService
{
    Task<MessageResponseDTO> CreateAsync(int channelId, int userId, CreateMessageDTO createMessageDTO);
    Task<MessageListResponseDTO> GetByChannelIdAsync(int channelId, int userId, MessagePaginationDTO paginationDTO);
    Task<bool> DeleteAsync(int id, int channelId, int userId);
}

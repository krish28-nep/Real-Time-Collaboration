using RealTimeCollaboration.Modules.Message.DTOs;
using RealTimeCollaboration.Modules.Message.Interfaces;

namespace RealTimeCollaboration.Modules.Message;

public class MessageService : IMessageService
{
    private const int MaxPageSize = 100;
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageResponseDTO> CreateAsync(int channelId, int userId, CreateMessageDTO createMessageDTO)
    {
        if (string.IsNullOrWhiteSpace(createMessageDTO.Content) && createMessageDTO.Images.Length == 0)
        {
            throw new ArgumentException("Message content or image is required.");
        }

        var now = DateTime.UtcNow;
        var message = new Models.Message
        {
            ChannelId = channelId,
            UserId = userId,
            Content = string.IsNullOrWhiteSpace(createMessageDTO.Content) ? null : createMessageDTO.Content.Trim(),
            Images = createMessageDTO.Images,
            CreatedAt = now,
            UpdatedAt = now
        };

        var createdMessage = await _messageRepository.CreateAsync(message);
        var messageWithReactions = await _messageRepository.GetByIdWithReactionsAsync(createdMessage.Id);

        return ToResponseDTO(messageWithReactions ?? createdMessage, userId);
    }

    public async Task<MessageListResponseDTO> GetByChannelIdAsync(
        int channelId,
        int userId,
        MessagePaginationDTO paginationDTO)
    {
        var limit = Math.Clamp(paginationDTO.Limit, 1, MaxPageSize);
        var messages = await _messageRepository.GetByChannelIdAsync(channelId, paginationDTO.BeforeMessageId, limit);
        var oldestMessage = messages.LastOrDefault();

        return new MessageListResponseDTO
        {
            Items = messages.Select(message => ToResponseDTO(message, userId)).ToList(),
            NextCursor = oldestMessage?.Id,
            HasMore = oldestMessage is not null && await _messageRepository.HasMoreBeforeAsync(channelId, oldestMessage)
        };
    }

    public async Task<bool> DeleteAsync(int id, int channelId, int userId)
    {
        return await _messageRepository.SoftDeleteAsync(id, channelId, userId);
    }

    private static MessageResponseDTO ToResponseDTO(Models.Message message, int currentUserId)
    {
        return new MessageResponseDTO
        {
            Id = message.Id,
            ChannelId = message.ChannelId,
            UserId = message.UserId,
            Username = message.User?.Username ?? string.Empty,
            AvatarUrl = message.User?.AvatarUrl,
            Content = message.IsDeleted ? null : message.Content,
            Images = message.IsDeleted ? [] : message.Images,
            IsEdited = message.IsEdited,
            IsDeleted = message.IsDeleted,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt,
            Reactions = message.Reactions
                .GroupBy(reaction => reaction.Emoji)
                .Select(group => new MessageReactionDTO
                {
                    Emoji = group.Key,
                    Count = group.Count(),
                    ReactedByMe = group.Any(reaction => reaction.UserId == currentUserId),
                    Users = group.Select(reaction => new MessageReactionUserDTO
                    {
                        Id = reaction.UserId,
                        Username = reaction.User?.Username ?? string.Empty,
                        AvatarUrl = reaction.User?.AvatarUrl
                    }).ToList()
                })
                .OrderByDescending(reaction => reaction.Count)
                .ThenBy(reaction => reaction.Emoji)
                .ToList()
        };
    }
}

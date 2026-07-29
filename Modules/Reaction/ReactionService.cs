using Microsoft.EntityFrameworkCore;
using RealTimeCollaboration.Modules.Message.Interfaces;
using RealTimeCollaboration.Modules.Reaction.DTOs;
using RealTimeCollaboration.Modules.Reaction.Interfaces;

namespace RealTimeCollaboration.Modules.Reaction;

public class ReactionService : IReactionService
{
    private readonly IReactionRepository _reactionRepository;
    private readonly IMessageRepository _messageRepository;

    public ReactionService(IReactionRepository reactionRepository, IMessageRepository messageRepository)
    {
        _reactionRepository = reactionRepository;
        _messageRepository = messageRepository;
    }

    public async Task<ReactionResponseDTO> CreateAsync(int messageId, int userId, CreateReactionDTO createReactionDTO)
    {
        var emoji = NormalizeEmoji(createReactionDTO.Emoji);
        var message = await _messageRepository.GetByIdAsync(messageId);

        if (message is null || message.IsDeleted)
        {
            throw new ArgumentException("Message not found.");
        }

        var existingReaction = await _reactionRepository.GetAsync(messageId, userId, emoji);
        if (existingReaction is not null)
        {
            throw new InvalidOperationException("Reaction already exists.");
        }

        var reaction = new Models.Reaction
        {
            MessageId = messageId,
            UserId = userId,
            Emoji = emoji,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            return ToResponseDTO(await _reactionRepository.CreateAsync(reaction));
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Reaction already exists.");
        }
    }

    public async Task<bool> DeleteAsync(int messageId, int userId, string emoji)
    {
        return await _reactionRepository.DeleteAsync(messageId, userId, NormalizeEmoji(emoji));
    }

    public async Task<ReactionResponseDTO?> UpdateAsync(int messageId, int userId, UpdateReactionDTO updateReactionDTO)
    {
        var oldEmoji = NormalizeEmoji(updateReactionDTO.OldEmoji);
        var newEmoji = NormalizeEmoji(updateReactionDTO.NewEmoji);

        if (oldEmoji == newEmoji)
        {
            var existingReaction = await _reactionRepository.GetAsync(messageId, userId, oldEmoji);
            return existingReaction is null ? null : ToResponseDTO(existingReaction);
        }

        var duplicateReaction = await _reactionRepository.GetAsync(messageId, userId, newEmoji);
        if (duplicateReaction is not null)
        {
            throw new InvalidOperationException("Reaction already exists.");
        }

        try
        {
            var updatedReaction = await _reactionRepository.UpdateAsync(messageId, userId, oldEmoji, newEmoji);
            return updatedReaction is null ? null : ToResponseDTO(updatedReaction);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Reaction already exists.");
        }
    }

    private static string NormalizeEmoji(string emoji)
    {
        var normalizedEmoji = emoji.Trim();
        if (string.IsNullOrWhiteSpace(normalizedEmoji))
        {
            throw new ArgumentException("Emoji is required.");
        }

        return normalizedEmoji;
    }

    private static ReactionResponseDTO ToResponseDTO(Models.Reaction reaction)
    {
        return new ReactionResponseDTO
        {
            Id = reaction.Id,
            MessageId = reaction.MessageId,
            UserId = reaction.UserId,
            Emoji = reaction.Emoji,
            CreatedAt = reaction.CreatedAt
        };
    }
}

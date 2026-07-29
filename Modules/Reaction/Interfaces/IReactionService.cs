using RealTimeCollaboration.Modules.Reaction.DTOs;

namespace RealTimeCollaboration.Modules.Reaction.Interfaces;

public interface IReactionService
{
    Task<ReactionResponseDTO> CreateAsync(int messageId, int userId, CreateReactionDTO createReactionDTO);
    Task<bool> DeleteAsync(int messageId, int userId, string emoji);
    Task<ReactionResponseDTO?> UpdateAsync(int messageId, int userId, UpdateReactionDTO updateReactionDTO);
}

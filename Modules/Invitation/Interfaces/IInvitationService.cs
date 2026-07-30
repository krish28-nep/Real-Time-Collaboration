namespace RealTimeCollaboration.Modules.Invitation.Interfaces;
using RealTimeCollaboration.Modules.Invitation.DTOs;

public interface IInvitationService
{
    Task<InvitationResponseDTO> CreateAsync(int workSpaceId, int invitedByUserId, int invitedUserId);

    Task<InvitationResponseDTO?> GetByTokenAsync(string token);

    Task<IEnumerable<InvitationResponseDTO>> GetPendingByUserIdAsync(int userId);

    // Returns (success, errorMessage)
    Task<(bool success, string? error)> ConsumeAsync(string token, int joiningUserId);
}

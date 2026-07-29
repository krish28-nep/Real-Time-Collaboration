namespace RealTimeCollaboration.Modules.Invitation.Interfaces;

public interface IInvitationService
{
    Task<Models.Invitation> CreateAsync(int workSpaceId, int invitedByUserId);

    Task<Models.Invitation?> GetByTokenAsync(string token);

    // Returns (success, errorMessage)
    Task<(bool success, string? error)> ConsumeAsync(string token, int joiningUserId);
}

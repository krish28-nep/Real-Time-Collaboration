namespace RealTimeCollaboration.Modules.Invitation.Interfaces;

public interface IInvitationRepository
{
    Task<Models.Invitation> CreateAsync(Models.Invitation invitation);

    Task<Models.Invitation?> GetByTokenAsync(string token);

    Task<bool> DeleteAsync(int id);

    Task<int> DeleteExpiredAsync(DateTime now);

    Task MarkAcceptedAsync(Models.Invitation invitation);
}
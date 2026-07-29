using RealTimeCollaboration.Modules.Invitation.Interfaces;
using RealTimeCollaboration.Modules.WorkSpace.Interfaces;

namespace RealTimeCollaboration.Modules.Invitation;

public class InvitationService : IInvitationService
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IWorkSpaceRepository _workSpaceRepository;

    public InvitationService(IInvitationRepository invitationRepository, IWorkSpaceRepository workSpaceRepository)
    {
        _invitationRepository = invitationRepository;
        _workSpaceRepository = workSpaceRepository;
    }

    public async Task<Models.Invitation> CreateAsync(int workSpaceId, int invitedByUserId)
    {
        var token = Guid.NewGuid().ToString("N");
        var invitation = new Models.Invitation
        {
            WorkSpaceId = workSpaceId,
            Token = token,
            ExpireAt = DateTime.UtcNow.AddMinutes(30),
            InvitedByUserId = invitedByUserId,
        };

        return await _invitationRepository.CreateAsync(invitation);
    }

    public async Task<Models.Invitation?> GetByTokenAsync(string token)
    {
        var inv = await _invitationRepository.GetByTokenAsync(token);
        if (inv is null) return null;
        if (inv.ExpireAt <= DateTime.UtcNow) return null;
        return inv;
    }

    public async Task<(bool success, string? error)> ConsumeAsync(string token, int joiningUserId)
    {
        var inv = await _invitationRepository.GetByTokenAsync(token);
        if (inv is null) return (false, "Invalid token");

        if (inv.AcceptAt is not null) return (false, "Invitation already accepted");

        if (inv.ExpireAt <= DateTime.UtcNow) return (false, "Invitation expired");

        var ws = await _workSpaceRepository.GetByIdentifierAsync(inv.WorkSpaceId.ToString());
        if (ws is null) return (false, "Workspace not found");

        var alreadyMember = await _workSpaceRepository.IsUserMemberAsync(joiningUserId, inv.WorkSpaceId);
        if (alreadyMember) return (false, "User is already a member of the workspace");

        var userWorkSpace = new RealTimeCollaboration.Modules.WorkSpace.Models.UserWorkSpace
        {
            UserId = joiningUserId,
            WorkSpaceById = inv.WorkSpaceId,
            JoinedAt = DateTime.UtcNow
        };

        await _workSpaceRepository.CreateUserWorkSpaceAsync(userWorkSpace);
        await _invitationRepository.MarkAcceptedAsync(inv);
        return (true, null);
    }
}

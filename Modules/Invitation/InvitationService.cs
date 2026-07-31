using RealTimeCollaboration.Modules.Invitation.Interfaces;
using RealTimeCollaboration.Modules.Invitation.DTOs;
using RealTimeCollaboration.Modules.User.Interfaces;
using RealTimeCollaboration.Modules.WorkSpace.Interfaces;

namespace RealTimeCollaboration.Modules.Invitation;

public class InvitationService : IInvitationService
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IWorkSpaceRepository _workSpaceRepository;
    private readonly IUserRepository _userRepository;

    public InvitationService(
        IInvitationRepository invitationRepository,
        IWorkSpaceRepository workSpaceRepository,
        IUserRepository userRepository)
    {
        _invitationRepository = invitationRepository;
        _workSpaceRepository = workSpaceRepository;
        _userRepository = userRepository;
    }

    public async Task<InvitationResponseDTO> CreateAsync(int workSpaceId, int invitedByUserId, int invitedUserId)
    {
        if (invitedByUserId == invitedUserId)
        {
            throw new ArgumentException("You cannot invite yourself.");
        }

        var workSpace = await _workSpaceRepository.GetByIdentifierAsync(workSpaceId.ToString());
        if (workSpace is null)
        {
            throw new ArgumentException("Workspace not found.");
        }

        var inviterIsMember = await _workSpaceRepository.IsUserMemberAsync(invitedByUserId, workSpaceId);
        if (!inviterIsMember)
        {
            throw new InvalidOperationException("You are not a member of this workspace.");
        }

        var invitedUser = await _userRepository.GetByIdAsync(invitedUserId);
        if (invitedUser is null)
        {
            throw new ArgumentException("User not found.");
        }

        var alreadyMember = await _workSpaceRepository.IsUserMemberAsync(invitedUserId, workSpaceId);
        if (alreadyMember)
        {
            throw new InvalidOperationException("User is already a member of this workspace.");
        }

        var token = Guid.NewGuid().ToString("N");
        var invitation = new Models.Invitation
        {
            WorkSpaceId = workSpaceId,
            Token = token,
            ExpireAt = DateTime.UtcNow.AddMinutes(30),
            InvitedByUserId = invitedByUserId,
            InvitedUserId = invitedUserId
        };

        return ToResponseDTO(await _invitationRepository.CreateAsync(invitation));
    }

    public async Task<InvitationResponseDTO?> GetByTokenAsync(string token)
    {
        var inv = await _invitationRepository.GetByTokenAsync(token);
        if (inv is null) return null;
        if (inv.ExpireAt <= DateTime.UtcNow) return null;
        return ToResponseDTO(inv);
    }

    public async Task<IEnumerable<InvitationResponseDTO>> GetPendingByUserIdAsync(int userId)
    {
        var invitations = await _invitationRepository.GetPendingByUserIdAsync(userId, DateTime.UtcNow);

        return invitations.Select(ToResponseDTO);
    }

    public async Task<(bool success, string? error)> ConsumeAsync(string token, int joiningUserId)
    {
        var inv = await _invitationRepository.GetByTokenAsync(token);
        if (inv is null) return (false, "Invalid token");

        if (inv.AcceptAt is not null) return (false, "Invitation already accepted");

        if (inv.ExpireAt <= DateTime.UtcNow) return (false, "Invitation expired");

        if (inv.InvitedUserId is not null && inv.InvitedUserId != joiningUserId)
        {
            return (false, "This invitation is for another user");
        }

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

    private static InvitationResponseDTO ToResponseDTO(Models.Invitation invitation)
    {
        return new InvitationResponseDTO
        {
            Id = invitation.Id,
            WorkSpaceId = invitation.WorkSpaceId,
            WorkSpaceName = invitation.WorkSpace?.Name,
            InvitedUserId = invitation.InvitedUserId,
            InvitedByUserId = invitation.InvitedByUserId,
            InvitedByUsername = invitation.InvitedByUser?.Username,
            InvitedByEmail = invitation.InvitedByUser?.Email,
            Token = invitation.Token,
            ExpireAt = invitation.ExpireAt,
            AcceptAt = invitation.AcceptAt
        };
    }
}

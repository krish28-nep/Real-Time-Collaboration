using Microsoft.EntityFrameworkCore;
using RealTimeCollaboration.Data;
using RealTimeCollaboration.Modules.Invitation.Interfaces;

namespace RealTimeCollaboration.Modules.Invitation;

public class InvitationRepository : IInvitationRepository
{
    private readonly AppDbContext _context;

    public InvitationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Invitation> CreateAsync(Models.Invitation invitation)
    {
        _context.Invitations.Add(invitation);
        await _context.SaveChangesAsync();
        return await _context.Invitations
            .AsNoTracking()
            .Include(invitation => invitation.WorkSpace)
            .Include(invitation => invitation.InvitedByUser)
            .FirstAsync(existingInvitation => existingInvitation.Id == invitation.Id);
    }

    public async Task<Models.Invitation?> GetByTokenAsync(string token)
    {
        return await _context.Invitations
            .AsNoTracking()
            .Include(invitation => invitation.WorkSpace)
            .Include(invitation => invitation.InvitedByUser)
            .FirstOrDefaultAsync(i => i.Token == token);
    }

    public async Task<IEnumerable<Models.Invitation>> GetPendingByUserIdAsync(int userId, DateTime now)
    {
        return await _context.Invitations
            .AsNoTracking()
            .Include(invitation => invitation.WorkSpace)
            .Include(invitation => invitation.InvitedByUser)
            .Where(invitation =>
                invitation.InvitedUserId == userId
                && invitation.AcceptAt == null
                && invitation.ExpireAt > now)
            .OrderBy(invitation => invitation.ExpireAt)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var inv = await _context.Invitations.FindAsync(id);
        if (inv is null) return false;
        _context.Invitations.Remove(inv);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> DeleteExpiredAsync(DateTime now)
    {
        var expired = await _context.Invitations
            .Where(i => i.ExpireAt <= now)
            .ToListAsync();

        if (expired.Count == 0) return 0;

        _context.Invitations.RemoveRange(expired);
        await _context.SaveChangesAsync();
        return expired.Count;
    }

    public async Task MarkAcceptedAsync(Models.Invitation invitation)
    {
        var existing = await _context.Invitations.FindAsync(invitation.Id);
        if (existing is null) return;
        existing.AcceptAt = DateTime.UtcNow;
        _context.Invitations.Update(existing);
        await _context.SaveChangesAsync();
    }
}

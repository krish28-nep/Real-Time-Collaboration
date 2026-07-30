using Microsoft.EntityFrameworkCore;
using RealTimeCollaboration.Data;
using RealTimeCollaboration.Modules.WorkSpace.DTOs;
using RealTimeCollaboration.Modules.WorkSpace.Interfaces;
using ChannelModel = RealTimeCollaboration.Modules.Channel.Models.Channel;

namespace RealTimeCollaboration.Modules.WorkSpace;

public class WorkSpaceRepository : IWorkSpaceRepository
{
    private readonly AppDbContext _context;

    public WorkSpaceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Models.WorkSpace>> GetAllByUserIdAsync(int userId)
    {
        return await _context.WorkSpaces
        .AsNoTracking()
        .Where(w => w.OwnerId == userId)
        .OrderBy(w => w.Id)
        .ToListAsync();
    }

    public async Task<Models.WorkSpace?> GetByIdentifierAsync(string identifier)
    {
        if (int.TryParse(identifier, out var id))
        {
            var workSpaceById = await _context.WorkSpaces
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workSpaceById is not null)
            {
                return workSpaceById;
            }
        }

        return await _context.WorkSpaces
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Slug == identifier);
    }

    public async Task<IEnumerable<WorkSpaceUserDTO>> GetAllUsersByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.UserWorkSpaces
            .AsNoTracking()
            .Where(userWorkSpace => userWorkSpace.WorkSpaceById == workspaceId)
            .OrderBy(userWorkSpace => userWorkSpace.JoinedAt)
            .Select(userWorkSpace => new WorkSpaceUserDTO
            {
                Id = userWorkSpace.User.Id,
                Username = userWorkSpace.User.Username,
                Email = userWorkSpace.User.Email,
                AvatarUrl = userWorkSpace.User.AvatarUrl,
                Role = userWorkSpace.Role,
                JoinedAt = userWorkSpace.JoinedAt
            })
            .ToListAsync();
    }

    public async Task<Models.WorkSpace> CreateAsync(Models.WorkSpace workSpace)
    {
        _context.WorkSpaces.Add(workSpace);
        await _context.SaveChangesAsync();

        return workSpace;
    }

    public async Task<Models.UserWorkSpace> CreateUserWorkSpaceAsync(Models.UserWorkSpace userWorkSpace)
    {
        _context.UserWorkSpaces.Add(userWorkSpace);
        await _context.SaveChangesAsync();
        return userWorkSpace;
    }

    public async Task<ChannelModel> CreateChannelAsync(ChannelModel channel)
    {
        _context.Channels.Add(channel);
        await _context.SaveChangesAsync();
        return channel;
    }

    public async Task<bool> IsUserMemberAsync(int userId, int workSpaceId)
    {
        return await _context.UserWorkSpaces
            .AsNoTracking()
            .AnyAsync(uw => uw.UserId == userId && uw.WorkSpaceById == workSpaceId);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var workSpace = await _context.WorkSpaces.FindAsync(id);
        if (workSpace is null)
        {
            return false;
        }

        _context.WorkSpaces.Remove(workSpace);
        await _context.SaveChangesAsync();

        return true;
    }
}

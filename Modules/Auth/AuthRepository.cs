using Microsoft.EntityFrameworkCore;
using RealTimeCollaboration.Data;
using RealTimeCollaboration.Modules.Auth.Interfaces;
using UserModel = RealTimeCollaboration.Modules.User.Models.User;

namespace RealTimeCollaboration.Modules.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserModel?> GetUser(string user)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(existingUser =>
                existingUser.Email == user || existingUser.Username == user);
    }
}

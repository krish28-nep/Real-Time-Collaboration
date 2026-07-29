using RealTimeCollaboration.Modules.User.DTOs;

namespace RealTimeCollaboration.Modules.User.Interfaces;

public interface IUserService
{
    Task<IEnumerable<Models.User>> GetAllUsersAsync();
    Task<Models.User?> GetUserByIdAsync(int id);
    Task<Models.User?> GetUserByUsernameAsync(string username);
    Task<Models.User> CreateUserAsync(CreateUserDTO createUserDto);
    Task<bool> DeleteUserAsync(int id);
}

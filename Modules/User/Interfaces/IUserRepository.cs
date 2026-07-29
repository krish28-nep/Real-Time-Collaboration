
namespace RealTimeCollaboration.Modules.User.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<Models.User>> GetAllAsync();
    Task<Models.User?> GetByIdAsync(int id);
    Task<Models.User?> GetByUsernameAsync(string username);
    Task<Models.User?> GetByEmailAsync(string email);
    Task<Models.User> CreateAsync(Models.User user);
    Task<bool> DeleteAsync(int id);
}

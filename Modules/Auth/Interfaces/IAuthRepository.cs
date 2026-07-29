using UserModel = RealTimeCollaboration.Modules.User.Models.User;
namespace RealTimeCollaboration.Modules.Auth.Interfaces;

public interface IAuthRepository
{
    Task<UserModel?> GetUser(string user);
}

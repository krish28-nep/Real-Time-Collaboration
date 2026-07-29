using RealTimeCollaboration.Modules.Auth.DTOs;

namespace RealTimeCollaboration.Modules.Auth.Interfaces;

public interface IAuthService
{
    Task<string?> Login(LoginDTO loginDto);
}

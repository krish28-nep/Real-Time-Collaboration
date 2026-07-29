using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RealTimeCollaboration.Modules.Auth.DTOs;
using RealTimeCollaboration.Modules.Auth.Interfaces;
using UserModel = RealTimeCollaboration.Modules.User.Models.User;

namespace RealTimeCollaboration.Modules.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<UserModel> _passwordHasher;

    public AuthService(
        IAuthRepository authRepository,
        IConfiguration configuration,
        IPasswordHasher<UserModel> passwordHasher)
    {
        _authRepository = authRepository;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<string?> Login(LoginDTO loginDto)
    {
        var user = await _authRepository.GetUser(loginDto.User);

        if (user is null || !ValidatePassword(user, loginDto.Password))
        {
            return null;
        }

        return GenerateJwtToken(user);
    }

    private bool ValidatePassword(UserModel user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        return result != PasswordVerificationResult.Failed;
    }

    private string GenerateJwtToken(UserModel user)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var jwtIssuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var jwtAudience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
        var expiresInMinutes = _configuration.GetValue("Jwt:ExpiresInMinutes", 60);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

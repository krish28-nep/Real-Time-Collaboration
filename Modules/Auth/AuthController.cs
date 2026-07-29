using Microsoft.AspNetCore.Mvc;
using RealTimeCollaboration.Modules.Auth.DTOs;
using RealTimeCollaboration.Modules.Auth.Interfaces;

namespace RealTimeCollaboration.Modules.Auth;

[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private const string AccessTokenCookieName = "Access_Token";

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var token = await _authService.Login(loginDto);

        if (token is null)
        {
            return Unauthorized();
        }

        Response.Cookies.Append(AccessTokenCookieName, token, CreateAccessTokenCookieOptions());

        return Ok(new { token });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        if (Request.Cookies.ContainsKey(AccessTokenCookieName))
        {
            Response.Cookies.Delete(AccessTokenCookieName, CreateAccessTokenCookieOptions());
        }

        return Ok(new { message = "Logout successful" });
    }

    private CookieOptions CreateAccessTokenCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        };
    }
}

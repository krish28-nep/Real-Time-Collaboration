using System.Security.Claims;

namespace RealTimeCollaboration.Modules.Auth.Utils;

public static class AuthUserContext
{
    public static int? GetCurrentUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}

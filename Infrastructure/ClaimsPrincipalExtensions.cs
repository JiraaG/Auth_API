using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth_API.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user) =>
            user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID missing");

        public static string GetEmail(this ClaimsPrincipal user) =>
            user.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? user.FindFirst(ClaimTypes.Email)?.Value
            ?? throw new InvalidOperationException("Email missing");

        public static string GetFirstName(this ClaimsPrincipal user) =>
            user.FindFirst(ClaimTypes.GivenName)?.Value
            ?? user.FindFirst("firstName")?.Value
            ?? throw new InvalidOperationException("First name missing");
    }
}

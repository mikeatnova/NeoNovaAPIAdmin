using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NeoNovaAPIAdmin.Helpers
{
    public class JwtExtractorHelper
    {
        public ClaimsPrincipal? GetClaimsFromJwt(string? token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, "custom");
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }

        public string? ExtractGeneratedPasswordFromJwt(string? token)
        {
            var claimsPrincipal = GetClaimsFromJwt(token);
            if (claimsPrincipal != null)
            {
                var generatedPasswordClaim = claimsPrincipal.FindFirst("GeneratedPassword");
                return generatedPasswordClaim?.Value;
            }
            return null;
        }
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NeoNovaAPIAdmin.Helpers
{
    public class JwtExtractorHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtExtractorHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? GetClaimsFromJwt()
        {
            string? token = _httpContextAccessor.HttpContext?.Request.Cookies["NeoWebAppCookie"];
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
    }
}

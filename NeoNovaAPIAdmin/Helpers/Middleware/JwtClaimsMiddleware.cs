using Microsoft.AspNetCore.Http;
using NeoNovaAPIAdmin.Helpers;
using System.Threading.Tasks;


namespace NeoNovaAPIAdmin.Helpers.Middleware
{
    public class JwtClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, JwtExtractorHelper jwtExtractorHelper)
        {
            var claims = jwtExtractorHelper.GetClaimsFromJwt();
            if (claims != null)
            {
                var expiryClaim = claims.FindFirst("exp");
                if (expiryClaim != null)
                {
                    var expiryUnixTime = long.Parse(expiryClaim.Value);
                    var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(expiryUnixTime);
                    if (DateTimeOffset.UtcNow > expiryDateTime)
                    {
                        // Token is expired, clear it
                        context.Response.Cookies.Delete("NeoWebAppCookie");
                        context.Items["UserClaims"] = null;
                    }
                    else
                    {
                        context.Items["UserClaims"] = claims;
                    }
                }
            }
            await _next(context);
        }

    }
}

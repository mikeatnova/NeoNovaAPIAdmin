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
                context.Items["UserClaims"] = claims;
            }
            await _next(context);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NeoNovaAPIAdmin.Helpers;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace NeoNovaAPIAdmin.Controllers
{
    public class CoreController : Controller
    {
        private readonly JwtExtractorHelper _jwtExtractorHelper;

        public CoreController(JwtExtractorHelper jwtExtractorHelper)
        {
            _jwtExtractorHelper = jwtExtractorHelper;
        }

        private void FetchUserRole()
        {
            var claims = _jwtExtractorHelper.GetClaimsFromJwt();
            if (claims != null)
            {
                var roleClaim = claims.FindFirst(ClaimTypes.Role);
                if (roleClaim != null)
                {
                    ViewBag.UserRole = roleClaim.Value;
                }
            }
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool isAuthenticated = IsUserAuthenticated();
            ViewBag.IsUserAuthenticated = isAuthenticated;

            if (!isAuthenticated)
            {
                // Invalidate the cookie here
                HttpContext.Response.Cookies.Delete("NeoWebAppCookie");
            }

            FetchUserRole(); // Fetch user role for view
            base.OnActionExecuting(context);
        }


        private bool IsUserAuthenticated()
        {
            var claims = _jwtExtractorHelper.GetClaimsFromJwt();
            if (claims != null)
            {
                var usernameClaim = claims.FindFirst(ClaimTypes.Name);
                if (usernameClaim != null)
                {
                    ViewBag.UserNameOrEmail = usernameClaim.Value;
                }
                return true;
            }
            return false;
        }

        // Common method to initialize HttpClient with authorization header
        private HttpClient InitializeHttpClient()
        {
            var httpClient = new HttpClient();

            // Retrieve the token from the cookie
            var token = HttpContext.Request.Cookies["NeoWebAppCookie"];

            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return httpClient;
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NeoNovaAPIAdmin.Helpers;
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
            ViewBag.IsUserAuthenticated = IsUserAuthenticated();
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

    }
}

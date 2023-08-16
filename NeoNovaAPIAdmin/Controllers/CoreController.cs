using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NeoNovaAPIAdmin.Helpers;

namespace NeoNovaAPIAdmin.Controllers
{
    public class CoreController : Controller
    {
        private readonly JwtExtractorHelper _jwtExtractorHelper;

        public CoreController(JwtExtractorHelper jwtExtractorHelper)
        {
            _jwtExtractorHelper = jwtExtractorHelper;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.IsUserAuthenticated = IsUserAuthenticated();
            base.OnActionExecuting(context);
        }

        private bool IsUserAuthenticated()
        {
            var claims = _jwtExtractorHelper.GetClaimsFromJwt();
            return claims != null;
        }
    }
}

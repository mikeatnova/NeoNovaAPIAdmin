using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoNovaAPIAdmin.Helpers;

namespace NeoNovaAPIAdmin.Controllers
{
    public class AdminController : CoreController
    {
        public AdminController(JwtExtractorHelper jwtExtractorHelper)
            : base(jwtExtractorHelper) // Call the base constructor with the required parameter
        {
        }

        public IActionResult AdminPortal()
        {
            return View();
        }
    }
}

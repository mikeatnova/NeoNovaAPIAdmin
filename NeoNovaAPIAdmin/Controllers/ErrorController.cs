using Microsoft.AspNetCore.Mvc;

namespace NeoNovaAPIAdmin.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            if (statusCode == 401)
            {
                return View("AccessDenied"); // The view you created for 401 errors
            }

            // Handle other status codes if needed

            return View("Error"); // A general error view
        }
    }

}

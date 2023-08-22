using Microsoft.AspNetCore.Mvc;
using NeoNovaAPIAdmin.Helpers;
using NeoNovaAPIAdmin.Models;
using NeoNovaAPIAdmin.Controllers;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace NeoNovaAPIAdmin.Controllers
{
    public class HomeController : CoreController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, JwtExtractorHelper jwtExtractorHelper)
            : base(jwtExtractorHelper) // Call the base constructor with the required parameter
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
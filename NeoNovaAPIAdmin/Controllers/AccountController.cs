using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using NeoNovaAPIAdmin.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using NeoNovaAPIAdmin.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace NeoNovaAPIAdmin.Controllers
{
    public class AccountController : CoreController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration, JwtExtractorHelper jwtExtractorHelper)
            : base(jwtExtractorHelper) // Call the base constructor with the required parameter
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult AccountPage()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            ViewBag.UserNameOrEmail = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
            return View();
        }

        // LOGIN

        // Load LoginPage
        [AllowAnonymous]
        public IActionResult LoginPage()
        {
            // Check if the user should be authenticated
            bool isAuthenticated = IsUserAuthenticated();

            if (isAuthenticated)
            {
                // Redirect to the Admin Dashboard if the user is authenticated and has a specified role
                if (User.IsInRole("Neo") || User.IsInRole("Admin"))
                {
                    return RedirectToAction("AdminPortal", "Admin");
                }
            }
            else
            {
                // Invalidate the cookie if the user is not authenticated
                HttpContext.Response.Cookies.Delete("NeoWebAppCookie");
            }

            return View();
        }

        // Login Api request
        [AllowAnonymous]
        public async Task<IActionResult> Login(AdminUser adminUser)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("apiClient");
                var response = await httpClient.PostAsJsonAsync("/api/auth/login", adminUser);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    if (result != null && result.Token != null)
                    {
                        var token = result.Token;
                        Response.Cookies.Append("NeoWebAppCookie", token, new CookieOptions { SameSite = SameSiteMode.None, HttpOnly = true, Secure = true });
                    }
                    return RedirectToAction("AdminPortal", "Admin");
                }
                return RedirectToAction("LoginPage", "Account");
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Display a user-friendly message
                TempData["ApiError"] = "Could not connect to the API. Please try again later.";

                return RedirectToAction("LoginPage", "Account");
            }
        }


        // LOGOUT
        [Authorize(Roles = "Neo, Admin")]
        public IActionResult Logout()
        {
            // Delete the authentication cookie
            Response.Cookies.Delete("NeoWebAppCookie");

            // Redirect to the login page
            return RedirectToAction("LoginPage", "Account");
        }
    }
}

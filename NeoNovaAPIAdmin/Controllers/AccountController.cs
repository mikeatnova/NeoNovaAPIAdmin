using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NeoNovaAPIAdmin.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using NeoNovaAPIAdmin.Helpers;
using Microsoft.AspNetCore.Authorization;

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

        [AllowAnonymous]
        public IActionResult LoginPage()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(AdminUser adminUser)
        {
            var httpClient = _httpClientFactory.CreateClient("apiClient");

            var response = await httpClient.PostAsJsonAsync("https://novaapp-2023.azurewebsites.net/api/auth/login", adminUser);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (result != null && result.Token != null)
                {
                    var token = result.Token;

                    Response.Cookies.Append("NeoWebAppCookie", token, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });
                }
            return RedirectToAction("AdminPortal", "Admin");
            }
            return RedirectToAction("LoginPage", "Account");
        }
        [AllowAnonymous]
        public IActionResult Logout()
        {
            // Delete the authentication cookie
            Response.Cookies.Delete("NeoWebAppCookie");

            // Redirect to the login page
            return RedirectToAction("LoginPage", "Account");
        }
    }
}

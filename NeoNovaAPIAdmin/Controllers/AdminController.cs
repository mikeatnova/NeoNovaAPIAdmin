using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeoNovaAPIAdmin.Helpers;
using NeoNovaAPIAdmin.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NeoNovaAPIAdmin.Controllers
{
    public class AdminController : CoreController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(JwtExtractorHelper jwtExtractorHelper, IHttpContextAccessor httpContextAccessor)
            : base(jwtExtractorHelper) // Call the base constructor with the required parameter
        {
            _httpContextAccessor = httpContextAccessor;

        }

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


        private async Task<IActionResult> GetViewAsync<T>(string url)
        {
            using (var httpClient = InitializeHttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    var items = JsonSerializer.Deserialize<List<T>>(content, options);
                    return View(items);
                }
            }
            return View(new List<T>()); // Return an empty list if the request fails
        }

        public IActionResult AdminPortal()
        {
            return View();
        }

        public async Task<IActionResult> ListAllUsers()
        {
            return await GetViewAsync<AllUser>("https://novaapp-2023.azurewebsites.net/api/Auth/get-users");
        }

        [HttpPost]
        public async Task<IActionResult> SeedNewUser(string email, string role)
        {
            using (var httpClient = InitializeHttpClient())
            {
                var seedUserObject = new
                {
                    Email = email,
                    Role = role
                };

                var payload = JsonSerializer.Serialize(seedUserObject);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://novaapp-2023.azurewebsites.net/api/Auth/seed-new-user", content);

                if (response.IsSuccessStatusCode)
                {
                    var tokenHeaderValue = response.Headers.GetValues("Authorization").FirstOrDefault()?.Substring(7);
                    if (tokenHeaderValue != null)
                    {
                        // Store the token in the cookie so the JwtExtractorHelper can use it
                        HttpContext.Response.Cookies.Append("NeoWebAppCookie", tokenHeaderValue);

                        var jwtExtractor = new JwtExtractorHelper(_httpContextAccessor);
                        var generatedPassword = jwtExtractor.ExtractGeneratedPasswordFromJwt(); // No need to pass token here

                        if (!string.IsNullOrEmpty(generatedPassword))
                        {
                            TempData["GeneratedPassword"] = generatedPassword;
                            return RedirectToAction("AdminPortal");
                        }
                    }
                    return View("Error");
                }
                else
                {
                    return View("Error");
                }
            }
        }

        [HttpPost]
        public IActionResult ClearGeneratedPassword()
        {
            TempData.Remove("GeneratedPassword");
            return RedirectToAction("AdminPortal");
        }

    }
}

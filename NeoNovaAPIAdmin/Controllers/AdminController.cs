using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeoNovaAPIAdmin.Helpers;
using NeoNovaAPIAdmin.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NeoNovaAPIAdmin.Controllers
{
    public class AdminController : CoreController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public AdminController(JwtExtractorHelper jwtExtractorHelper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(jwtExtractorHelper) // Call the base constructor with the required parameter
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
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

        [Authorize(Roles = "Neo, Admin")]
        public IActionResult AdminPortal()
        {
            return View();
        }

        [Authorize(Roles = "Neo, Admin")]
        public async Task<IActionResult> ListAllUsers()
        {
            string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
            return await GetViewAsync<AllUser>($"{baseUrl}/api/Auth/get-users");
        }

        [Authorize(Roles = "Neo")]
        [HttpPost]
        public async Task<IActionResult> SeedNewUser(string email, string role, string password)  // Added password parameter
        {
            using (var httpClient = InitializeHttpClient())
            {
                // Prepare the payload
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var seedUserObject = new { Email = email, Role = role, Password = password };  // Added password to payload
                var payload = JsonSerializer.Serialize(seedUserObject);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                // Make the request
                var response = await httpClient.PostAsync($"{baseUrl}/api/Auth/seed-new-user", content);

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    string username = data.username;

                    TempData["GeneratedUsername"] = username;
                    return RedirectToAction("AdminPortal");
                }

                // Redirect to error page for any failures
                return View("Error");
            }
        }
    }
}

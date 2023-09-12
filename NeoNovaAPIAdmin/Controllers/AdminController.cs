using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeoNovaAPIAdmin.Helpers;
using NeoNovaAPIAdmin.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NeoNovaAPIAdmin.Controllers
{
    public class AdminController : CoreController
    {
        public AdminController(JwtExtractorHelper jwtExtractorHelper)
            : base(jwtExtractorHelper) // Call the base constructor with the required parameter
        {
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
            return await GetViewAsync<AllUser>("https://novaapp-2023.azurewebsites.net/api/get-users"); // Replace with your API base URL
        }

    }
}

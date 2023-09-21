using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace NeoNovaAPIAdmin.Controllers.Security
{
    [Authorize(Roles = "Neo, Admin")]
    public class LocationController : CoreController
    {
        private readonly IConfiguration _configuration;

        public LocationController(IConfiguration configuration)
            : base() // Call the base constructor with the required parameter
        {
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

        [HttpGet]
        public async Task<IActionResult> Locations()
        {
            string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
            return await GetViewAsync<Models.SecurityModels.Location>($"{baseUrl}/api/Location");
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(Models.SecurityModels.Location location)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(location);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{baseUrl}/api/Location", content);
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("Locations");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> PutLocation(int id, Models.SecurityModels.Location location)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(location);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{baseUrl}/api/Location/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("Locations");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/Location/{id}");
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("Locations");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace NeoNovaAPIAdmin.Controllers.Security
{
    public class CameraLocationController : CoreController
    {
        private readonly IConfiguration _configuration;

        public CameraLocationController(IConfiguration configuration)
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
        public async Task<IActionResult> CameraLocations()
        {
            string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
            return await GetViewAsync<Models.SecurityModels.CameraLocation>($"{baseUrl}/api/CameraLocation");
        }

        [HttpPost]
        public async Task<IActionResult> AddCameraLocation(Models.SecurityModels.CameraLocation cameraLocation)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(cameraLocation);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{baseUrl}/api/CameraLocation", content);
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("CameraLocation");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> PutCameraLocation(int id, Models.SecurityModels.CameraLocation cameraLocation)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(cameraLocation);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{baseUrl}/api/CameraLocation/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("CameraLocation");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCameraLocation(int id)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/CameraLocation/{id}");
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("CameraLocation");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }
    }
}

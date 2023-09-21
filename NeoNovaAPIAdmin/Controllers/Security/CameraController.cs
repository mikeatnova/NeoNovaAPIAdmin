using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using NeoNovaAPIAdmin.Helpers;
using System.Security.Claims;

namespace NeoNovaAPIAdmin.Controllers.Security
{
    [Authorize(Roles = "Neo, Admin")]
    public class CameraController : CoreController
    {
        private readonly IConfiguration _configuration;
        private readonly JwtExtractorHelper _jwtExtractorHelper;

        public CameraController(IConfiguration configuration, JwtExtractorHelper jwtExtractorHelper)
            : base() // Call the base constructor with the required parameter
        {
            _jwtExtractorHelper = jwtExtractorHelper;
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
        public async Task<IActionResult> Cameras()
        {
            string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
            Dictionary<int, Tuple<string, string>> locationMap = new Dictionary<int, Tuple<string, string>>()
            {
                {1, Tuple.Create("Attleboro", "Massachusetts")},
                {2, Tuple.Create("Framingham", "Massachusetts")},
                {3, Tuple.Create("Sheffield", "Massachusetts")},
                {4, Tuple.Create("Dracut", "Massachusetts")},
                {5, Tuple.Create("Pawtucket", "Rhode Island")},
                {6, Tuple.Create("Central Falls", "Rhode Island")},
                {7, Tuple.Create("Thorndike", "Maine")},
                {8, Tuple.Create("Greenville Junction", "Maine")},
                {9, Tuple.Create("Woodbury", "New Jersey")}
            };
            ViewBag.LocationMap = locationMap;
            return await GetViewAsync<Models.SecurityModels.Camera>($"{baseUrl}/api/Camera");
        }

        [HttpPost]
        public async Task<IActionResult> AddCamera(Models.SecurityModels.Camera camera)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                DateTime currentTime = DateTime.UtcNow;
                camera.CreatedAt = currentTime;
                camera.ModifiedAt = currentTime;

                var claims = _jwtExtractorHelper.GetClaimsFromJwt();
                string username = claims?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                if (!string.IsNullOrWhiteSpace(camera.Notes))
                {
                    string formattedTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    camera.Notes = $"{username} ({formattedTime}): {camera.Notes}";
                }
                var json = JsonSerializer.Serialize(camera);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{baseUrl}/api/Camera", content);
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("Cameras");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> PutCamera(int id, Models.SecurityModels.Camera camera)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                camera.ModifiedAt = DateTime.UtcNow;
                var json = JsonSerializer.Serialize(camera);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{baseUrl}/api/Camera/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("Cameras");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCamera(int id)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/Camera/{id}");
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("Cameras");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }

    }
}

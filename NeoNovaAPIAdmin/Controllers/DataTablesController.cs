using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoNovaAPIAdmin.Helpers;
using NeoNovaAPIAdmin.Models.DbModels;
using System.Text.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.JsonWebTokens;

namespace NeoNovaAPIAdmin.Controllers
{
    [Authorize(Roles = "Neo, Admin")]
    public class DataTablesController : CoreController
    {
        private readonly IConfiguration _configuration;
        private readonly JwtExtractorHelper _jwtExtractorHelper;

        public DataTablesController(JwtExtractorHelper jwtExtractorHelper, IConfiguration configuration)
            : base(jwtExtractorHelper) // Call the base constructor with the required parameter
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

        // FREQUENTLY ASKED QUESTIONS
        [HttpGet]
        public async Task<IActionResult> Faqs()
        {
            string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
            return await GetViewAsync<Models.DbModels.Faq>($"{baseUrl}/api/Faqs");
        }

        // Add Faq
        [HttpPost]
        public async Task<IActionResult> AddFaq(Models.DbModels.Faq faq)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(faq);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{baseUrl}/api/Faqs", content);
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("Faqs");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }

        // Update Faq
        [HttpPost]
        public async Task<IActionResult> PutFaq(int id, Models.DbModels.Faq faq)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(faq);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{baseUrl}/api/Faqs/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("Faqs");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }

        // Delete Faq
        [HttpPost]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/Faqs/{id}");
                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the GET action to refresh the page
                    return RedirectToAction("Faqs");
                }
            }
            // Handle error, e.g., return a view with an error message
            return View("Error");
        }


        // GEOFENCES

        [HttpGet]
        public async Task<IActionResult> Geofences()
        {
            string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
            return await GetViewAsync<Models.DbModels.Geofence>($"{baseUrl}/api/Geofences");
        }

        // Add Geofence
        [HttpPost]
        public async Task<IActionResult> AddGeofence(Models.DbModels.Geofence geofence)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(geofence);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{baseUrl}/api/Geofences", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Geofences");
                }
            }
            return View("Error");
        }

        // Update Geofence
        [HttpPost]
        public async Task<IActionResult> PutGeofence(int id, Models.DbModels.Geofence geofence)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(geofence);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{baseUrl}/api/Geofences/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Geofences");
                }
            }
            return View("Error");
        }

        // Delete Geofence
        [HttpPost]
        public async Task<IActionResult> DeleteGeofence(int id)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/Geofences/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Geofences");
                }
            }
            return View("Error");
        }


        // NOVADECKS

        [HttpGet]
        public async Task<IActionResult> Novadecks()
        {
            string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
            return await GetViewAsync<Models.DbModels.Novadeck>($"{baseUrl}/api/Novadecks");
        }

        // Add Novadeck
        [HttpPost]
        public async Task<IActionResult> AddNovadeck(Models.DbModels.Novadeck novadeck)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(novadeck);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{baseUrl}/api/Novadecks", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Novadecks"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }

        // Update Novadeck
        [HttpPost]
        public async Task<IActionResult> PutNovadeck(int id, Models.DbModels.Novadeck novadeck)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(novadeck);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{baseUrl}/api/Novadecks/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Novadecks"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }

        // Delete Novadeck
        [HttpPost]
        public async Task<IActionResult> DeleteNovadeck(int id)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/Novadecks/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Novadecks"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }


        // STORES

        [HttpGet]
        public async Task<IActionResult> Stores()
        {
            string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
            return await GetViewAsync<Models.DbModels.Store>($"{baseUrl}/api/Stores");
        }

        // Add Store
        [HttpPost]
        public async Task<IActionResult> AddStore(Models.DbModels.Store store)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(store);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{baseUrl}/api/Stores", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Stores"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }

        // Update Store
        [HttpPost]
        public async Task<IActionResult> PutStore(int id, Models.DbModels.Store store)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(store);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{baseUrl}/api/Stores/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Stores"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }

        // Delete Store
        [HttpPost]
        public async Task<IActionResult> DeleteStore(int id)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/Stores/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Stores"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }


        // WHOLESALE BUG MESSAGES

        [HttpGet]
        public async Task<IActionResult> WholesaleBugMessages()
        {
            string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
            return await GetViewAsync<Models.DbModels.WholesaleBugMessage>($"{baseUrl}/api/WholesaleBugMessages");
        }

        // Add WholesaleBugMessage
        [HttpPost]
        public async Task<IActionResult> AddWholesaleBugMessage(Models.DbModels.WholesaleBugMessage wholesaleBugMessage)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(wholesaleBugMessage);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{baseUrl}/api/WholesaleBugMessages", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("WholesaleBugMessages");
                }
            }
            return View("Error");
        }

        // Update WholesaleBugMessage
        [HttpPost]
        public async Task<IActionResult> PutWholesaleBugMessage(int id, Models.DbModels.WholesaleBugMessage wholesaleBugMessage)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var json = JsonSerializer.Serialize(wholesaleBugMessage);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{baseUrl}/api/WholesaleBugMessages/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("WholesaleBugMessages");
                }
            }
            return View("Error");
        }

        // Delete WholesaleBugMessage
        [HttpPost]
        public async Task<IActionResult> DeleteWholesaleBugMessage(int id)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/WholesaleBugMessages/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("WholesaleBugMessages");
                }
            }
            return View("Error");
        }
    }
}

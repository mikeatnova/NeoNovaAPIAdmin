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
    public class DataTablesController : CoreController
    {
        private readonly JwtExtractorHelper _jwtExtractorHelper;

        public DataTablesController(JwtExtractorHelper jwtExtractorHelper)
            : base(jwtExtractorHelper) // Call the base constructor with the required parameter
        {
        }


        // Common method to initialize HttpClient with authorization header
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

        // FREQUENTLY ASKED QUESTIONS

        [HttpGet]
        public async Task<IActionResult> Faqs()
        {
            return await GetViewAsync<Models.DbModels.Faq>("https://novaapp-2023.azurewebsites.net/api/Faqs");
        }

        // Add Faq
        [HttpPost]
        public async Task<IActionResult> AddFaq(Models.DbModels.Faq faq)
        {
            using (var httpClient = InitializeHttpClient())
            {
                var json = JsonSerializer.Serialize(faq);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://novaapp-2023.azurewebsites.net/api/Faqs", content);
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
                var json = JsonSerializer.Serialize(faq);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"https://novaapp-2023.azurewebsites.net/api/Faqs/{id}", content);
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
                var response = await httpClient.DeleteAsync($"https://novaapp-2023.azurewebsites.net/api/Faqs/{id}");
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
            return await GetViewAsync<Models.DbModels.Geofence>("https://novaapp-2023.azurewebsites.net/api/Geofences");
        }

        // Add Geofence
        [HttpPost]
        public async Task<IActionResult> AddGeofence(Models.DbModels.Geofence geofence)
        {
            using (var httpClient = InitializeHttpClient())
            {
                var json = JsonSerializer.Serialize(geofence);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://novaapp-2023.azurewebsites.net/api/Geofences", content);
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
                var json = JsonSerializer.Serialize(geofence);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"https://novaapp-2023.azurewebsites.net/api/Geofences/{id}", content);
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
                var response = await httpClient.DeleteAsync($"https://novaapp-2023.azurewebsites.net/api/Geofences/{id}");
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
            return await GetViewAsync<Models.DbModels.Novadeck>("https://novaapp-2023.azurewebsites.net/api/Novadecks");
        }

        // Add Novadeck
        [HttpPost]
        public async Task<IActionResult> AddNovadeck(Models.DbModels.Novadeck novadeck)
        {
            using (var httpClient = InitializeHttpClient())
            {
                var json = JsonSerializer.Serialize(novadeck);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://novaapp-2023.azurewebsites.net/api/Novadecks", content);
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
                var json = JsonSerializer.Serialize(novadeck);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"https://novaapp-2023.azurewebsites.net/api/Novadecks/{id}", content);
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
                var response = await httpClient.DeleteAsync($"https://novaapp-2023.azurewebsites.net/api/Novadecks/{id}");
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
            return await GetViewAsync<Models.DbModels.Store>("https://novaapp-2023.azurewebsites.net/api/Stores");
        }

        // Add Store
        [HttpPost]
        public async Task<IActionResult> AddStore(Models.DbModels.Store store)
        {
            using (var httpClient = InitializeHttpClient())
            {
                var json = JsonSerializer.Serialize(store);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://novaapp-2023.azurewebsites.net/api/Stores", content);
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
                var json = JsonSerializer.Serialize(store);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"https://novaapp-2023.azurewebsites.net/api/Stores/{id}", content);
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
                var response = await httpClient.DeleteAsync($"https://novaapp-2023.azurewebsites.net/api/Stores/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Stores"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }

        // STORE HOURS

        [HttpGet]
        public async Task<IActionResult> StoreHours()
        {
            return await GetViewAsync<Models.DbModels.StoreHour>("https://novaapp-2023.azurewebsites.net/api/StoreHours");
        }

        // Add Store Hours
        [HttpPost]
        public async Task<IActionResult> AddStoreHour(Models.DbModels.StoreHour storeHour)
        {
            using (var httpClient = InitializeHttpClient())
            {
                var json = JsonSerializer.Serialize(storeHour);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://novaapp-2023.azurewebsites.net/api/StoreHours", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("StoreHours"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }

        // Update Store Hours
        [HttpPost]
        public async Task<IActionResult> PutStoreHour(int id, Models.DbModels.StoreHour storeHour)
        {
            using (var httpClient = InitializeHttpClient())
            {
                var json = JsonSerializer.Serialize(storeHour);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"https://novaapp-2023.azurewebsites.net/api/StoreHours/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("StoreHours"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }

        // Delete Store Hours
        [HttpPost]
        public async Task<IActionResult> DeleteStoreHour(int id)
        {
            using (var httpClient = InitializeHttpClient())
            {
                var response = await httpClient.DeleteAsync($"https://novaapp-2023.azurewebsites.net/api/StoreHours/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("StoreHours"); // Redirect to the GET action to refresh the page
                }
            }
            return View("Error"); // Handle error, e.g., return a view with an error message
        }


    }
}

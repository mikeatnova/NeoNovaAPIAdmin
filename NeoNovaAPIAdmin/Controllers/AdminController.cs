using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeoNovaAPIAdmin.Helpers;
using NeoNovaAPIAdmin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public AdminController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base() // Call the base constructor with the required parameter
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

        [Authorize(Roles = "Neo, Admin")]
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromForm] string userId)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");

                // Make the DELETE request
                var response = await httpClient.DeleteAsync($"{baseUrl}/api/Auth/delete-user/{userId}");

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    TempData["StatusType"] = "success";
                    TempData["StatusMessage"] = "User deleted successfully.";
                    return RedirectToAction("ListAllUsers");
                }
                else
                {
                    TempData["StatusType"] = "danger";
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    TempData["StatusMessage"] = "Failed to delete user: " + errorResponse;
                    return RedirectToAction("ListAllUsers");
                }
            }
        }


        [Authorize(Roles = "Neo, Admin")]
        [HttpPost("ResetUserPassword")]
        public async Task<IActionResult> ResetUserPassword([FromForm] string userId, [FromForm] string currentPassword, [FromForm] string newPassword, [FromForm] string retypeNewPassword)
        {
            using (var httpClient = InitializeHttpClient())
            {
                string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");

                // Step 1: Validate the current password against the external API
                var validatePasswordModel = new { UserId = userId, Password = currentPassword };
                var validatePayload = JsonSerializer.Serialize(validatePasswordModel);
                var validateContent = new StringContent(validatePayload, Encoding.UTF8, "application/json");

                var validationResponse = await httpClient.PostAsync($"{baseUrl}/api/Auth/{userId}/validate-password", validateContent);

                if (validationResponse.IsSuccessStatusCode)
                {
                    var validationContent = await validationResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Validation Content: {validationContent}");
                    var validationJson = JsonSerializer.Deserialize<Dictionary<string, bool>>(validationContent);

                    if (validationJson.TryGetValue("isValid", out bool isValidPassword) && !isValidPassword)
                    {
                        TempData["StatusType"] = "danger";
                        TempData["StatusMessage"] = "Current password is incorrect";
                        return RedirectToAction("AccountPage", "Account");
                    }
                }
                else
                {
                    TempData["StatusType"] = "danger";
                    TempData["StatusMessage"] = "Failed to validate the current password";
                    return RedirectToAction("AccountPage", "Account");
                }

                // Step 2: Check if the new password and retyped password match
                if (newPassword != retypeNewPassword)
                {
                    TempData["StatusType"] = "danger";
                    TempData["StatusMessage"] = "The new password and the retyped password must match";
                    return RedirectToAction("AccountPage", "Account");
                }

                // Step 3: Proceed with the password reset
                var resetPasswordObject = new { UserId = userId, NewPassword = newPassword };
                var resetPayload = JsonSerializer.Serialize(resetPasswordObject);
                var resetContent = new StringContent(resetPayload, Encoding.UTF8, "application/json");

                var resetResponse = await httpClient.PostAsync($"{baseUrl}/api/Auth/reset-password", resetContent);

                if (resetResponse.IsSuccessStatusCode)
                {
                    TempData["StatusType"] = "success";
                    TempData["StatusMessage"] = "Password reset successfully.";
                    return RedirectToAction("AccountPage", "Account");
                }
                else
                {
                    TempData["StatusType"] = "danger";
                    string errorResponse = await resetResponse.Content.ReadAsStringAsync();
                    TempData["StatusMessage"] = "Failed to reset password: " + errorResponse;
                    return RedirectToAction("AccountPage", "Account");
                }
            }
        }


        [Authorize(Roles = "Neo, Admin")]
        [HttpPost("ResetUserUsername")]
        public async Task<IActionResult> ResetUserUsername([FromForm] string userId, [FromForm] string oldUsername, [FromForm] string newUsername)
        {
            // Validate old username
            if (ViewBag.Username != null && ViewBag.Username.Equals(oldUsername))
            {
                using (var httpClient = InitializeHttpClient())
                {
                    string baseUrl = _configuration.GetValue<string>("NeoNovaApiBaseUrl");
                    var resetUsernameObject = new { UserId = userId, NewUsername = newUsername };
                    var payload = JsonSerializer.Serialize(resetUsernameObject);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync($"{baseUrl}/api/Auth/reset-username", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                        if (result != null && !string.IsNullOrEmpty(result.Token))
                        {
                            HttpContext.Response.Cookies.Append("NeoWebAppCookie", result.Token);
                            TempData["StatusType"] = "success";
                            TempData["StatusMessage"] = "Username reset successfully.";
                        }
                        else
                        {
                            TempData["StatusType"] = "danger";
                            TempData["StatusMessage"] = "Failed to reset username: Token is missing.";
                        }

                        return RedirectToAction("AccountPage", "Account");
                    }

                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        TempData["StatusType"] = "danger";
                        TempData["StatusMessage"] = "Failed to reset username: " + errorResponse;
                        return RedirectToAction("AccountPage", "Account");
                    }
                }
            }
            else
            {
                TempData["StatusType"] = "danger";
                TempData["StatusMessage"] = "Old Username is incorrect";
                return RedirectToAction("AccountPage", "Account");
            }
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

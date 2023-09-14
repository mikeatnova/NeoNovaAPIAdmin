﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NeoNovaAPIAdmin.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace NeoNovaAPIAdmin.Controllers
{
    public class CoreController : Controller
    {
        private readonly JwtExtractorHelper _jwtExtractorHelper;

        public CoreController(JwtExtractorHelper jwtExtractorHelper)
        {
            _jwtExtractorHelper = jwtExtractorHelper;
        }

        private void FetchUserStats()
        {
            var claims = _jwtExtractorHelper.GetClaimsFromJwt();
            if (claims != null)
            {
                ViewBag.Role = claims.FindFirst(ClaimTypes.Role)?.Value;
                ViewBag.Username = claims.FindFirst(ClaimTypes.Name)?.Value;
                ViewBag.Email = claims.FindFirst(ClaimTypes.Email)?.Value;
                ViewBag.UserId = claims.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool isAuthenticated = IsUserAuthenticated();
            ViewBag.IsUserAuthenticated = isAuthenticated;

            if (!isAuthenticated)
            {
                // Invalidate the cookie here
                HttpContext.Response.Cookies.Delete("NeoWebAppCookie");
            }

            FetchUserStats(); // Fetch user role for view
            base.OnActionExecuting(context);
        }


        public bool IsUserAuthenticated()
        {
            var claims = _jwtExtractorHelper.GetClaimsFromJwt();
            if (claims != null)
            {
                var usernameClaim = claims.FindFirst(ClaimTypes.Name);
                if (usernameClaim != null)
                {
                    ViewBag.UserNameOrEmail = usernameClaim.Value;
                }
                return true;
            }
            return false;
        }

        // Common method to initialize HttpClient with authorization header
        public HttpClient InitializeHttpClient()
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

    }
}

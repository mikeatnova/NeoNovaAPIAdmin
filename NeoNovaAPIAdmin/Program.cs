using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using NeoNovaAPIAdmin.Helpers;
using NeoNovaAPIAdmin.Helpers.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JwtExtractorHelper>();



builder.Services.AddHttpClient("apiClient", client =>
{
    string? baseAddress = builder.Configuration["NeoNovaApiBaseUrl"];
    if (baseAddress != null)
    {
        client.BaseAddress = new Uri(baseAddress);
    }
    else
    {
        // Handle the case where the base address is not configured
        // log a warning or throw an exception
    }
});

// Get JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");

// Ensure JWT Key is not null
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["NeoWebAppCookie"];
            return Task.CompletedTask;
        }
    };
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => 
{
     options.LoginPath = "/Account/LoginPage"; 
});

builder.Services.AddAuthorization(options =>
{

});


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/LoginPage"; // Redirect to login page if not authenticated
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/Error/{0}");


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtClaimsMiddleware>();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=LoginPage}/{id?}");

app.Run();

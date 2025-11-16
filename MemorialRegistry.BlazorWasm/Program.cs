/*
 * BLAZOR WEBASSEMBLY PROGRAM.CS
 * ==============================
 * Clean, professional deployment-ready version.
 */

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MemorialRegistry.BlazorWasm;
using MemorialRegistry.BlazorWasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

/* 
========================================
 1. API CONFIGURATION (CLEAN VERSION)
========================================
 RULES:
 - Development uses Aspire URL when available
 - Otherwise uses appsettings.json
 - Production uses appsettings.Production.json 
 - No hardcoded URLs in code
*/

// Read the base API URL from config files
// (appsettings.json / appsettings.Production.json)
var apiBaseAddress =
    builder.Configuration["ApiBaseAddress"]
    ?? "https://localhost:7001"; // fallback for dev console runs

Console.WriteLine($"🔗 Configured API Base Address: {apiBaseAddress}");

/* 
========================================
 2. ASPIRE INTEGRATION (DEV ONLY)
========================================
 Aspire automatically injects service URLs using:
   services:{service-name}:{protocol}:{index}

 If Aspire is running, we override the base URL.
*/

var aspireHttps = builder.Configuration["services:memorial-api:https:0"];
var aspireHttp = builder.Configuration["services:memorial-api:http:0"];
var aspireEndpoint = aspireHttps ?? aspireHttp;

if (!string.IsNullOrEmpty(aspireEndpoint))
{
    apiBaseAddress = aspireEndpoint;
    Console.WriteLine($"🚀 Aspire endpoint detected. Using: {apiBaseAddress}");
}

/* 
========================================
 3. HTTP CLIENT REGISTRATION
========================================
 SINGLE PLACE where HttpClient is configured.
*/

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

/*
========================================
 4. APP SERVICES
========================================
*/
builder.Services.AddScoped<ObituaryApiService>();
builder.Services.AddScoped<AuthService>();

/*
========================================
 5. AUTHENTICATION
========================================
*/
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// START APP
await builder.Build().RunAsync();

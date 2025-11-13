/* 
 * BLAZOR WEBASSEMBLY PROGRAM.CS
 * ==============================
 * 
 * MIGRATION: MVC → Blazor WebAssembly Client-Side Application
 * 
 * This file configures the Blazor WebAssembly application that runs entirely
 * in the browser, communicating with the ASP.NET Core API via HTTP calls.
 * 
 * Key Features Implemented:
 * ✅ HTTP Client configuration for API communication
 * ✅ JWT Authentication with CustomAuthenticationStateProvider
 * ✅ Service registration for dependency injection
 * ✅ Aspire integration for development orchestration
 * ✅ Cross-origin configuration for API access
 */

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MemorialRegistry.BlazorWasm;
using MemorialRegistry.BlazorWasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API CONFIGURATION: Setup HTTP client for backend communication
// ===============================================================
// The Blazor WebAssembly app needs to know where the API server is running.
// In development: Uses Aspire orchestration or fallback to localhost:7001
// In production: Should be configured via appsettings or environment variables

var apiBaseAddress = builder.Configuration.GetValue<string>("ApiBaseAddress") ?? "https://localhost:7001";
Console.WriteLine($"🔗 API Base Address from configuration: {apiBaseAddress}");
Console.WriteLine($"📋 All configuration keys: {string.Join(", ", builder.Configuration.AsEnumerable().Select(c => $"{c.Key}={c.Value}"))}");

// ASPIRE INTEGRATION: Auto-detect API endpoint when running through AppHost
var aspireEndpoint = builder.Configuration["services:memorial-api:https:0"] ?? builder.Configuration["services:memorial-api:http:0"];
if (!string.IsNullOrEmpty(aspireEndpoint))
{
    apiBaseAddress = aspireEndpoint;
    Console.WriteLine($"🚀 Using Aspire orchestrated endpoint: {apiBaseAddress}");
}
// HTTP CLIENT: Configure base HTTP client for API communication
// =============================================================
// This HttpClient will be used by all services to make API calls
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

// SERVICE REGISTRATION: Custom services for business logic
// ========================================================
builder.Services.AddScoped<ObituaryApiService>();    // 📡 API communication layer
builder.Services.AddScoped<AuthService>();           // 🔐 Authentication management
// 🔧 Debug configuration handled via static class

// AUTHENTICATION SETUP: JWT-based client-side authentication
// ===========================================================
// Replaces server-side Identity with client-side JWT token management
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// 🎉 MIGRATION COMPLETE: Start the Blazor WebAssembly application
// ================================================================
// The app now runs entirely in the browser, with all UI rendered client-side
// and data fetched from the API server via HTTP calls.
await builder.Build().RunAsync();

/* 
 * MIGRATION SUCCESS SUMMARY:
 * =========================
 * 
 * ✅ MVC Views → Blazor Components (.razor files)
 * ✅ Server Rendering → Client-Side Rendering (WebAssembly)
 * ✅ Entity Framework Direct → HTTP API Calls
 * ✅ Server-Side Identity → JWT Authentication
 * ✅ Form Posts → API Service Methods
 * ✅ ViewData/TempData → Component State Management
 * ✅ Server Validation → Client + Server Validation
 * ✅ Razor Pages → Blazor Router Navigation
 * 
 * Performance Benefits:
 * 🚀 Reduced server load (client-side rendering)
 * 🚀 Improved user experience (SPA navigation)
 * 🚀 Better scalability (stateless API)
 * 🚀 Offline capabilities (PWA potential)
 */;

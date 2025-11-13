/* 
 * ASPIRE APPHOST CONFIGURATION
 * ============================
 * 
 * 🚀 ORCHESTRATION: Manages both API and Blazor WebAssembly services
 * 
 * This AppHost coordinates the development environment by:
 * ✅ Starting the ASP.NET Core API server (port 7001)
 * ✅ Starting the Blazor WebAssembly dev server (port 5180) 
 * ✅ Configuring service-to-service communication
 * ✅ Managing service dependencies and startup order
 * ✅ Providing unified logging and monitoring
 * 
 * Migration Impact:
 * - Replaces manual service startup with coordinated orchestration
 * - Ensures consistent development environment across team
 * - Simplifies debugging with centralized dashboard
 * - Manages CORS and networking configuration automatically
 */

using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// 📡 BACKEND API: ASP.NET Core with Entity Framework, Identity & JWT
// Serves: /api/obituary endpoints, authentication, database operations
var apiService = builder.AddProject<Projects.Assignment1>("memorial-api")
    .WithHttpsEndpoint(port: 7001, name: "api-https");

// 🖥️ FRONTEND SPA: Blazor WebAssembly Client Application
// Serves: Static files, client-side routing, component rendering
var blazorApp = builder.AddProject<Projects.MemorialRegistry_BlazorWasm>("memorial-blazor")
    .WithHttpEndpoint(port: 5180, name: "blazor-http")  
    .WithEnvironment("ApiBaseAddress", "https://localhost:7001")  // 🔗 API connection
    .WaitFor(apiService);  // ⏳ Ensure API starts first

builder.Build().Run();
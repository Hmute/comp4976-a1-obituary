using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// ===============================
// BACKEND SERVICE
// ===============================
var obituaryApi = builder.AddProject<Projects.Assignment1>("obituary-api")
    .WithHttpEndpoint(name: "obituary-api-http")   // Let Aspire assign ports dynamically
    .WithHttpsEndpoint(name: "obituary-api-https");

// ===============================
// FRONTEND SERVICE
// ===============================
builder.AddProject<Projects.Assignment1>("obituary-ui")
    .WithHttpEndpoint(name: "obituary-ui-http")    // Also dynamic
    .WithHttpsEndpoint(name: "obituary-ui-https")
    // Let frontend access the backend’s URL dynamically through Aspire
    .WithEnvironment("BACKEND_URL", obituaryApi.GetEndpoint("obituary-api-http"))
    // Wait until backend is ready before starting frontend
    .WaitFor(obituaryApi)
    // Automatically propagate OpenTelemetry config (via ServiceDefaults)
    .WithReference(obituaryApi);

builder.Build().Run();
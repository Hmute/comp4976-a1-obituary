using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// ===============================
// BACKEND SERVICE CONFIGURATION
// ===============================
// Creates the main web API service that handles all obituary operations
// Uses .NET Aspire for orchestration and service discovery
var obituaryApi = builder.AddProject<Projects.Assignment1>("obituary-api")
    .WithHttpEndpoint(name: "obituary-api-http")   // Let Aspire assign ports dynamically for flexibility
    .WithHttpsEndpoint(name: "obituary-api-https"); // HTTPS endpoint for secure communication

// ===============================
// FRONTEND SERVICE CONFIGURATION
// ===============================
// Memorial Registry web application with enhanced UI and authorization
// Configured for seamless integration with backend service through Aspire service discovery
builder.AddProject<Projects.Assignment1>("obituary-ui")
    .WithHttpEndpoint(name: "obituary-ui-http")    // Dynamic port assignment for development flexibility
    .WithHttpsEndpoint(name: "obituary-ui-https")  // HTTPS for secure authentication and data transmission
                                                   // Enables frontend to discover backend URL dynamically through Aspire service mesh
    .WithEnvironment("BACKEND_URL", obituaryApi.GetEndpoint("obituary-api-http"))
    // Ensures backend service starts before frontend to prevent connection issues
    .WaitFor(obituaryApi)
    // Creates service-to-service reference for distributed tracing and monitoring
    // Automatically propagates OpenTelemetry config (via ServiceDefaults)
    .WithReference(obituaryApi);

// Start the distributed application with orchestrated services
builder.Build().Run();
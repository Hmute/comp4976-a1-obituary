using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Assignment1.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Assignment1.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// DATABASE
// ==========================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ==========================
// JWT AUTHENTICATION
// ==========================
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? "your-super-secret-jwt-key-that-is-at-least-32-characters-long!";

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MemorialRegistry";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MemorialRegistry";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
})
.AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ==========================
// CORS
// ==========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins(
            "https://localhost:5180",                               // Local Blazor WASM
            "https://red-dune-0446b1110.6.azurestaticapps.net"      // Azure SWA
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// ==========================
// OPENAI OPTIONS + HTTP CLIENT
// ==========================
var openAiKey =
    builder.Configuration["AzureOpenAI:ApiKey"] ??
    Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

if (string.IsNullOrEmpty(openAiKey))
{
    throw new Exception("ERROR: Azure OpenAI API Key is missing.");
}

// register strongly typed config
builder.Services.AddSingleton(new AzureOpenAIOptions
{
    Endpoint = builder.Configuration["AzureOpenAI:Endpoint"]!,
    ApiKey = openAiKey,
    ApiVersion = builder.Configuration["AzureOpenAI:ApiVersion"]!,
    Model = builder.Configuration["AzureOpenAI:Model"]!,
    MaxTokens = int.Parse(builder.Configuration["AzureOpenAI:MaxTokens"] ?? "40000")
});

// configure HttpClient with auth header
builder.Services.AddHttpClient<AzureOpenAIService>((sp, client) =>
{
    var opts = sp.GetRequiredService<AzureOpenAIOptions>();
    client.DefaultRequestHeaders.Add("api-key", opts.ApiKey);
});

// ==========================
// MVC + SWAGGER
// ==========================
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Aspire defaults
builder.AddServiceDefaults();

var app = builder.Build();

// ==========================
// PIPELINE
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Fix HTTPS redirect for Azure App Service
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    });
}

app.UseHttpsRedirection();

app.UseCors("BlazorClient");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Obituary}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapIdentityApi<IdentityUser>();
app.MapRazorPages().WithStaticAssets();

// RUN MIGRATIONS IN PRODUCTION
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations...");
        db.Database.Migrate();
        logger.LogInformation("Database migrations completed successfully.");

        // Ensure database is created and accessible
        var canConnect = db.Database.CanConnect();
        logger.LogInformation($"Database connection test: {(canConnect ? "SUCCESS" : "FAILED")}");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while applying database migrations.");
    // Don't throw - let the app start even if migrations fail
}

app.Run();

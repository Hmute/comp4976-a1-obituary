using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Assignment1.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// JWT Configuration
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-super-secret-jwt-key-that-is-at-least-32-characters-long!";
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
.AddBearerToken(IdentityConstants.BearerScheme); // Keep existing bearer token for MVC

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()  // Add roles support
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 🌐 CORS CONFIGURATION: Enable Cross-Origin Requests for Blazor WebAssembly
// ============================================================================
// 📡 MIGRATION: Added CORS support for client-side Blazor app
// 
// Previously: Server-side rendering didn't need CORS (same origin)
// Now: Blazor WebAssembly runs on different port, requires CORS policy
// 
// Security: Restricted to localhost in development, should be configured
//           for specific domains in production
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorWasmPolicy", policy =>
    {
        // 🌐 AZURE PRODUCTION: Static Web App domain
        policy.WithOrigins("https://red-dune-0446b1110.6.azurestaticapps.net")
              .AllowAnyMethod()     // ✅ GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader()     // ✅ Authorization, Content-Type, etc.
              .AllowCredentials();  // ✅ JWT tokens, authentication cookies

        // 🛡️ DEVELOPMENT: Allow any localhost origin (Aspire assigns dynamic ports)
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllersWithViews();

// UI ENHANCEMENT: Add QuickGrid services for modern data grid functionality
// Provides pagination, sorting, and enhanced styling capabilities for obituary listings
builder.Services.AddQuickGridEntityFrameworkAdapter();

// Add HttpClient and Azure OpenAI Service
builder.Services.AddHttpClient<Assignment1.Services.AzureOpenAIService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable CORS for all environments (not just development) to support Blazor WASM
app.UseCors("BlazorWasmPolicy");

app.UseRouting();

// IMPORTANT: Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// ROUTING CHANGE: Changed default controller from Home to Obituary
// This ensures the application starts with the main obituary listing page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Obituary}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapIdentityApi<IdentityUser>();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

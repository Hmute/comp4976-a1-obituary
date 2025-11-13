using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Assignment1.Data;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()  // Add roles support
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// UI ENHANCEMENT: Add QuickGrid services for modern data grid functionality
// Provides pagination, sorting, and enhanced styling capabilities for obituary listings
builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.UseRouting();

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

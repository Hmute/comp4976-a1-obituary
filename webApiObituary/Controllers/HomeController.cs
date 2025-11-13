using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Assignment1.Models;

namespace Assignment1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // AUTHORIZATION CHANGE: Redirect Home/Index to Obituary/Index instead of trying to load non-existent view
    // ROUTING CHANGE: Redirect Home controller actions to Obituary controller
    // This prevents "View not found" errors and ensures users see the main obituary listing
    public IActionResult Index()
    {
        // Direct users to the main obituary listing page instead of a missing Home view
        return RedirectToAction("Index", "Obituary");
    }

    // AUTHORIZATION CHANGE: Redirect Home/Privacy to Obituary/Index instead of trying to load non-existent view
    // ROUTING CHANGE: Redirect Privacy action to main obituary page
    // Maintains consistency by keeping all navigation within the obituary system
    public IActionResult Privacy()
    {
        // Redirect privacy requests to obituary listing to maintain app flow
        return RedirectToAction("Index", "Obituary");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

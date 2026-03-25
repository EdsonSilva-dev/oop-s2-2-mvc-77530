using FoodSafety.Domain.Enums;
using FoodSafety.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafety.MVC.Controllers;

[Authorize(Roles = "Admin,Inspector,Viewer")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? town, RiskRating? riskRating)
    {
        _logger.LogInformation(
            "Dashboard accessed with filters Town={Town}, RiskRating={RiskRating}",
            town,
            riskRating);

        var model = await _dashboardService.GetDashboardAsync(town, riskRating);
        return View(model);
    }
}
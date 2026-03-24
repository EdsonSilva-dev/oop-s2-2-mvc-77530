using FoodSafety.Domain.Enums;
using FoodSafety.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafety.MVC.Controllers;

[Authorize(Roles = "Admin,Inspector,Viewer")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index(string? town, RiskRating? riskRating)
    {
        var model = await _dashboardService.GetDashboardAsync(town, riskRating);
        return View(model);
    }
}
using FoodSafety.MVC.Services.Interfaces;
using FoodSafety.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafety.MVC.Controllers;

[Authorize]
public class PremisesController : Controller
{
    private readonly IPremisesService _premisesService;
    private readonly ILogger<PremisesController> _logger;

    public PremisesController(
        IPremisesService premisesService,
        ILogger<PremisesController> logger)
    {
        _premisesService = premisesService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public async Task<IActionResult> Index(string? searchTerm, string? town)
    {
        var model = await _premisesService.GetAllAsync(searchTerm, town);
        ViewBag.SearchTerm = searchTerm;
        ViewBag.Town = town;

        return View(model);
    }

    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public async Task<IActionResult> Details(int id)
    {
        var model = await _premisesService.GetDetailsAsync(id);

        if (model == null)
        {
            _logger.LogWarning("Premises not found for Details. Id={PremisesId}", id);
            return NotFound();
        }

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var model = await _premisesService.GetCreateModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(PremisesCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model while creating premises.");
            return View(model);
        }

        await _premisesService.CreateAsync(model);

        _logger.LogInformation(
            "Premises created: Name={Name}, Town={Town}, RiskRating={RiskRating}",
            model.Name,
            model.Town,
            model.RiskRating);

        TempData["SuccessMessage"] = "Premises created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _premisesService.GetEditModelAsync(id);

        if (model == null)
        {
            _logger.LogWarning("Premises not found for Edit. Id={PremisesId}", id);
            return NotFound();
        }

        return View(model);
    }

    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public async Task<IActionResult> History(int id, int monthsBack = 1)
    {
        var model = await _premisesService.GetHistoryAsync(id, monthsBack);

        if (model == null)
        {
            _logger.LogWarning("Premises not found for History. Id={PremisesId}", id);
            return NotFound();
        }

        _logger.LogInformation(
            "Premises history viewed. Id={PremisesId}, MonthsBack={MonthsBack}",
            id,
            monthsBack);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(PremisesCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model while editing premises Id={PremisesId}", model.Id);
            return View(model);
        }

        var updated = await _premisesService.UpdateAsync(model);

        if (!updated)
        {
            _logger.LogWarning("Premises not found during update Id={PremisesId}", model.Id);
            TempData["ErrorMessage"] = "Premises not found.";
            return NotFound();
        }

        _logger.LogInformation(
            "Premises updated: Id={Id}, Name={Name}, Town={Town}, RiskRating={RiskRating}",
            model.Id,
            model.Name,
            model.Town,
            model.RiskRating);

        TempData["SuccessMessage"] = "Premises updated successfully.";
        return RedirectToAction(nameof(Index));
    }
}
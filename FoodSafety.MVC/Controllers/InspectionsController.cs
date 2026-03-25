using FoodSafety.MVC.Services.Interfaces;
using FoodSafety.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafety.MVC.Controllers;

[Authorize]
public class InspectionsController : Controller
{
    private readonly IInspectionService _inspectionService;
    private readonly ILogger<InspectionsController> _logger;

    public InspectionsController(
        IInspectionService inspectionService,
        ILogger<InspectionsController> logger)
    {
        _inspectionService = inspectionService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public async Task<IActionResult> Index()
    {
        var inspections = await _inspectionService.GetAllAsync();
        return View(inspections);
    }

    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Create(int premisesId)
    {
        var model = await _inspectionService.GetCreateModelAsync(premisesId);

        if (model == null)
        {
            _logger.LogWarning("Premises not found for creating inspection. PremisesId={PremisesId}", premisesId);
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Create(InspectionCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid inspection model. PremisesId={PremisesId}", model.PremisesId);
            return View(model);
        }

        var created = await _inspectionService.CreateAsync(model);

        if (!created)
        {
            _logger.LogWarning("Inspection creation failed. PremisesId={PremisesId}", model.PremisesId);
            TempData["ErrorMessage"] = "Unable to create inspection.";
            return NotFound();
        }

        _logger.LogInformation(
            "Inspection created: PremisesId={PremisesId}, Score={Score}, Date={Date}",
            model.PremisesId,
            model.Score,
            model.InspectionDate);

        TempData["SuccessMessage"] = "Inspection created successfully.";
        return RedirectToAction(nameof(Index));
    }
}
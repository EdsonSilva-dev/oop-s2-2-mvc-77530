using FoodSafety.MVC.Services.Interfaces;
using FoodSafety.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafety.MVC.Controllers;

[Authorize]
public class FollowUpsController : Controller
{
    private readonly IFollowUpService _followUpService;
    private readonly ILogger<FollowUpsController> _logger;

    public FollowUpsController(
        IFollowUpService followUpService,
        ILogger<FollowUpsController> logger)
    {
        _followUpService = followUpService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public async Task<IActionResult> Index()
    {
        var followUps = await _followUpService.GetAllAsync();
        return View(followUps);
    }

    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Create(int inspectionId)
    {
        var model = await _followUpService.GetCreateModelAsync(inspectionId);

        if (model == null)
        {
            _logger.LogWarning("Inspection not found for follow-up creation. InspectionId={InspectionId}", inspectionId);
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Create(FollowUpCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid follow-up model. InspectionId={InspectionId}", model.InspectionId);
            return View(model);
        }

        var result = await _followUpService.CreateAsync(model);

        if (!result.Success)
        {
            _logger.LogWarning(
                "Follow-up creation failed. InspectionId={InspectionId}, Reason={Reason}",
                model.InspectionId,
                result.ErrorMessage);

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        _logger.LogInformation(
            "Follow-up created: InspectionId={InspectionId}, DueDate={DueDate}",
            model.InspectionId,
            model.DueDate);

        TempData["SuccessMessage"] = "Follow-up created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Close(int id)
    {
        var model = await _followUpService.GetCloseModelAsync(id);

        if (model == null)
        {
            _logger.LogWarning("Follow-up not found for closing. Id={FollowUpId}", id);
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Close(FollowUpCloseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid close follow-up model. Id={FollowUpId}", model.FollowUpId);
            return View(model);
        }

        var result = await _followUpService.CloseAsync(model);

        if (!result.Success)
        {
            _logger.LogWarning(
                "Follow-up close failed. Id={FollowUpId}, Reason={Reason}",
                model.FollowUpId,
                result.ErrorMessage);

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        _logger.LogInformation(
            "Follow-up closed: Id={FollowUpId}, ClosedDate={ClosedDate}",
            model.FollowUpId,
            model.ClosedDate);

        TempData["SuccessMessage"] = "Follow-up closed successfully.";
        return RedirectToAction(nameof(Index));
    }
}
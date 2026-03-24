using FoodSafety.MVC.Services.Interfaces;
using FoodSafety.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafety.MVC.Controllers;

[Authorize]
public class FollowUpsController : Controller
{
    private readonly IFollowUpService _followUpService;

    public FollowUpsController(IFollowUpService followUpService)
    {
        _followUpService = followUpService;
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
            return View(model);
        }

        var result = await _followUpService.CreateAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Inspector")]
    public async Task<IActionResult> Close(int id)
    {
        var model = await _followUpService.GetCloseModelAsync(id);

        if (model == null)
        {
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
            return View(model);
        }

        var result = await _followUpService.CloseAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }
}
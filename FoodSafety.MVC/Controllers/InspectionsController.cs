using FoodSafety.Domain.Models;
using FoodSafety.MVC.Data;
using FoodSafety.MVC.Services.Interfaces;
using FoodSafety.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodSafety.MVC.Controllers
{
    [Authorize]
    public class InspectionsController : Controller
    {
        private readonly IInspectionService _inspectionService;

        public InspectionsController(IInspectionService inspectionService)
        {
            _inspectionService = inspectionService;
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
                return View(model);
            }

            var created = await _inspectionService.CreateAsync(model);

            if (!created)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using FoodSafety.Domain.Enums;
using FoodSafety.Domain.Models;
using FoodSafety.MVC.Data;
using FoodSafety.MVC.Services.Interfaces;
using FoodSafety.MVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodSafety.MVC.Services;

public class InspectionService : IInspectionService
{
    private readonly ApplicationDbContext _context;

    public InspectionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InspectionListItemViewModel>> GetAllAsync()
    {
        return await _context.Inspections
            .Include(i => i.Premises)
            .OrderByDescending(i => i.InspectionDate)
            .Select(i => new InspectionListItemViewModel
            {
                Id = i.Id,
                PremisesId = i.PremisesId,
                PremisesName = i.Premises != null ? i.Premises.Name : string.Empty,
                Town = i.Premises != null ? i.Premises.Town : string.Empty,
                InspectionDate = i.InspectionDate,
                Score = i.Score,
                Outcome = i.Outcome,
                Notes = i.Notes
            })
            .ToListAsync();
    }

    public async Task<InspectionCreateViewModel?> GetCreateModelAsync(int premisesId)
    {
        var premisesExists = await _context.Premises.AnyAsync(p => p.Id == premisesId);

        if (!premisesExists)
        {
            return null;
        }

        return new InspectionCreateViewModel
        {
            PremisesId = premisesId,
            InspectionDate = DateTime.Today
        };
    }

    public async Task<bool> CreateAsync(InspectionCreateViewModel model)
    {
        var premisesExists = await _context.Premises.AnyAsync(p => p.Id == model.PremisesId);

        if (!premisesExists)
        {
            return false;
        }

        var inspection = new Inspection
        {
            PremisesId = model.PremisesId,
            InspectionDate = model.InspectionDate,
            Score = model.Score,
            Outcome = CalculateOutcome(model.Score),
            Notes = model.Notes
        };

        _context.Inspections.Add(inspection);
        await _context.SaveChangesAsync();

        return true;
    }

    private static InspectionOutcome CalculateOutcome(int score)
    {
        return score >= 60
            ? InspectionOutcome.Pass
            : InspectionOutcome.Fail;
    }
}
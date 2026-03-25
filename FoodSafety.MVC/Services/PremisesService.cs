using FoodSafety.Domain.Enums;
using FoodSafety.Domain.Models;
using FoodSafety.MVC.Data;
using FoodSafety.MVC.Services.Interfaces;
using FoodSafety.MVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodSafety.MVC.Services;

public class PremisesService : IPremisesService
{
    private readonly ApplicationDbContext _context;

    public PremisesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PremisesListItemViewModel>> GetAllAsync(string? searchTerm, string? town)
    {
        var query = _context.Premises
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(town))
        {
            query = query.Where(p => p.Town == town);
        }

        return await query
            .OrderBy(p => p.Name)
            .Select(p => new PremisesListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                Town = p.Town,
                RiskRating = p.RiskRating,
                TotalInspections = p.Inspections.Count,
                OpenFollowUps = p.Inspections
                    .SelectMany(i => i.FollowUps)
                    .Count(f => f.Status == FollowUpStatus.Open)
            })
            .ToListAsync();
    }

    public async Task<PremisesDetailsViewModel?> GetDetailsAsync(int id)
    {
        var today = DateTime.Today;

        var premises = await _context.Premises
            .Include(p => p.Inspections)
                .ThenInclude(i => i.FollowUps)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (premises == null)
        {
            return null;
        }

        var inspectionsOrdered = premises.Inspections
            .OrderByDescending(i => i.InspectionDate)
            .ToList();

        var followUps = inspectionsOrdered
            .SelectMany(i => i.FollowUps.Select(f => new { Inspection = i, FollowUp = f }))
            .OrderByDescending(x => x.FollowUp.DueDate)
            .ToList();

        var latestInspection = inspectionsOrdered.FirstOrDefault();

        return new PremisesDetailsViewModel
        {
            Id = premises.Id,
            Name = premises.Name,
            Address = premises.Address,
            Town = premises.Town,
            RiskRating = premises.RiskRating,

            TotalInspections = premises.Inspections.Count,
            FailedInspections = premises.Inspections.Count(i => i.Outcome == InspectionOutcome.Fail),
            OpenFollowUps = followUps.Count(x => x.FollowUp.Status == FollowUpStatus.Open),
            OverdueFollowUps = followUps.Count(x => x.FollowUp.Status == FollowUpStatus.Open && x.FollowUp.DueDate.Date < today),

            LatestInspectionDate = latestInspection?.InspectionDate,
            LatestInspectionScore = latestInspection?.Score,
            LatestInspectionOutcome = latestInspection?.Outcome.ToString(),

            InspectionHistory = inspectionsOrdered
                .Select(i => new PremisesInspectionHistoryItemViewModel
                {
                    InspectionId = i.Id,
                    InspectionDate = i.InspectionDate,
                    Score = i.Score,
                    Outcome = i.Outcome,
                    Notes = i.Notes
                })
                .ToList(),

            FollowUpHistory = followUps
                .Select(x => new PremisesFollowUpHistoryItemViewModel
                {
                    FollowUpId = x.FollowUp.Id,
                    InspectionId = x.Inspection.Id,
                    InspectionDate = x.Inspection.InspectionDate,
                    DueDate = x.FollowUp.DueDate,
                    Status = x.FollowUp.Status,
                    ClosedDate = x.FollowUp.ClosedDate,
                    IsOverdue = x.FollowUp.Status == FollowUpStatus.Open && x.FollowUp.DueDate.Date < today
                })
                .ToList()
        };
    }

    public Task<PremisesCreateEditViewModel> GetCreateModelAsync()
    {
        return Task.FromResult(new PremisesCreateEditViewModel());
    }

    public async Task<bool> CreateAsync(PremisesCreateEditViewModel model)
    {
        var premises = new Premises
        {
            Name = model.Name,
            Address = model.Address,
            Town = model.Town,
            RiskRating = model.RiskRating
        };

        _context.Premises.Add(premises);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<PremisesCreateEditViewModel?> GetEditModelAsync(int id)
    {
        var premises = await _context.Premises.FirstOrDefaultAsync(p => p.Id == id);

        if (premises == null)
        {
            return null;
        }

        return new PremisesCreateEditViewModel
        {
            Id = premises.Id,
            Name = premises.Name,
            Address = premises.Address,
            Town = premises.Town,
            RiskRating = premises.RiskRating
        };
    }

    public async Task<bool> UpdateAsync(PremisesCreateEditViewModel model)
    {
        var premises = await _context.Premises.FirstOrDefaultAsync(p => p.Id == model.Id);

        if (premises == null)
        {
            return false;
        }

        premises.Name = model.Name;
        premises.Address = model.Address;
        premises.Town = model.Town;
        premises.RiskRating = model.RiskRating;

        await _context.SaveChangesAsync();

        return true;
    }
}
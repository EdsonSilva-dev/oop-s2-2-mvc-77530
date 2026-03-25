using FoodSafety.Domain.Enums;
using FoodSafety.MVC.Data;
using FoodSafety.MVC.Services.Interfaces;
using FoodSafety.MVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodSafety.MVC.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(string? town, RiskRating? riskRating)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var nextMonth = startOfMonth.AddMonths(1);

        var premisesQuery = _context.Premises.AsQueryable();

        if (!string.IsNullOrWhiteSpace(town))
        {
            premisesQuery = premisesQuery.Where(p => p.Town == town);
        }

        if (riskRating.HasValue)
        {
            premisesQuery = premisesQuery.Where(p => p.RiskRating == riskRating.Value);
        }

        var filteredPremisesIds = await premisesQuery
            .Select(p => p.Id)
            .ToListAsync();

        var inspectionsThisMonth = await _context.Inspections
            .Where(i => filteredPremisesIds.Contains(i.PremisesId))
            .Where(i => i.InspectionDate >= startOfMonth && i.InspectionDate < nextMonth)
            .CountAsync();

        var failedInspectionsThisMonth = await _context.Inspections
            .Where(i => filteredPremisesIds.Contains(i.PremisesId))
            .Where(i => i.InspectionDate >= startOfMonth && i.InspectionDate < nextMonth)
            .Where(i => i.Outcome == InspectionOutcome.Fail)
            .CountAsync();

        var overdueOpenFollowUps = await _context.FollowUps
            .Include(f => f.Inspection)
            .Where(f => f.Status == FollowUpStatus.Open)
            .Where(f => f.DueDate < now)
            .Where(f => f.Inspection != null && filteredPremisesIds.Contains(f.Inspection.PremisesId))
            .CountAsync();

        var recentInspections = await _context.Inspections
            .Include(i => i.Premises)
            .Where(i => filteredPremisesIds.Contains(i.PremisesId))
            .OrderByDescending(i => i.InspectionDate)
            .Take(5)
            .Select(i => new RecentInspectionItemViewModel
            {
                InspectionId = i.Id,
                PremisesName = i.Premises != null ? i.Premises.Name : string.Empty,
                Town = i.Premises != null ? i.Premises.Town : string.Empty,
                InspectionDate = i.InspectionDate,
                Score = i.Score,
                Outcome = i.Outcome
            })
            .ToListAsync();

              var overdueFollowUpsData = await _context.FollowUps
     .Include(f => f.Inspection)
         .ThenInclude(i => i!.Premises)
     .Where(f => f.Status == FollowUpStatus.Open)
     .Where(f => f.DueDate < now)
     .Where(f => f.Inspection != null && filteredPremisesIds.Contains(f.Inspection.PremisesId))
     .OrderBy(f => f.DueDate)
     .Take(5)
     .ToListAsync();

        var overdueFollowUps = overdueFollowUpsData
            .Select(f => new OverdueFollowUpItemViewModel
            {
                FollowUpId = f.Id,
                InspectionId = f.InspectionId,
                PremisesName = f.Inspection?.Premises?.Name ?? string.Empty,
                Town = f.Inspection?.Premises?.Town ?? string.Empty,
                DueDate = f.DueDate,
                DaysOverdue = (now.Date - f.DueDate.Date).Days
            })
            .ToList();

        var availableTowns = await _context.Premises
            .Select(p => p.Town)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        return new DashboardViewModel
        {
            InspectionsThisMonth = inspectionsThisMonth,
            FailedInspectionsThisMonth = failedInspectionsThisMonth,
            OverdueOpenFollowUps = overdueOpenFollowUps,
            SelectedTown = town,
            SelectedRiskRating = riskRating,
            AvailableTowns = availableTowns,
            AvailableRiskRatings = Enum.GetValues<RiskRating>().ToList(),
            RecentInspections = recentInspections,
            OverdueFollowUps = overdueFollowUps
        };
    }
}

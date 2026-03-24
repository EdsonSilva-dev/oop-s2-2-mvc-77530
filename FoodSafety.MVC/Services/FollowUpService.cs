using FoodSafety.Domain.Enums;
using FoodSafety.Domain.Models;
using FoodSafety.MVC.Data;
using FoodSafety.MVC.Services.Interfaces;
using FoodSafety.MVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodSafety.MVC.Services;

public class FollowUpService : IFollowUpService
{
    private readonly ApplicationDbContext _context;

    public FollowUpService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FollowUpListItemViewModel>> GetAllAsync()
    {
        var today = DateTime.Today;

        return await _context.FollowUps
            .Include(f => f.Inspection)
                .ThenInclude(i => i!.Premises)
            .OrderBy(f => f.Status)
            .ThenBy(f => f.DueDate)
            .Select(f => new FollowUpListItemViewModel
            {
                Id = f.Id,
                InspectionId = f.InspectionId,
                PremisesName = f.Inspection != null && f.Inspection.Premises != null ? f.Inspection.Premises.Name : string.Empty,
                Town = f.Inspection != null && f.Inspection.Premises != null ? f.Inspection.Premises.Town : string.Empty,
                InspectionDate = f.Inspection != null ? f.Inspection.InspectionDate : DateTime.MinValue,
                DueDate = f.DueDate,
                Status = f.Status,
                ClosedDate = f.ClosedDate,
                IsOverdue = f.Status == FollowUpStatus.Open && f.DueDate < today
            })
            .ToListAsync();
    }

    public async Task<FollowUpCreateViewModel?> GetCreateModelAsync(int inspectionId)
    {
        var inspection = await _context.Inspections
            .Include(i => i.Premises)
            .FirstOrDefaultAsync(i => i.Id == inspectionId);

        if (inspection == null || inspection.Premises == null)
        {
            return null;
        }

        return new FollowUpCreateViewModel
        {
            InspectionId = inspection.Id,
            PremisesId = inspection.PremisesId,
            PremisesName = inspection.Premises.Name,
            InspectionDate = inspection.InspectionDate,
            DueDate = inspection.InspectionDate.AddDays(7)
        };
    }

    public async Task<(bool Success, string ErrorMessage)> CreateAsync(FollowUpCreateViewModel model)
    {
        var inspection = await _context.Inspections
            .FirstOrDefaultAsync(i => i.Id == model.InspectionId);

        if (inspection == null)
        {
            return (false, "Inspection not found.");
        }

        if (model.DueDate.Date < inspection.InspectionDate.Date)
        {
            return (false, "Due date cannot be earlier than the inspection date.");
        }

        var followUp = new FollowUp
        {
            InspectionId = model.InspectionId,
            DueDate = model.DueDate.Date,
            Status = FollowUpStatus.Open,
            ClosedDate = null
        };

        _context.FollowUps.Add(followUp);
        await _context.SaveChangesAsync();

        return (true, string.Empty);
    }

    public async Task<FollowUpCloseViewModel?> GetCloseModelAsync(int followUpId)
    {
        var followUp = await _context.FollowUps
            .Include(f => f.Inspection)
                .ThenInclude(i => i!.Premises)
            .FirstOrDefaultAsync(f => f.Id == followUpId);

        if (followUp == null || followUp.Inspection == null || followUp.Inspection.Premises == null)
        {
            return null;
        }

        return new FollowUpCloseViewModel
        {
            FollowUpId = followUp.Id,
            InspectionId = followUp.InspectionId,
            PremisesName = followUp.Inspection.Premises.Name,
            InspectionDate = followUp.Inspection.InspectionDate,
            DueDate = followUp.DueDate,
            ClosedDate = DateTime.Today
        };
    }

    public async Task<(bool Success, string ErrorMessage)> CloseAsync(FollowUpCloseViewModel model)
    {
        var followUp = await _context.FollowUps
            .Include(f => f.Inspection)
            .FirstOrDefaultAsync(f => f.Id == model.FollowUpId);

        if (followUp == null || followUp.Inspection == null)
        {
            return (false, "Follow-up not found.");
        }

        if (model.ClosedDate == default)
        {
            return (false, "Closed date is required.");
        }

        if (model.ClosedDate.Date < followUp.Inspection.InspectionDate.Date)
        {
            return (false, "Closed date cannot be earlier than the inspection date.");
        }

        followUp.Status = FollowUpStatus.Closed;
        followUp.ClosedDate = model.ClosedDate.Date;

        await _context.SaveChangesAsync();

        return (true, string.Empty);
    }
}
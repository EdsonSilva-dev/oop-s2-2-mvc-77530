using FluentAssertions;
using FoodSafety.Domain.Enums;
using FoodSafety.Domain.Models;
using FoodSafety.MVC.Services;
using FoodSafety.MVC.ViewModels;
using FoodSafety.Tests.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodSafety.Tests.Services;

public class FollowUpServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Fail_WhenDueDateIsEarlierThanInspectionDate()
    {
        using var context = TestDbContextFactory.Create();

        var premises = new Premises
        {
            Name = "Test Bakery",
            Address = "10 Market Road",
            Town = "Galway",
            RiskRating = RiskRating.High
        };

        context.Premises.Add(premises);
        await context.SaveChangesAsync();

        var inspection = new Inspection
        {
            PremisesId = premises.Id,
            InspectionDate = new DateTime(2026, 3, 20),
            Score = 40,
            Outcome = InspectionOutcome.Fail,
            Notes = "Serious non-compliance"
        };

        context.Inspections.Add(inspection);
        await context.SaveChangesAsync();

        var service = new FollowUpService(context);

        var model = new FollowUpCreateViewModel
        {
            InspectionId = inspection.Id,
            DueDate = new DateTime(2026, 3, 19)
        };

        var result = await service.CreateAsync(model);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Due date cannot be earlier than the inspection date.");
        context.FollowUps.Should().BeEmpty();
    }

    [Fact]
    public async Task CloseAsync_Should_Fail_WhenClosedDateIsEarlierThanInspectionDate()
    {
        using var context = TestDbContextFactory.Create();

        var premises = new Premises
        {
            Name = "Test Grill",
            Address = "88 River Lane",
            Town = "Dublin",
            RiskRating = RiskRating.Medium
        };

        context.Premises.Add(premises);
        await context.SaveChangesAsync();

        var inspection = new Inspection
        {
            PremisesId = premises.Id,
            InspectionDate = new DateTime(2026, 3, 20),
            Score = 50,
            Outcome = InspectionOutcome.Fail
        };

        context.Inspections.Add(inspection);
        await context.SaveChangesAsync();

        var followUp = new FollowUp
        {
            InspectionId = inspection.Id,
            DueDate = new DateTime(2026, 3, 25),
            Status = FollowUpStatus.Open
        };

        context.FollowUps.Add(followUp);
        await context.SaveChangesAsync();

        var service = new FollowUpService(context);

        var model = new FollowUpCloseViewModel
        {
            FollowUpId = followUp.Id,
            ClosedDate = new DateTime(2026, 3, 19)
        };

        var result = await service.CloseAsync(model);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Closed date cannot be earlier than the inspection date.");
    }
}
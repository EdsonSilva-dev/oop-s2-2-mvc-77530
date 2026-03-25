using FluentAssertions;
using FoodSafety.Domain.Enums;
using FoodSafety.Domain.Models;
using FoodSafety.MVC.Services;
using FoodSafety.Tests.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodSafety.Tests.Services;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_Should_ReturnCorrectCounts_ForFilteredTown()
    {
        using var context = TestDbContextFactory.Create();

        var now = DateTime.UtcNow;
        var thisMonthDate = new DateTime(now.Year, now.Month, 10);

        var dublinPremises = new Premises
        {
            Name = "Dublin Cafe",
            Address = "1 Main Street",
            Town = "Dublin",
            RiskRating = RiskRating.High
        };

        var corkPremises = new Premises
        {
            Name = "Cork Bistro",
            Address = "2 Main Street",
            Town = "Cork",
            RiskRating = RiskRating.Low
        };

        context.Premises.AddRange(dublinPremises, corkPremises);
        await context.SaveChangesAsync();

        var dublinInspectionFail = new Inspection
        {
            PremisesId = dublinPremises.Id,
            InspectionDate = thisMonthDate,
            Score = 45,
            Outcome = InspectionOutcome.Fail
        };

        var dublinInspectionPass = new Inspection
        {
            PremisesId = dublinPremises.Id,
            InspectionDate = thisMonthDate.AddDays(-1),
            Score = 80,
            Outcome = InspectionOutcome.Pass
        };

        var corkInspection = new Inspection
        {
            PremisesId = corkPremises.Id,
            InspectionDate = thisMonthDate,
            Score = 90,
            Outcome = InspectionOutcome.Pass
        };

        context.Inspections.AddRange(dublinInspectionFail, dublinInspectionPass, corkInspection);
        await context.SaveChangesAsync();

        var overdueFollowUp = new FollowUp
        {
            InspectionId = dublinInspectionFail.Id,
            DueDate = now.AddDays(-2),
            Status = FollowUpStatus.Open
        };

        context.FollowUps.Add(overdueFollowUp);
        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        var result = await service.GetDashboardAsync("Dublin", null);

        result.InspectionsThisMonth.Should().Be(2);
        result.FailedInspectionsThisMonth.Should().Be(1);
        result.OverdueOpenFollowUps.Should().Be(1);
    }
}
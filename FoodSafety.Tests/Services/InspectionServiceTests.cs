using FluentAssertions;
using FoodSafety.Domain.Enums;
using FoodSafety.Domain.Models;
using FoodSafety.MVC.Services;
using FoodSafety.MVC.ViewModels;
using FoodSafety.Tests.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodSafety.Tests.Services;

public class InspectionServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_SetOutcomeToPass_WhenScoreIs60OrMore()
    {
        using var context = TestDbContextFactory.Create();

        var premises = new Premises
        {
            Name = "Test Cafe",
            Address = "123 Main Street",
            Town = "Dublin",
            RiskRating = RiskRating.Medium
        };

        context.Premises.Add(premises);
        await context.SaveChangesAsync();

        var service = new InspectionService(context);

        var model = new InspectionCreateViewModel
        {
            PremisesId = premises.Id,
            InspectionDate = new DateTime(2026, 3, 20),
            Score = 60,
            Notes = "Borderline pass"
        };

        var result = await service.CreateAsync(model);

        result.Should().BeTrue();

        var inspection = context.Inspections.Single();
        inspection.Outcome.Should().Be(InspectionOutcome.Pass);
    }

    [Fact]
    public async Task CreateAsync_Should_SetOutcomeToFail_WhenScoreIsBelow60()
    {
        using var context = TestDbContextFactory.Create();

        var premises = new Premises
        {
            Name = "Test Deli",
            Address = "45 High Street",
            Town = "Cork",
            RiskRating = RiskRating.High
        };

        context.Premises.Add(premises);
        await context.SaveChangesAsync();

        var service = new InspectionService(context);

        var model = new InspectionCreateViewModel
        {
            PremisesId = premises.Id,
            InspectionDate = new DateTime(2026, 3, 20),
            Score = 59,
            Notes = "Failed hygiene checks"
        };

        var result = await service.CreateAsync(model);

        result.Should().BeTrue();

        var inspection = context.Inspections.Single();
        inspection.Outcome.Should().Be(InspectionOutcome.Fail);
    }
}
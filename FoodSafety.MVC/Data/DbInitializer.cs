using FoodSafety.Domain.Enums;
using FoodSafety.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodSafety.MVC.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
        await SeedDomainDataAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Admin", "Inspector", "Viewer"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
    {
        await CreateUserIfNotExistsAsync(userManager,
            email: "admin@foodsafety.local",
            password: "Admin123!",
            role: "Admin");

        await CreateUserIfNotExistsAsync(userManager,
            email: "inspector@foodsafety.local",
            password: "Inspector123!",
            role: "Inspector");

        await CreateUserIfNotExistsAsync(userManager,
            email: "viewer@foodsafety.local",
            password: "Viewer123!",
            role: "Viewer");
    }

    private static async Task CreateUserIfNotExistsAsync(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
            return;

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }

    private static async Task SeedDomainDataAsync(ApplicationDbContext context)
    {
        if (await context.Premises.AnyAsync())
            return;

        var premisesList = new List<Premises>
        {
            new() { Name = "Green Fork Cafe", Address = "12 River Street", Town = "Dublin", RiskRating = RiskRating.Low },
            new() { Name = "Oak Deli", Address = "45 Market Lane", Town = "Dublin", RiskRating = RiskRating.Medium },
            new() { Name = "Sunrise Bakery", Address = "8 Main Road", Town = "Dublin", RiskRating = RiskRating.High },
            new() { Name = "Harbour Bistro", Address = "101 Dock Street", Town = "Dublin", RiskRating = RiskRating.Medium },

            new() { Name = "Cork Fresh Foods", Address = "22 St Patrick Street", Town = "Cork", RiskRating = RiskRating.Low },
            new() { Name = "Lee Valley Grill", Address = "9 Bridge Avenue", Town = "Cork", RiskRating = RiskRating.High },
            new() { Name = "City Spice Kitchen", Address = "17 Oliver Place", Town = "Cork", RiskRating = RiskRating.Medium },
            new() { Name = "Atlantic Bites", Address = "60 Coast Road", Town = "Cork", RiskRating = RiskRating.High },

            new() { Name = "Galway Bay Eats", Address = "4 Quay Street", Town = "Galway", RiskRating = RiskRating.Low },
            new() { Name = "West End Diner", Address = "33 Shop Lane", Town = "Galway", RiskRating = RiskRating.Medium },
            new() { Name = "Claddagh Kitchen", Address = "70 Seaview Road", Town = "Galway", RiskRating = RiskRating.High },
            new() { Name = "Stone Mill Cafe", Address = "19 Abbey Street", Town = "Galway", RiskRating = RiskRating.Medium }
        };

        await context.Premises.AddRangeAsync(premisesList);
        await context.SaveChangesAsync();

        var premises = await context.Premises.OrderBy(p => p.Id).ToListAsync();

        var inspections = new List<Inspection>
        {
            new() { PremisesId = premises[0].Id, InspectionDate = DateTime.UtcNow.AddDays(-10), Score = 82, Outcome = InspectionOutcome.Pass, Notes = "Clean kitchen and good storage." },
            new() { PremisesId = premises[0].Id, InspectionDate = DateTime.UtcNow.AddMonths(-3), Score = 75, Outcome = InspectionOutcome.Pass, Notes = "Minor labeling issue." },

            new() { PremisesId = premises[1].Id, InspectionDate = DateTime.UtcNow.AddDays(-8), Score = 58, Outcome = InspectionOutcome.Fail, Notes = "Temperature log incomplete." },
            new() { PremisesId = premises[1].Id, InspectionDate = DateTime.UtcNow.AddMonths(-2), Score = 61, Outcome = InspectionOutcome.Pass, Notes = "Improved after prior warning." },

            new() { PremisesId = premises[2].Id, InspectionDate = DateTime.UtcNow.AddDays(-5), Score = 49, Outcome = InspectionOutcome.Fail, Notes = "Cross-contamination risk observed." },
            new() { PremisesId = premises[2].Id, InspectionDate = DateTime.UtcNow.AddMonths(-4), Score = 55, Outcome = InspectionOutcome.Fail, Notes = "Repeated hygiene concerns." },

            new() { PremisesId = premises[3].Id, InspectionDate = DateTime.UtcNow.AddDays(-18), Score = 73, Outcome = InspectionOutcome.Pass, Notes = "Acceptable condition overall." },
            new() { PremisesId = premises[3].Id, InspectionDate = DateTime.UtcNow.AddMonths(-6), Score = 64, Outcome = InspectionOutcome.Pass, Notes = "Satisfactory inspection." },

            new() { PremisesId = premises[4].Id, InspectionDate = DateTime.UtcNow.AddDays(-12), Score = 90, Outcome = InspectionOutcome.Pass, Notes = "Very strong compliance." },
            new() { PremisesId = premises[4].Id, InspectionDate = DateTime.UtcNow.AddMonths(-5), Score = 87, Outcome = InspectionOutcome.Pass, Notes = "No major issues." },

            new() { PremisesId = premises[5].Id, InspectionDate = DateTime.UtcNow.AddDays(-7), Score = 52, Outcome = InspectionOutcome.Fail, Notes = "Sanitising procedures not followed." },
            new() { PremisesId = premises[5].Id, InspectionDate = DateTime.UtcNow.AddMonths(-3), Score = 66, Outcome = InspectionOutcome.Pass, Notes = "Previous issues resolved." },

            new() { PremisesId = premises[6].Id, InspectionDate = DateTime.UtcNow.AddDays(-15), Score = 78, Outcome = InspectionOutcome.Pass, Notes = "Good operational control." },
            new() { PremisesId = premises[6].Id, InspectionDate = DateTime.UtcNow.AddMonths(-2), Score = 71, Outcome = InspectionOutcome.Pass, Notes = "Minor cleaning note." },

            new() { PremisesId = premises[7].Id, InspectionDate = DateTime.UtcNow.AddDays(-3), Score = 46, Outcome = InspectionOutcome.Fail, Notes = "Food storage breaches found." },
            new() { PremisesId = premises[7].Id, InspectionDate = DateTime.UtcNow.AddMonths(-7), Score = 59, Outcome = InspectionOutcome.Fail, Notes = "Repeat compliance failure." },

            new() { PremisesId = premises[8].Id, InspectionDate = DateTime.UtcNow.AddDays(-20), Score = 84, Outcome = InspectionOutcome.Pass, Notes = "Strong standards maintained." },
            new() { PremisesId = premises[8].Id, InspectionDate = DateTime.UtcNow.AddMonths(-3), Score = 80, Outcome = InspectionOutcome.Pass, Notes = "No significant concerns." },

            new() { PremisesId = premises[9].Id, InspectionDate = DateTime.UtcNow.AddDays(-14), Score = 62, Outcome = InspectionOutcome.Pass, Notes = "Borderline but acceptable." },
            new() { PremisesId = premises[9].Id, InspectionDate = DateTime.UtcNow.AddMonths(-2), Score = 57, Outcome = InspectionOutcome.Fail, Notes = "Cleaning schedule not followed." },

            new() { PremisesId = premises[10].Id, InspectionDate = DateTime.UtcNow.AddDays(-6), Score = 51, Outcome = InspectionOutcome.Fail, Notes = "Pest control documentation missing." },
            new() { PremisesId = premises[10].Id, InspectionDate = DateTime.UtcNow.AddMonths(-4), Score = 60, Outcome = InspectionOutcome.Pass, Notes = "Bare minimum pass." },

            new() { PremisesId = premises[11].Id, InspectionDate = DateTime.UtcNow.AddDays(-9), Score = 77, Outcome = InspectionOutcome.Pass, Notes = "Reasonable controls in place." },
            new() { PremisesId = premises[11].Id, InspectionDate = DateTime.UtcNow.AddMonths(-5), Score = 69, Outcome = InspectionOutcome.Pass, Notes = "Satisfactory." },

            new() { PremisesId = premises[7].Id, InspectionDate = DateTime.UtcNow.AddMonths(-1), Score = 54, Outcome = InspectionOutcome.Fail, Notes = "Follow-up required again." }
        };

        await context.Inspections.AddRangeAsync(inspections);
        await context.SaveChangesAsync();

        var allInspections = await context.Inspections.OrderBy(i => i.Id).ToListAsync();

        var followUps = new List<FollowUp>
        {
            new() { InspectionId = allInspections[2].Id, DueDate = DateTime.UtcNow.AddDays(-2), Status = FollowUpStatus.Open, ClosedDate = null },
            new() { InspectionId = allInspections[4].Id, DueDate = DateTime.UtcNow.AddDays(5), Status = FollowUpStatus.Open, ClosedDate = null },
            new() { InspectionId = allInspections[5].Id, DueDate = DateTime.UtcNow.AddDays(-20), Status = FollowUpStatus.Closed, ClosedDate = DateTime.UtcNow.AddDays(-10) },
            new() { InspectionId = allInspections[10].Id, DueDate = DateTime.UtcNow.AddDays(-1), Status = FollowUpStatus.Open, ClosedDate = null },
            new() { InspectionId = allInspections[14].Id, DueDate = DateTime.UtcNow.AddDays(7), Status = FollowUpStatus.Open, ClosedDate = null },
            new() { InspectionId = allInspections[15].Id, DueDate = DateTime.UtcNow.AddDays(-30), Status = FollowUpStatus.Closed, ClosedDate = DateTime.UtcNow.AddDays(-15) },
            new() { InspectionId = allInspections[19].Id, DueDate = DateTime.UtcNow.AddDays(3), Status = FollowUpStatus.Open, ClosedDate = null },
            new() { InspectionId = allInspections[20].Id, DueDate = DateTime.UtcNow.AddDays(-6), Status = FollowUpStatus.Open, ClosedDate = null },
            new() { InspectionId = allInspections[24].Id, DueDate = DateTime.UtcNow.AddDays(10), Status = FollowUpStatus.Open, ClosedDate = null },
            new() { InspectionId = allInspections[9].Id, DueDate = DateTime.UtcNow.AddDays(-12), Status = FollowUpStatus.Closed, ClosedDate = DateTime.UtcNow.AddDays(-5) }
        };

        await context.FollowUps.AddRangeAsync(followUps);
        await context.SaveChangesAsync();
    }
}
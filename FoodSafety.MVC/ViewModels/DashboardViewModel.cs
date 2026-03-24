using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class DashboardViewModel
{
    public int InspectionsThisMonth { get; set; }
    public int FailedInspectionsThisMonth { get; set; }
    public int OverdueOpenFollowUps { get; set; }

    public string? SelectedTown { get; set; }
    public RiskRating? SelectedRiskRating { get; set; }

    public List<string> AvailableTowns { get; set; } = new();
    public List<RiskRating> AvailableRiskRatings { get; set; } = new();

    public List<RecentInspectionItemViewModel> RecentInspections { get; set; } = new();
    public List<OverdueFollowUpItemViewModel> OverdueFollowUps { get; set; } = new();
}
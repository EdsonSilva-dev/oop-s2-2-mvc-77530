using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class PremisesDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public RiskRating RiskRating { get; set; }

    public int TotalInspections { get; set; }
    public int FailedInspections { get; set; }
    public int OpenFollowUps { get; set; }
    public int OverdueFollowUps { get; set; }

    public DateTime? LatestInspectionDate { get; set; }
    public int? LatestInspectionScore { get; set; }
    public string? LatestInspectionOutcome { get; set; }

    public List<PremisesInspectionHistoryItemViewModel> InspectionHistory { get; set; } = new();
    public List<PremisesFollowUpHistoryItemViewModel> FollowUpHistory { get; set; } = new();
}
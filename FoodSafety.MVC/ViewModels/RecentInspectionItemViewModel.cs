using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class RecentInspectionItemViewModel
{
    public int InspectionId { get; set; }
    public string PremisesName { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public int Score { get; set; }
    public InspectionOutcome Outcome { get; set; }
}

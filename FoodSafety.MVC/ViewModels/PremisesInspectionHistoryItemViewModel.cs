using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class PremisesInspectionHistoryItemViewModel
{
    public int InspectionId { get; set; }
    public DateTime InspectionDate { get; set; }
    public int Score { get; set; }
    public InspectionOutcome Outcome { get; set; }
    public string? Notes { get; set; }
}
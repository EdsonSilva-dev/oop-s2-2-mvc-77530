using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class InspectionListItemViewModel
{
    public int Id { get; set; }
    public int PremisesId { get; set; }
    public string PremisesName { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public int Score { get; set; }
    public InspectionOutcome Outcome { get; set; }
    public string? Notes { get; set; }
}
using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class PremisesHistoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public RiskRating RiskRating { get; set; }

    public int MonthsBack { get; set; }
    public DateTime CutoffDate { get; set; }

    public int TotalHistoricalInspections { get; set; }
    public int TotalHistoricalFails { get; set; }
    public int TotalHistoricalFollowUps { get; set; }
    public int TotalHistoricalClosedFollowUps { get; set; }
    public int TotalHistoricalOpenFollowUps { get; set; }

    public List<PremisesInspectionHistoryItemViewModel> HistoricalInspections { get; set; } = new();
    public List<PremisesFollowUpHistoryItemViewModel> HistoricalFollowUps { get; set; } = new();
}
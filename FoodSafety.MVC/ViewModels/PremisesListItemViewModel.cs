using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class PremisesListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public RiskRating RiskRating { get; set; }

    public int TotalInspections { get; set; }
    public int OpenFollowUps { get; set; }
}
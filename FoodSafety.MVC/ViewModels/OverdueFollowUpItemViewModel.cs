namespace FoodSafety.MVC.ViewModels;

public class OverdueFollowUpItemViewModel
{
    public int FollowUpId { get; set; }
    public int InspectionId { get; set; }
    public string PremisesName { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
}
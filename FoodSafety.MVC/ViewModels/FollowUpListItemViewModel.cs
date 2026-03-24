using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class FollowUpListItemViewModel
{
    public int Id { get; set; }
    public int InspectionId { get; set; }
    public string PremisesName { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public DateTime DueDate { get; set; }
    public FollowUpStatus Status { get; set; }
    public DateTime? ClosedDate { get; set; }
    public bool IsOverdue { get; set; }
}
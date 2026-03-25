using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class PremisesFollowUpHistoryItemViewModel
{
    public int FollowUpId { get; set; }
    public int InspectionId { get; set; }
    public DateTime InspectionDate { get; set; }
    public DateTime DueDate { get; set; }
    public FollowUpStatus Status { get; set; }
    public DateTime? ClosedDate { get; set; }
    public bool IsOverdue { get; set; }
}
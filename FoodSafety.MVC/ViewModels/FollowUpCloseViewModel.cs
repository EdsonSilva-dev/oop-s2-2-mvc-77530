using System.ComponentModel.DataAnnotations;

namespace FoodSafety.MVC.ViewModels;

public class FollowUpCloseViewModel
{
    [Required]
    public int FollowUpId { get; set; }

    public int InspectionId { get; set; }

    public string PremisesName { get; set; } = string.Empty;

    public DateTime InspectionDate { get; set; }

    public DateTime DueDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ClosedDate { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace FoodSafety.MVC.ViewModels;

public class FollowUpCreateViewModel
{
    [Required]
    public int InspectionId { get; set; }

    public int PremisesId { get; set; }

    public string PremisesName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    public DateTime InspectionDate { get; set; }
}
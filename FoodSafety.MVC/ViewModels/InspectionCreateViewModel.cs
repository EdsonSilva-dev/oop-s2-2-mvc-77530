using System.ComponentModel.DataAnnotations;

namespace FoodSafety.MVC.ViewModels;

public class InspectionCreateViewModel
{
    [Required]
    public int PremisesId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime InspectionDate { get; set; }

    [Range(0, 100)]
    public int Score { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

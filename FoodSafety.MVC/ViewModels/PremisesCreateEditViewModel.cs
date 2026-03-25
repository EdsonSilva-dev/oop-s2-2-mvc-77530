using System.ComponentModel.DataAnnotations;
using FoodSafety.Domain.Enums;

namespace FoodSafety.MVC.ViewModels;

public class PremisesCreateEditViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Town { get; set; } = string.Empty;

    [Required]
    public RiskRating RiskRating { get; set; }
}
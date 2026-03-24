using FoodSafety.Domain.Enums;
using FoodSafety.MVC.ViewModels;

namespace FoodSafety.MVC.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(string? town, RiskRating? riskRating);
}
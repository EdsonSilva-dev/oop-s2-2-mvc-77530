using FoodSafety.MVC.ViewModels;

namespace FoodSafety.MVC.Services.Interfaces;

public interface IPremisesService
{
    Task<List<PremisesListItemViewModel>> GetAllAsync(string? searchTerm, string? town);
    Task<PremisesDetailsViewModel?> GetDetailsAsync(int id);
    Task<PremisesHistoryViewModel?> GetHistoryAsync(int id, int monthsBack = 1);
    Task<PremisesCreateEditViewModel> GetCreateModelAsync();
    Task<bool> CreateAsync(PremisesCreateEditViewModel model);
    Task<PremisesCreateEditViewModel?> GetEditModelAsync(int id);
    Task<bool> UpdateAsync(PremisesCreateEditViewModel model);
}
using FoodSafety.MVC.ViewModels;

namespace FoodSafety.MVC.Services.Interfaces;

public interface IInspectionService
{
    Task<List<InspectionListItemViewModel>> GetAllAsync();
    Task<InspectionCreateViewModel?> GetCreateModelAsync(int premisesId);
    Task<bool> CreateAsync(InspectionCreateViewModel model);
}
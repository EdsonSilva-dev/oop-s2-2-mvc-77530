using FoodSafety.MVC.ViewModels;

namespace FoodSafety.MVC.Services.Interfaces;

public interface IFollowUpService
{
    Task<List<FollowUpListItemViewModel>> GetAllAsync();
    Task<FollowUpCreateViewModel?> GetCreateModelAsync(int inspectionId);
    Task<(bool Success, string ErrorMessage)> CreateAsync(FollowUpCreateViewModel model);
    Task<FollowUpCloseViewModel?> GetCloseModelAsync(int followUpId);
    Task<(bool Success, string ErrorMessage)> CloseAsync(FollowUpCloseViewModel model);
}
namespace FoodSafety.MVC.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);
    }
}

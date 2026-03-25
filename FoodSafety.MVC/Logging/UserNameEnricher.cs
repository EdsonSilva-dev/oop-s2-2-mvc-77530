using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace FoodSafety.MVC.Logging;

public class UserNameEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserNameEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var userName = "Anonymous";

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            userName = httpContext.User.Identity?.Name ?? "AuthenticatedUser";
        }

        var property = propertyFactory.CreateProperty("UserName", userName);
        logEvent.AddOrUpdateProperty(property);
    }
}
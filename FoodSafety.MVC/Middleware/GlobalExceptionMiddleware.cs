using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace FoodSafety.MVC.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception occurred. Path={Path}, Method={Method}, TraceIdentifier={TraceIdentifier}",
                context.Request.Path,
                context.Request.Method,
                context.TraceIdentifier);

            await HandleExceptionAsync(context);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context)
    {
        if (IsApiRequest(context.Request))
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = _environment.IsDevelopment()
                    ? "Check server logs for full exception details."
                    : "Please try again later.",
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        context.Response.Redirect("/Home/Error");
    }

    private static bool IsApiRequest(HttpRequest request)
    {
        if (request.Path.StartsWithSegments("/api"))
        {
            return true;
        }

        var acceptHeader = request.Headers.Accept.ToString();
        return acceptHeader.Contains("application/json", StringComparison.OrdinalIgnoreCase);
    }
}
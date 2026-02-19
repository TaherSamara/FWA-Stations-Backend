using Microsoft.AspNetCore.Diagnostics;
using System.Security;
using FWA_Stations.Data;
using FWA_Stations.Entities;

namespace FWA_Stations.Errors;

public class GlobalExceptionHandler(IServiceScopeFactory scopeFactory) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();

        await db.ErrorLogs.AddAsync(new ErrorLogs
        {
            message = exception.Message,
            details = exception.InnerException?.ToString() ?? exception.StackTrace,
            source = "GlobalExceptionHandler",
            operation = httpContext.Request.Method,
            entity_type = "N/A",
            insert_date = DateTime.UtcNow
        }, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        int statusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            SecurityException => StatusCodes.Status403Forbidden,
            ArgumentNullException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            msg = exception.Message,
            details = exception.ToString(),
            error_type = exception.GetType().Name
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}

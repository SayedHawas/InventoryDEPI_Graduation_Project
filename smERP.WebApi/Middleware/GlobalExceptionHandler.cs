using Azure;
using Microsoft.AspNetCore.Diagnostics;
using smERP.Application.Behaviors;
using smERP.SharedKernel.Localizations.Resources;
using System.Security.Claims;

namespace smERP.WebApi.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHttpContextAccessor httpContextAccessor) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        string username = GetCurrentUsername();

        var response = new ApiResult()
        {
            IsSuccess = false,
            StatusCode = 500,
            Message = SharedResourcesKeys.InternalServerError.ToString(),
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private string GetCurrentUsername()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "Anonymous";
    }
}

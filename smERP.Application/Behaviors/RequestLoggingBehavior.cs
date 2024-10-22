using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using smERP.SharedKernel.Responses;
using System.Security.Claims;
using System.Text.Json;

namespace smERP.Application.Behaviors;

public class RequestLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : IResultBase
{
    private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestLoggingBehavior(
        ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        string username = GetCurrentUsername();

        _logger.LogInformation(
            "Processing request {RequestName} for user {Username}", requestName, username);

        TResponse result = await next();

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Completed request {RequestName} for user {Username}", requestName, username);
        }
        else
        {
            string errorDetails = JsonSerializer.Serialize(result.Errors);
            _logger.LogError(
                "Completed request {RequestName} for user {Username} with errors: {Errors}",
                requestName, username, errorDetails);
        }

        return result;
    }

    private string GetCurrentUsername()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "Anonymous";
    }
}


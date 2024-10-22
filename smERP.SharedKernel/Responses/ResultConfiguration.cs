using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Data.SqlTypes;
using ILogger = Serilog.ILogger;

namespace smERP.SharedKernel.Responses;

public static class ResultConfiguration
{
    public static void ResultInitialize(IServiceProvider serviceProvider)
    {
        var logger = new AdvancedSerilogLogger(serviceProvider);
        Result.Setup(cfg =>
        {
            cfg.Logger = logger;
        });
    }

    public class AdvancedSerilogLogger : IResultLogger
    {
        private readonly ILogger _logger;

        public AdvancedSerilogLogger(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

        }

        public void Log(string context, string content, ResultBase result, LogLevel logLevel)
        {
            var serilogLevel = ConvertToSerilogLevel(logLevel);
            var exception = result.Errors.FirstOrDefault(e => e.Metadata.ContainsKey("Exception"))?.Metadata["Exception"] as Exception;

            if (exception != null)
            {
                _logger.Write(serilogLevel, exception, "Result: {Reasons} {Content} <{Context}>",
                    result.Reasons.Select(r => r.Message), content, context);
            }
            else
            {
                _logger.Write(serilogLevel, "Result: {Reasons} {Content} <{Context}>",
                    result.Reasons.Select(r => r.Message), content, context);
            }
        }

        public void Log<TContext>(string content, ResultBase result, LogLevel logLevel)
        {
            Log(typeof(TContext).FullName, content, result, logLevel);
        }

        private LogEventLevel ConvertToSerilogLevel(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.None => LogEventLevel.Verbose,
                LogLevel.Information => LogEventLevel.Information,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                _ => LogEventLevel.Information
            };
        }
    }

    public class UserEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Anonymous";
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Username", username));
        }
    }
}


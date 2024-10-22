using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using smERP.Application.Behaviors;
using System.Reflection;
using smERP.Application.Helpers;
using Microsoft.Extensions.Configuration;
using smERP.Application.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace smERP.Application;

public static class ApplicationDependencies
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        //services.AddSignalRCore();
        services.AddTransient<NotificationHub>();
        services.AddSingleton(new FileEncryptionHelper(configuration));

        return services;
    }
}

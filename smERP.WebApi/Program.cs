using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.Application;
using smERP.Persistence;
using System.Globalization;
using Serilog;
using smERP.WebApi.Middleware;
using smERP.Infrastructure;
using Microsoft.Extensions.FileProviders;
using smERP.Application.Notifications;

namespace smERP.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Host.UseSerilog((context, loggerConfigs) =>
        {
            loggerConfigs.ReadFrom.Configuration(context.Configuration);
        });


        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .Enrich.With()
            .WriteTo.File("log.txt",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
            .CreateLogger();

        builder.Services.AddControllers();

        builder.Services.AddSignalR();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddApplicationDependencies(builder.Configuration)
                        .AddInfrastructureDependencies(builder.Configuration)
                        .AddPersistenceDependencies(builder.Configuration);

        builder.Services.AddLocalization();
        builder.Services.AddLocalizationExtension();

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("ar-EG") };
            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        builder.Services.AddCors();

        var app = builder.Build();

        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        LocalizationExtension.InitializeLocalizer(app.Services);

        //ResultConfiguration.ResultInitialize(app.Services);

        app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

        app.UseHttpsRedirection();

        app.UseSerilogRequestLogging();

        app.UseAuthentication();

        app.UseAuthorization();

        // Add static file middleware
        app.UseStaticFiles(); // This allows serving static files from the wwwroot folder

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "FileStorage")),
            RequestPath = "/FileStorage"
        });


        app.UseCors(x => x
            .SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Location")
            .AllowCredentials());

        app.MapHub<NotificationHub>("/notifications");

        app.MapControllers();

        app.Run();
    }
}

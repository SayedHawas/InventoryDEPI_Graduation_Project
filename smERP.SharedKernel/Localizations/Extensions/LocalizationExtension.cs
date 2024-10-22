using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.SharedKernel.Localizations.Extensions
{
    public static class LocalizationExtension
    {
        private static IStringLocalizer _localizer;

        public static void AddLocalizationExtension(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var factory = sp.GetRequiredService<IStringLocalizerFactory>();
                return factory.Create(typeof(SharedResources));
            });
        }

        public static void InitializeLocalizer(IServiceProvider serviceProvider)
        {
            _localizer = serviceProvider.GetRequiredService<IStringLocalizer>();
        }

        public static string Localize(this string key, params object[] arguments)
        {
            return string.Format(_localizer[key], arguments);
        }
    }
}

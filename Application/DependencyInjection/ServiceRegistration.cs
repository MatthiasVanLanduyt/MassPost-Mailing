using Microsoft.Extensions.DependencyInjection;
using Validator.Application.Addresses;
using Validator.Domain.Addresses;

namespace Validator.Application.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var applicationPath = Path.GetDirectoryName(typeof(ServiceRegistration).Assembly.Location)
            ?? throw new InvalidOperationException("Could not determine application path");

            var postalCodesPath = Path.Combine(applicationPath, "Addresses", "BelgianPostalCodes.json");

            services.AddSingleton<IPostalCodeService>(_ =>
                new PostalCodeService(postalCodesPath));

            // Register other application services

            return services;
        }
    }
}
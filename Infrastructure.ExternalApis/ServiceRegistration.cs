using Core.Application.Services;
using Core.Domain.Interfaces;
using Infrastructure.ExternalApis.ModularServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ExternalApis
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IExchangeProvider>(sp =>
                new FrankfurterService(new HttpClient { BaseAddress = new Uri("https://api.frankfurter.app/") },
                                       sp.GetRequiredService<ILogger<FrankfurterService>>()));

            services.AddScoped<IExchangeProvider>(sp =>
                new FloatratesService(new HttpClient { BaseAddress = new Uri("https://www.floatrates.com/") },
                                      sp.GetRequiredService<ILogger<FloatratesService>>()));

            services.AddScoped<IExchangeService, ExchangeService>();
        }
    }
}

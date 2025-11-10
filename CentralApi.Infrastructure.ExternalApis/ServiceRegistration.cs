using CentralApi.Core.Application.Services;
using CentralApi.Core.Domain.Interfaces;
using CentralApi.Infrastructure.ExternalApis.ModularServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CentralApi.Infrastructure.ExternalApis
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

            services.AddScoped<IExchangeProvider>(sp =>
                new FirstApiService(new HttpClient { BaseAddress = new Uri("http://localhost:5150/") },
                                      sp.GetRequiredService<ILogger<FirstApiService>>()));

            services.AddScoped<IExchangeProvider>(sp =>
                new SecondApiService(new HttpClient { BaseAddress = new Uri("https://localhost:7142/") },
                                      sp.GetRequiredService<ILogger<SecondApiService>>()));

            services.AddScoped<IExchangeProvider>(sp =>
                new ThirdApiService(new HttpClient { BaseAddress = new Uri("http://localhost:5107/") },
                                      sp.GetRequiredService<ILogger<ThirdApiService>>()));

            services.AddScoped<IExchangeService, ExchangeService>();
        }
    }
}

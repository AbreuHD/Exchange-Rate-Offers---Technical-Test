using CentralApi.Core.Application.Services;
using CentralApi.Core.Domain.Interfaces;
using CentralApi.Infrastructure.ExternalApis.ModularServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CentralApi.Infrastructure.ExternalApis
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Get API endpoints from configuration (supports both local and Docker environments)
            var firstApiUrl = configuration["ApiEndpoints:FirstApi"] ?? "http://localhost:5150/";
            var secondApiUrl = configuration["ApiEndpoints:SecondApi"] ?? "https://localhost:7142/";
            var thirdApiUrl = configuration["ApiEndpoints:ThirdApi"] ?? "http://localhost:5107/";

            services.AddScoped<IExchangeProvider>(sp =>
                new FrankfurterService(new HttpClient { BaseAddress = new Uri("https://api.frankfurter.app/") },
                                       sp.GetRequiredService<ILogger<FrankfurterService>>()));

            services.AddScoped<IExchangeProvider>(sp =>
                new FloatratesService(new HttpClient { BaseAddress = new Uri("https://www.floatrates.com/") },
                                      sp.GetRequiredService<ILogger<FloatratesService>>()));

            services.AddScoped<IExchangeProvider>(sp =>
                new FirstApiService(new HttpClient { BaseAddress = new Uri(firstApiUrl) },
                                      sp.GetRequiredService<ILogger<FirstApiService>>()));

            services.AddScoped<IExchangeProvider>(sp =>
                new SecondApiService(new HttpClient { BaseAddress = new Uri(secondApiUrl) },
                                      sp.GetRequiredService<ILogger<SecondApiService>>()));

            services.AddScoped<IExchangeProvider>(sp =>
                new ThirdApiService(new HttpClient { BaseAddress = new Uri(thirdApiUrl) },
                                      sp.GetRequiredService<ILogger<ThirdApiService>>()));

            services.AddScoped<IExchangeService, ExchangeService>();
        }
    }
}

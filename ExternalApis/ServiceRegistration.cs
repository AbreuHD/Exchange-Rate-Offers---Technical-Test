using Core.Application.Services;
using Core.Domain.Interfaces;
using Infrastructure.ExternalApis.ModularServices;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ExternalApis
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddHttpClient<IExchangeProvider, FrankfurterService>(c =>
                c.BaseAddress = new Uri("https://api.frankfurter.app"));

            services.AddScoped<ExchangeService>();
        }
    }
}

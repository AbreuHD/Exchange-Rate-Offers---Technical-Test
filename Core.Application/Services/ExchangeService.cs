using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.Services
{
    public class ExchangeService(IEnumerable<IExchangeProvider> providers)
    {
        private readonly IEnumerable<IExchangeProvider> _providers = providers;

        public async Task<GenericResponse<ExchangeResults?>> GetBestRateAsync(string from, string to, decimal amount)
        {
            var tasks = _providers.Select(p => p.GetExchangeRateAsync(from, to, amount));
            var results = await Task.WhenAll(tasks);

            var response = results
                .Where(r => r?.Payload != null)
                .OrderByDescending(r => r!.Payload!.ConvertedAmount)
                .FirstOrDefault();

            return response ?? new GenericResponse<ExchangeResults?>
            {
                Payload = null,
                Statuscode = 404,
                Message = "No valid exchange rate found."
            };
        }
    }
}
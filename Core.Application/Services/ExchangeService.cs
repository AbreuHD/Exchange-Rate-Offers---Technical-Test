using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.Services
{
    public class ExchangeService(IEnumerable<IExchangeProvider> providers)
    {
        private readonly IEnumerable<IExchangeProvider> _providers = providers;

        public async Task<ExchangeResults?> GetBestRateAsync(string from, string to, decimal amount)
        {
            var tasks = _providers.Select(p => p.GetExchangeRateAsync(from, to, amount));
            var results = await Task.WhenAll(tasks);

            return results
                .Where(r => r != null)
                .OrderByDescending(r => r!.ConvertedAmount)
                .FirstOrDefault();
        }
    }
}
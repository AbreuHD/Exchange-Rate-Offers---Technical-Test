using CentralApi.Core.Domain.Common;
using CentralApi.Core.Domain.Entities;
using CentralApi.Core.Domain.Interfaces;

namespace CentralApi.Core.Application.Services
{
    public class ExchangeService(IEnumerable<IExchangeProvider> providers) : IExchangeService
    {
        private readonly IEnumerable<IExchangeProvider> _providers = providers;

        public async Task<GenericResponse<ExchangeResults?>> GetBestRateAsync(ExchangeRequest request)
        {
            var tasks = _providers.Select(p => p.GetExchangeRateAsync(request.From, request.To, request.Amount));
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

        public async Task<GenericResponse<List<ExchangeResults?>>> GetBetterRatesAsync(List<ExchangeRequest> requests)
        {
            if (requests == null || requests.Count == 0)
            {
                return new GenericResponse<List<ExchangeResults?>>
                {
                    Payload = null,
                    Statuscode = 400,
                    Message = "No exchange requests provided."
                };
            }

            var allResults = new List<ExchangeResults?>();

            var tasks = requests.Select(async request =>
            {
                var providerTasks = _providers.Select(p => p.GetExchangeRateAsync(request.From, request.To, request.Amount));
                var providerResults = await Task.WhenAll(providerTasks);

                var bestRate = providerResults
                    .Where(r => r?.Payload != null)
                    .OrderByDescending(r => r!.Payload!.ConvertedAmount)
                    .FirstOrDefault();

                return bestRate?.Payload;
            });

            var results = await Task.WhenAll(tasks);
            allResults.AddRange(results.Where(r => r != null));

            if (allResults.Count > 0)
            {
                return new GenericResponse<List<ExchangeResults?>>
                {
                    Payload = allResults,
                    Statuscode = 200,
                    Message = "Best rates retrieved successfully."
                };
            }
            else
            {
                return new GenericResponse<List<ExchangeResults?>>
                {
                    Payload = null,
                    Statuscode = 404,
                    Message = "No valid exchange rates found."
                };
            }
        }

        public async Task<GenericResponse<List<ExchangeResults?>>> GetRatesAsync(ExchangeRequest request)
        {
            var tasks = _providers.Select(p => p.GetExchangeRateAsync(request.From, request.To, request.Amount));
            var results = await Task.WhenAll(tasks);

            var response = results
                .Where(r => r?.Payload != null)
                .Select(r => r!.Payload)
                .ToList();

            if (response.Count > 0)
            {
                return new GenericResponse<List<ExchangeResults?>>
                {
                    Payload = response,
                    Statuscode = 200,
                    Message = "Rates retrieved successfully."
                };
            }
            else
            {
                return new GenericResponse<List<ExchangeResults?>>
                {
                    Payload = null,
                    Statuscode = 404,
                    Message = "No valid exchange rate found."
                };
            }
        }
    }
}
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Infrastructure.ExternalApis.ModularServices
{
    public class FrankfurterService(HttpClient httpClient, ILogger<FrankfurterService> logger) : IExchangeProvider
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<FrankfurterService> _logger = logger;

        public async Task<ExchangeResults> GetExchangeRateAsync(string from, string to, decimal amount)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/latest?from={from}&to={to}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("API1 returned non-success status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var data = await response.Content.ReadFromJsonAsync<ExchangeApiResponse>();

                if (data == null || !data.Rates.TryGetValue("EUR", out var rate))
                    return null;

                if (data == null)
                {
                    _logger.LogWarning("API1 returned an invalid JSON structure.");
                    return null;
                }

                return new ExchangeResults
                {
                    ProviderName = "API1",
                    Rate = rate,
                    ConvertedAmount = amount * rate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling API1.");
                return null;
            }
        }
        public class ExchangeApiResponse
        {
            public decimal Amount { get; set; }
            public string Base { get; set; } = string.Empty;
            public string Date { get; set; } = string.Empty;
            public Dictionary<string, decimal> Rates { get; set; } = new();
        }
    }
}

using Core.Domain.Common;
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

        public async Task<GenericResponse<ExchangeResults?>> GetExchangeRateAsync(string from, string to, decimal amount)
        {
            try
            {
                var apiResponse = await _httpClient.GetAsync($"/latest?from={from}&to={to}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("FrankfurterService returned non-success status: {StatusCode}", apiResponse.StatusCode);
                    return new GenericResponse<ExchangeResults?>
                    {
                        Message = "Failed to retrieve exchange rate from FrankfurterService.",
                        Statuscode = (int)apiResponse.StatusCode,
                        Payload = null
                    };
                }

                var data = await apiResponse.Content.ReadFromJsonAsync<ExchangeApiResponse>();

                if (data == null || !data.Rates.TryGetValue(to, out var rate))
                {
                    _logger.LogWarning("FrankfurterService returned an invalid JSON structure or missing rate.");
                    return new GenericResponse<ExchangeResults?>
                    {
                        Message = "Invalid response structure from FrankfurterService.",
                        Statuscode = 500,
                        Payload = null
                    };
                }

                return new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults
                    {
                        ExchangePair = $"{from}/{to}",
                        ProviderName = "FrankfurterService",
                        Rate = rate,
                        ConvertedAmount = amount * rate
                    },
                    Statuscode = 200,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling FrankfurterService.");
                return new GenericResponse<ExchangeResults?>
                {
                    Message = "An error occurred while retrieving exchange rate from FrankfurterService.",
                    Statuscode = 500,
                    Payload = null
                };
            }
        }
        private sealed class ExchangeApiResponse
        {
            public decimal Amount { get; set; }
            public string Base { get; set; } = string.Empty;
            public string Date { get; set; } = string.Empty;
            public Dictionary<string, decimal> Rates { get; set; } = [];
        }
    }
}

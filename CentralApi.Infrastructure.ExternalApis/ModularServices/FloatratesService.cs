using CentralApi.Core.Domain.Common;
using CentralApi.Core.Domain.Entities;
using CentralApi.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CentralApi.Infrastructure.ExternalApis.ModularServices
{
    public class FloatratesService(HttpClient httpClient, ILogger<FloatratesService> logger) : IExchangeProvider
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<FloatratesService> _logger = logger;

        public async Task<GenericResponse<ExchangeResults?>> GetExchangeRateAsync(string from, string to, decimal amount)
        {
            try
            {
                var apiResponse = await _httpClient.GetAsync($"/daily/{from}.json");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("FloatratesService returned non-success status: {StatusCode}", apiResponse.StatusCode);
                    return new GenericResponse<ExchangeResults?>
                    {
                        Message = "Failed to retrieve exchange rate from FloatratesService.",
                        Statuscode = (int)apiResponse.StatusCode,
                        Payload = null
                    };
                }

                var data = await apiResponse.Content.ReadFromJsonAsync<Dictionary<string, ExchangeApiResponse>>();


                if (data == null || !data.TryGetValue(to.ToLower(), out var currency))
                {
                    _logger.LogWarning("FloatratesService returned an invalid JSON structure or missing rate.");
                    return new GenericResponse<ExchangeResults?>
                    {
                        Message = "Invalid response structure from FloatratesService.",
                        Statuscode = 500,
                        Payload = null
                    };
                }

                return new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults
                    {
                        ExchangePair = $"{from}/{to}",
                        ProviderName = "FloatratesService",
                        Rate = currency.Rate,
                        ConvertedAmount = amount * currency.Rate
                    },
                    Statuscode = 200,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling FloatratesService.");
                return new GenericResponse<ExchangeResults?>
                {
                    Message = "An error occurred while retrieving exchange rate from FloatratesService.",
                    Statuscode = 500,
                    Payload = null
                };
            }
        }
        private sealed class ExchangeApiResponse
        {
            public string Code { get; set; }
            public decimal Rate { get; set; }
        }
    }
}

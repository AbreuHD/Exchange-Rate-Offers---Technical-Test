using CentralApi.Core.Domain.Common;
using CentralApi.Core.Domain.Entities;
using CentralApi.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CentralApi.Infrastructure.ExternalApis.ModularServices
{
    public class ThirdApiService(HttpClient httpClient, ILogger<ThirdApiService> logger) : IExchangeProvider
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<ThirdApiService> _logger = logger;

        public async Task<GenericResponse<ExchangeResults?>> GetExchangeRateAsync(string from, string to, decimal amount)
        {
            try
            {
                var apiResponse = await _httpClient.GetAsync($"api/exchange/Change?From={from}&To={to}");

                if (!apiResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("ThirdApiService returned non-success status: {StatusCode}", apiResponse.StatusCode);
                    return new GenericResponse<ExchangeResults?>
                    {
                        Message = "Failed to retrieve exchange rate from ThirdApiService.",
                        Statuscode = (int)apiResponse.StatusCode,
                        Payload = null
                    };
                }

                var data = await apiResponse.Content.ReadFromJsonAsync<ExchangeApiResponse>();

                if (data == null)
                {
                    _logger.LogWarning("ThirdApiService returned an invalid JSON structure or missing rate.");
                    return new GenericResponse<ExchangeResults?>
                    {
                        Message = "Invalid response structure from ThirdApiService.",
                        Statuscode = 500,
                        Payload = null
                    };
                }

                return new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults
                    {
                        ExchangePair = $"{from}/{to}",
                        ProviderName = "ThirdApiService",
                        Rate = data.ExchangeRate,
                        ConvertedAmount = data.ConvertedAmount
                    },
                    Statuscode = 200,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling ThirdApiService.");
                return new GenericResponse<ExchangeResults?>
                {
                    Message = "An error occurred while retrieving exchange rate from ThirdApiService.",
                    Statuscode = 500,
                    Payload = null
                };
            }
        }
        private sealed class ExchangeApiResponse
        {
            public long StatusCode { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public decimal ExchangeRate { get; set; }
            public decimal ConvertedAmount { get; set; }
        }
    }
}

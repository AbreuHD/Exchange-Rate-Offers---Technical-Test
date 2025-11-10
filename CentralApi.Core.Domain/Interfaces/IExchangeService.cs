using CentralApi.Core.Domain.Common;
using CentralApi.Core.Domain.Entities;

namespace CentralApi.Core.Domain.Interfaces
{
    public interface IExchangeService
    {
        Task<GenericResponse<ExchangeResults?>> GetBestRateAsync(ExchangeRequest request);
        Task<GenericResponse<List<ExchangeResults?>>> GetRatesAsync(ExchangeRequest request);
        Task<GenericResponse<List<ExchangeResults?>>> GetBetterRatesAsync(List<ExchangeRequest> requests);
    }
}

using Core.Domain.Common;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces
{
    public interface IExchangeService
    {
        Task<GenericResponse<ExchangeResults?>> GetBestRateAsync(ExchangeRequest request);
        Task<GenericResponse<List<ExchangeResults?>>> GetRatesAsync(ExchangeRequest request);
        Task<GenericResponse<List<ExchangeResults?>>> GetBetterRatesAsync(List<ExchangeRequest> requests);
    }
}

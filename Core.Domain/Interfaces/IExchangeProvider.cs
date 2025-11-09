using Core.Application.Dtos;
using Core.Application.Dtos.Generic;

namespace Core.Domain.Interfaces
{
    public interface IExchangeProvider
    {
        Task<GenericApiResponse<ExanchangeResponse>> GetExchangeRateAsync(string fromCurrency, string toCurrency);
    }
}

using CentralApi.Core.Domain.Common;
using CentralApi.Core.Domain.Entities;

namespace CentralApi.Core.Domain.Interfaces
{
    public interface IExchangeProvider
    {
        Task<GenericResponse<ExchangeResults?>> GetExchangeRateAsync(string from, string to, decimal amount);
    }
}

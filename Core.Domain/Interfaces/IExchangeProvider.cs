using Core.Domain.Common;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces
{
    public interface IExchangeProvider
    {
        Task<GenericResponse<ExchangeResults?>> GetExchangeRateAsync(string from, string to, decimal amount);
    }
}

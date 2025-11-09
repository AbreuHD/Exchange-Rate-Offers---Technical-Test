using Core.Domain.Entities;

namespace Core.Domain.Interfaces
{
    public interface IExchangeProvider
    {
        Task<ExchangeResults> GetExchangeRateAsync(string from, string to, decimal amount);
    }
}

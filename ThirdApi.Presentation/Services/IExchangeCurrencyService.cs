using ThirdApi.Presentation.Model;

namespace ThirdApi.Presentation.Services
{
    public interface IExchangeCurrencyService
    {
        Task<Response?> ExchangeAsync(string from, string to, decimal quantity);
    }
}

namespace FirstApi.Presentation.Services
{
    public interface IExchangeCurrencyService
    {
        Task<decimal?> ExchangeAsync(string from, string to);
    }
}

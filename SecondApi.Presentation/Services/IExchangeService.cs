using SecondApi.Presentation.Model;
using System.ServiceModel;

namespace SecondApi.Presentation.Services
{
    [ServiceContract]
    public interface IExchangeService
    {
        [OperationContract]
        Task<ExchangeResult> GetExchangeRate(string fromCurrency, string toCurrency, decimal amount);
    }
}

using SecondApi.Presentation.Model;
using System.Xml.Linq;

namespace SecondApi.Presentation.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly HashSet<string> _currencyCodes;

        public ExchangeService()
        {
            var doc = XDocument.Load("./IsoCodes.xml");

            _currencyCodes = [.. doc.Descendants("CcyNtry")
                .Select(x => (string?)x.Element("Ccy"))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim().ToUpperInvariant())];
        }

        private bool ValidateCurrency(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return _currencyCodes.Contains(code.ToUpperInvariant());
        }
        public async Task<ExchangeResult> GetExchangeRate(string fromCurrency, string toCurrency, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(fromCurrency) || string.IsNullOrWhiteSpace(toCurrency))
            {
                return new ExchangeResult
                {
                    Message = "Both 'fromCurrency' and 'toCurrency' parameters are required."
                };
            }

            fromCurrency = fromCurrency.Trim().ToUpperInvariant();
            toCurrency = toCurrency.Trim().ToUpperInvariant();

            var validFrom = ValidateCurrency(fromCurrency);
            var validTo = ValidateCurrency(toCurrency);

            if (!validFrom || !validTo)
            {
                return new ExchangeResult
                {
                    From = fromCurrency,
                    To = toCurrency,
                    Rate = 0,
                    OriginalAmount = amount,
                    ConvertedAmount = 0,
                    Message = $"One or both currency codes are invalid. Please verify: '{fromCurrency}' or '{toCurrency}'."
                };
            }

            var random = new Random();
            var rate = (decimal)(random.NextDouble() * (1.5 - 0.5) + 0.5);
            var converted = Math.Round(amount * rate, 4);

            return new ExchangeResult
            {
                From = fromCurrency,
                To = toCurrency,
                Rate = Math.Round(rate, 4),
                OriginalAmount = amount,
                ConvertedAmount = converted,
                Message = $"Exchange successful: {amount} {fromCurrency} = {converted} {toCurrency}"
            };
        }
    }
}

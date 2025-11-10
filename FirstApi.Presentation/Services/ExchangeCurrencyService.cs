using System.Xml.Linq;

namespace FirstApi.Presentation.Services
{
    public class ExchangeCurrencyService : IExchangeCurrencyService
    {
        private readonly HashSet<string> _currencyCodes;

        public ExchangeCurrencyService()
        {
            var doc = XDocument.Load("./IsoCodes.xml");

            _currencyCodes = [.. doc.Descendants("CcyNtry")
                .Select(x => (string?)x.Element("Ccy"))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim().ToUpperInvariant())];
        }

        public async Task<decimal?> ExchangeAsync(string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("The ‘from’ and ‘to’ parameters cannot be empty.");

            from = from.Trim().ToUpperInvariant();
            to = to.Trim().ToUpperInvariant();

            if (!ValidateCurrency(from) || !ValidateCurrency(to))
            {
                Console.WriteLine("One or both currency codes are invalid. Returning 0 as rate.");
                return null;
            }

            var randomRate = new Random().NextDouble() * (1.5 - 0.5) + 0.5;

            return (decimal)randomRate;
        }

        private bool ValidateCurrency(string code)
        {
            if (_currencyCodes.Contains(code))
                return true;

            Console.WriteLine($"The currency code '{code}' does not exist.");
            return false;
        }
    }
}

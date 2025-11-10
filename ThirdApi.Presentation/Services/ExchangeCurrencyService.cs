using System.Xml.Linq;
using ThirdApi.Presentation.Model;

namespace ThirdApi.Presentation.Services
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

        public async Task<Response?> ExchangeAsync(string from, string to, decimal quantity)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                return new Response
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    From = from,
                    To = to,
                    ExchangeRate = 0m
                };

            from = from.Trim().ToUpperInvariant();
            to = to.Trim().ToUpperInvariant();

            if (!ValidateCurrency(from) || !ValidateCurrency(to))
            {
                Console.WriteLine("One or both currency codes are invalid. Returning 0 as rate.");
                return new Response
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    From = from,
                    To = to,
                    ExchangeRate = 0m
                };
            }

            var randomRate = new Random().NextDouble() * (1.5 - 0.5) + 0.5;

            return new Response
            {
                StatusCode = StatusCodes.Status200OK,
                From = from,
                To = to,
                ExchangeRate = Math.Round((decimal)randomRate, 4),
                ConvertedAmount = Math.Round(quantity * (decimal)randomRate, 4)
            };
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

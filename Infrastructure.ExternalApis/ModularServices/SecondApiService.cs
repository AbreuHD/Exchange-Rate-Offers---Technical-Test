using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Xml.Linq;

namespace Infrastructure.ExternalApis.ModularServices
{
    public class SecondApiService(HttpClient httpClient, ILogger<SecondApiService> logger) : IExchangeProvider
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<SecondApiService> _logger = logger;

        public async Task<GenericResponse<ExchangeResults?>> GetExchangeRateAsync(string from, string to, decimal amount)
        {
            try
            {
                var soapEnvelope = $@"
                    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
                       <soapenv:Header/>
                       <soapenv:Body>
                          <tem:GetExchangeRate>
                             <tem:fromCurrency> {from} </tem:fromCurrency >
                             <tem:toCurrency>{to}</tem:toCurrency >
                             <tem:amount>{amount}</tem:amount >
                          </tem:GetExchangeRate>
                       </soapenv:Body>
                    </soapenv:Envelope>"
                    ;

                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

                var response = await _httpClient.PostAsync("/Service.asmx", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("SecondApiService returned non-success status: {StatusCode}", response.StatusCode);
                    return new GenericResponse<ExchangeResults?>
                    {
                        Message = "Failed to retrieve exchange rate from SecondApiService.",
                        Statuscode = (int)response.StatusCode,
                        Payload = null
                    };
                }

                var xmlResponse = await response.Content.ReadAsStringAsync();
                var xdoc = XDocument.Parse(xmlResponse);

                XNamespace d4p1 = "http://schemas.datacontract.org/2004/07/SecondApi.Presentation.Model";

                var resultNode = xdoc.Descendants()
                                     .FirstOrDefault(x => x.Name.LocalName == "GetExchangeRateResult");

                if (resultNode == null)
                {
                    _logger.LogWarning("SecondApiService returned invalid SOAP structure: missing GetExchangeRateResult.");
                    return new GenericResponse<ExchangeResults?>
                    {
                        Message = "Invalid SOAP response from SecondApiService.",
                        Statuscode = 500,
                        Payload = null
                    };
                }

                var fromCurrency = resultNode.Element(d4p1 + "From")?.Value ?? from;
                var toCurrency = resultNode.Element(d4p1 + "To")?.Value ?? to;
                var rate = decimal.TryParse(resultNode.Element(d4p1 + "Rate")?.Value, out var r) ? r : 0;
                var convertedAmount = decimal.TryParse(resultNode.Element(d4p1 + "ConvertedAmount")?.Value, out var c) ? c : rate * amount;

                return new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults
                    {
                        ExchangePair = $"{fromCurrency}/{toCurrency}",
                        ProviderName = "SecondApiService",
                        Rate = rate,
                        ConvertedAmount = convertedAmount,
                    },
                    Statuscode = 200,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling SecondApiService.");
                return new GenericResponse<ExchangeResults?>
                {
                    Message = "An error occurred while retrieving exchange rate from SecondApiService.",
                    Statuscode = 500,
                    Payload = null
                };
            }
        }
    }
}
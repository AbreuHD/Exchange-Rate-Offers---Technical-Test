using CentralApi.Infrastructure.ExternalApis.ModularServices;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace CentralApi.Test.Infrastructure
{
    public class SecondApiServiceTests
    {
        private readonly Mock<ILogger<SecondApiService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly SecondApiService _service;

        public SecondApiServiceTests()
        {
            _mockLogger = new Mock<ILogger<SecondApiService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri("https://localhost:7142/") };
            _service = new SecondApiService(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetExchangeRateAsync_WithValidSoapResponse_ReturnsSuccess()
        {
            // Arrange
            var soapResponse = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                    <soap:Body>
                        <GetExchangeRateResponse xmlns=""http://tempuri.org/"">
                            <GetExchangeRateResult xmlns:d4p1=""http://schemas.datacontract.org/2004/07/SecondApi.Presentation.Model"">
                                <d4p1:From>USD</d4p1:From>
                                <d4p1:To>EUR</d4p1:To>
                                <d4p1:Rate>0.85</d4p1:Rate>
                                <d4p1:ConvertedAmount>85</d4p1:ConvertedAmount>
                            </GetExchangeRateResult>
                        </GetExchangeRateResponse>
                    </soap:Body>
                </soap:Envelope>";

            var responseContent = new StringContent(soapResponse, System.Text.Encoding.UTF8, "text/xml");

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

            // Act
            var result = await _service.GetExchangeRateAsync("USD", "EUR", 100m);

            // Assert
            result.Statuscode.Should().Be(200);
            result.Payload!.Rate.Should().Be(0.85m);
        }

        [Fact]
        public async Task GetExchangeRateAsync_WithNotFound_ReturnsError()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

            // Act
            var result = await _service.GetExchangeRateAsync("USD", "XXX", 100m);

            // Assert
            result.Statuscode.Should().Be(404);
            result.Payload.Should().BeNull();
        }

        [Fact]
        public async Task GetExchangeRateAsync_WithException_ReturnsError()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());

            // Act
            var result = await _service.GetExchangeRateAsync("USD", "EUR", 100m);

            // Assert
            result.Statuscode.Should().Be(500);
            result.Payload.Should().BeNull();
        }
    }
}

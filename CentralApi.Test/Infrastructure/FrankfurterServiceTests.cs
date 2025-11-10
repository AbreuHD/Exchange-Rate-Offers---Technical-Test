using FluentAssertions;
using Infrastructure.ExternalApis.ModularServices;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace CentralApi.Test.Infrastructure
{
    public class FrankfurterServiceTests
    {
        private readonly Mock<ILogger<FrankfurterService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly FrankfurterService _service;

        public FrankfurterServiceTests()
        {
            _mockLogger = new Mock<ILogger<FrankfurterService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.frankfurter.app/")
            };
            _service = new FrankfurterService(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetExchangeRateAsync_WithValidResponse_ReturnsSuccess()
        {
            // Arrange
            var responseData = new { amount = 1.0, @base = "USD", date = "2024-01-01", rates = new Dictionary<string, decimal> { { "EUR", 0.85m } } };
            var responseContent = new StringContent(JsonSerializer.Serialize(responseData), System.Text.Encoding.UTF8, "application/json");

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent });

            // Act
            var result = await _service.GetExchangeRateAsync("USD", "EUR", 100m);

            // Assert
            result.Statuscode.Should().Be(200);
            result.Payload!.Rate.Should().Be(0.85m);
            result.Payload.ConvertedAmount.Should().Be(85m);
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
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _service.GetExchangeRateAsync("USD", "EUR", 100m);

            // Assert
            result.Statuscode.Should().Be(500);
            result.Payload.Should().BeNull();
        }
    }
}

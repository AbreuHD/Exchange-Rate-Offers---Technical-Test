using CentralApi.Infrastructure.ExternalApis.ModularServices;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace CentralApi.Test.Infrastructure
{
    public class ThirdApiServiceTests
    {
        private readonly Mock<ILogger<ThirdApiService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly ThirdApiService _service;

        public ThirdApiServiceTests()
        {
            _mockLogger = new Mock<ILogger<ThirdApiService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost:5000/") };
            _service = new ThirdApiService(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetExchangeRateAsync_WithValidResponse_ReturnsSuccess()
        {
            // Arrange
            var responseData = new { StatusCode = 200L, From = "USD", To = "EUR", ExchangeRate = 0.85m, ConvertedAmount = 85m };
            var responseContent = new StringContent(JsonSerializer.Serialize(responseData), System.Text.Encoding.UTF8, "application/json");

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

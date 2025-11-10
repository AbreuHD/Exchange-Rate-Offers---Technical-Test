using FluentAssertions;
using Infrastructure.ExternalApis.ModularServices;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace CentralApi.Test.Infrastructure
{
    public class FloatratesServiceTests
    {
        private readonly Mock<ILogger<FloatratesService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly FloatratesService _service;

        public FloatratesServiceTests()
        {
            _mockLogger = new Mock<ILogger<FloatratesService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri("https://www.floatrates.com/") };
            _service = new FloatratesService(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetExchangeRateAsync_WithNotFound_ReturnsError()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

            // Act
            var result = await _service.GetExchangeRateAsync("XXX", "EUR", 100m);

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

using FluentAssertions;
using Infrastructure.ExternalApis.ModularServices;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace CentralApi.Test.Infrastructure
{
    public class FirstApiServiceTests
    {
        private readonly Mock<ILogger<FirstApiService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly FirstApiService _service;

        public FirstApiServiceTests()
        {
            _mockLogger = new Mock<ILogger<FirstApiService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost:5150/") };
            _service = new FirstApiService(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetExchangeRateAsync_WithValidResponse_ReturnsSuccess()
        {
            // Arrange
            var responseData = new { exchangeRate = 0.85m };
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

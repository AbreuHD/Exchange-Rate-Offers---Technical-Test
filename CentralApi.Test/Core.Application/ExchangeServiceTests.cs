using Core.Application.Services;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CentralApi.Test.Core.Application
{
    public class ExchangeServiceTests
    {
        private readonly Mock<IExchangeProvider> _mockProvider1;
        private readonly Mock<IExchangeProvider> _mockProvider2;
        private readonly ExchangeService _exchangeService;

        public ExchangeServiceTests()
        {
            _mockProvider1 = new Mock<IExchangeProvider>();
            _mockProvider2 = new Mock<IExchangeProvider>();

            var providers = new List<IExchangeProvider>
            {
                _mockProvider1.Object,
                _mockProvider2.Object
            };

            _exchangeService = new ExchangeService(providers);
        }

        [Fact]
        public async Task GetBestRateAsync_WithValidProviders_ReturnsBestRate()
        {
            // Arrange
            var request = new ExchangeRequest { From = "USD", To = "EUR", Amount = 100 };

            _mockProvider1.Setup(p => p.GetExchangeRateAsync("USD", "EUR", 100))
                .ReturnsAsync(new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults { ExchangePair = "USD/EUR", ProviderName = "Provider1", Rate = 0.85m, ConvertedAmount = 85m },
                    Statuscode = 200
                });

            _mockProvider2.Setup(p => p.GetExchangeRateAsync("USD", "EUR", 100))
                .ReturnsAsync(new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults { ExchangePair = "USD/EUR", ProviderName = "Provider2", Rate = 0.90m, ConvertedAmount = 90m },
                    Statuscode = 200
                });

            // Act
            var result = await _exchangeService.GetBestRateAsync(request);

            // Assert
            result.Statuscode.Should().Be(200);
            result.Payload!.ProviderName.Should().Be("Provider2");
            result.Payload.ConvertedAmount.Should().Be(90m);
        }

        [Fact]
        public async Task GetBestRateAsync_WithNoValidProviders_Returns404()
        {
            // Arrange
            var request = new ExchangeRequest { From = "USD", To = "EUR", Amount = 100 };

            _mockProvider1.Setup(p => p.GetExchangeRateAsync("USD", "EUR", 100))
                .ReturnsAsync(new GenericResponse<ExchangeResults?> { Payload = null, Statuscode = 404 });

            _mockProvider2.Setup(p => p.GetExchangeRateAsync("USD", "EUR", 100))
                .ReturnsAsync(new GenericResponse<ExchangeResults?> { Payload = null, Statuscode = 500 });

            // Act
            var result = await _exchangeService.GetBestRateAsync(request);

            // Assert
            result.Statuscode.Should().Be(404);
            result.Payload.Should().BeNull();
        }

        [Fact]
        public async Task GetRatesAsync_WithValidProviders_ReturnsAllRates()
        {
            // Arrange
            var request = new ExchangeRequest { From = "USD", To = "EUR", Amount = 100 };

            _mockProvider1.Setup(p => p.GetExchangeRateAsync("USD", "EUR", 100))
                .ReturnsAsync(new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults { ExchangePair = "USD/EUR", ProviderName = "Provider1", Rate = 0.85m, ConvertedAmount = 85m },
                    Statuscode = 200
                });

            _mockProvider2.Setup(p => p.GetExchangeRateAsync("USD", "EUR", 100))
                .ReturnsAsync(new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults { ExchangePair = "USD/EUR", ProviderName = "Provider2", Rate = 0.90m, ConvertedAmount = 90m },
                    Statuscode = 200
                });

            // Act
            var result = await _exchangeService.GetRatesAsync(request);

            // Assert
            result.Statuscode.Should().Be(200);
            result.Payload!.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetRatesAsync_WithNoValidProviders_Returns404()
        {
            // Arrange
            var request = new ExchangeRequest { From = "USD", To = "EUR", Amount = 100 };

            _mockProvider1.Setup(p => p.GetExchangeRateAsync("USD", "EUR", 100))
                .ReturnsAsync(new GenericResponse<ExchangeResults?> { Payload = null, Statuscode = 404 });

            _mockProvider2.Setup(p => p.GetExchangeRateAsync("USD", "EUR", 100))
                .ReturnsAsync(new GenericResponse<ExchangeResults?> { Payload = null, Statuscode = 500 });

            // Act
            var result = await _exchangeService.GetRatesAsync(request);

            // Assert
            result.Statuscode.Should().Be(404);
            result.Payload.Should().BeNull();
        }

        [Fact]
        public async Task GetBetterRatesAsync_WithValidRequests_ReturnsBestRates()
        {
            // Arrange
            var requests = new List<ExchangeRequest>
            {
                new ExchangeRequest { From = "USD", To = "EUR", Amount = 100 },
                new ExchangeRequest { From = "USD", To = "GBP", Amount = 200 }
            };

            _mockProvider1.Setup(p => p.GetExchangeRateAsync("USD", "EUR", 100))
                .ReturnsAsync(new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults { ExchangePair = "USD/EUR", ProviderName = "Provider1", Rate = 0.85m, ConvertedAmount = 85m },
                    Statuscode = 200
                });

            _mockProvider1.Setup(p => p.GetExchangeRateAsync("USD", "GBP", 200))
                .ReturnsAsync(new GenericResponse<ExchangeResults?>
                {
                    Payload = new ExchangeResults { ExchangePair = "USD/GBP", ProviderName = "Provider1", Rate = 0.75m, ConvertedAmount = 150m },
                    Statuscode = 200
                });

            _mockProvider2.Setup(p => p.GetExchangeRateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                .ReturnsAsync(new GenericResponse<ExchangeResults?> { Payload = null, Statuscode = 404 });

            // Act
            var result = await _exchangeService.GetBetterRatesAsync(requests);

            // Assert
            result.Statuscode.Should().Be(200);
            result.Payload!.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetBetterRatesAsync_WithEmptyList_Returns400()
        {
            // Act
            var result = await _exchangeService.GetBetterRatesAsync(new List<ExchangeRequest>());

            // Assert
            result.Statuscode.Should().Be(400);
            result.Payload.Should().BeNull();
        }
    }
}

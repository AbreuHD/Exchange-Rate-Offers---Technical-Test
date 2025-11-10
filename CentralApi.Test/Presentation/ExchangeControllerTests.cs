using CentralApi.Core.Domain.Common;
using CentralApi.Core.Domain.Entities;
using CentralApi.Core.Domain.Interfaces;
using CentralApi.Presentation.Api.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CentralApi.Test.Presentation
{
    public class ExchangeControllerTests
    {
        private readonly Mock<IExchangeService> _mockExchangeService;
        private readonly ExchangeController _controller;

        public ExchangeControllerTests()
        {
            _mockExchangeService = new Mock<IExchangeService>();
            _controller = new ExchangeController(_mockExchangeService.Object);
        }

        [Fact]
        public async Task GetBestRate_WithValidRequest_Returns200()
        {
            // Arrange
            var request = new ExchangeRequest { From = "USD", To = "EUR", Amount = 100 };
            var expectedResponse = new GenericResponse<ExchangeResults?>
            {
                Payload = new ExchangeResults { ExchangePair = "USD/EUR", ProviderName = "Provider1", Rate = 0.85m, ConvertedAmount = 85m },
                Statuscode = 200
            };

            _mockExchangeService.Setup(s => s.GetBestRateAsync(request)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetBestRate(request);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetBestRate_WithNullRequest_Returns400()
        {
            // Act
            var result = await _controller.GetBestRate(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetBestRate_WhenServiceReturns404_Returns404()
        {
            // Arrange
            var request = new ExchangeRequest { From = "USD", To = "XXX", Amount = 100 };
            var expectedResponse = new GenericResponse<ExchangeResults?> { Payload = null, Statuscode = 404 };

            _mockExchangeService.Setup(s => s.GetBestRateAsync(request)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetBestRate(request);

            // Assert
            ((ObjectResult)result).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetRates_WithValidRequest_Returns200()
        {
            // Arrange
            var request = new ExchangeRequest { From = "USD", To = "EUR", Amount = 100 };
            var expectedResponse = new GenericResponse<List<ExchangeResults?>>
            {
                Payload = new List<ExchangeResults?> { new ExchangeResults { ExchangePair = "USD/EUR", ProviderName = "Provider1", Rate = 0.85m, ConvertedAmount = 85m } },
                Statuscode = 200
            };

            _mockExchangeService.Setup(s => s.GetRatesAsync(request)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetRates(request);

            // Assert
            ((ObjectResult)result).StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetRates_WithNullRequest_Returns400()
        {
            // Act
            var result = await _controller.GetRates(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetBetterRates_WithValidRequests_Returns200()
        {
            // Arrange
            var requests = new List<ExchangeRequest> { new ExchangeRequest { From = "USD", To = "EUR", Amount = 100 } };
            var expectedResponse = new GenericResponse<List<ExchangeResults?>>
            {
                Payload = new List<ExchangeResults?> { new ExchangeResults { ExchangePair = "USD/EUR", ProviderName = "Provider1", Rate = 0.85m, ConvertedAmount = 85m } },
                Statuscode = 200
            };

            _mockExchangeService.Setup(s => s.GetBetterRatesAsync(requests)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetBetterRates(requests);

            // Assert
            ((ObjectResult)result).StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetBetterRates_WithEmptyList_Returns400()
        {
            // Act
            var result = await _controller.GetBetterRates(new List<ExchangeRequest>());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}

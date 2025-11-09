using Core.Application.Dtos;
using Core.Application.Services;
using Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController(IExchangeService exchangeService) : ControllerBase
    {
        private readonly IExchangeService _exchangeService = exchangeService;

        [HttpPost("best-offer")]
        public async Task<IActionResult> GetBestRate([FromQuery] ExchangeRequest request)
        {
            var result = await _exchangeService.GetBestRateAsync(request.From, request.To, request.Amount);
            return StatusCode(result.Statuscode, result);
        }

        [HttpPost("rates")]
        public async Task<IActionResult> GetRates([FromQuery] ExchangeRequest request)
        {
            var result = await _exchangeService.GetRatesAsync(request.From, request.To, request.Amount);
            return StatusCode(result.Statuscode, result);
        }
    }
}

using Core.Application.Dtos;
using Core.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController(ExchangeService exchangeService) : ControllerBase
    {
        private readonly ExchangeService _exchangeService = exchangeService;

        [HttpPost("best-offer")]
        public async Task<IActionResult> GetBestRate([FromQuery] ExchangeRequest request)
        {
            var result = await _exchangeService.GetBestRateAsync(request.From, request.To, request.Amount);
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ThirdApi.Presentation.Services;

namespace ThirdApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeMoneyController(IExchangeCurrencyService exchangeCurrencyService) : ControllerBase
    {
        private readonly IExchangeCurrencyService _exchangeCurrencyService = exchangeCurrencyService;

        [HttpGet("change")]
        public async Task<IActionResult> ChangeValue(string from, string to, decimal quantity)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return BadRequest("Parameters cannot be null or empty.");
            }

            var response = await _exchangeCurrencyService.ExchangeAsync(from, to, quantity);

            if (response is null)
            {
                return NotFound("Exchange rate not found.");
            }
            return StatusCode(response.StatusCode, response);
        }
    }
}

using FirstApi.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController(IExchangeCurrencyService exchangeCurrencyService) : ControllerBase
    {
        private readonly IExchangeCurrencyService _exchangeCurrencyService = exchangeCurrencyService;

        [HttpGet("change")]
        public async Task<IActionResult> ChangeValue(string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return BadRequest("Parameters cannot be null or empty.");
            }

            var response = await _exchangeCurrencyService.ExchangeAsync(from, to);

            if (response is null)
            {
                return NotFound("Exchange rate not found.");
            }
            return Ok(new
            {
                From = from.ToUpper(),
                To = to.ToUpper(),
                ExchangeRate = response
            });
        }
    }
}

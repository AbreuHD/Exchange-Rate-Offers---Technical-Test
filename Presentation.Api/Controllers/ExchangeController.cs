using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers
{
    /// <summary>
    /// Controller responsible for handling exchange rate operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController(IExchangeService exchangeService) : ControllerBase
    {
        private readonly IExchangeService _exchangeService = exchangeService;

        /// <summary>
        /// Gets the best available exchange rate among all providers for a given request.
        /// </summary>
        /// <param name="request">Exchange request containing the origin, target currency, and amount.</param>
        /// <returns>Best exchange rate with provider information.</returns>
        [HttpPost("best-offer")]
        [ProducesResponseType(typeof(GenericResponse<ExchangeResults?>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<ExchangeResults?>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBestRate([FromBody] ExchangeRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null.");

            var result = await _exchangeService.GetBestRateAsync(request);
            return StatusCode(result.Statuscode, result);
        }

        /// <summary>
        /// Retrieves all available exchange rates from every provider for the specified request.
        /// </summary>
        /// <param name="request">Exchange request containing the currencies and amount.</param>
        /// <returns>List of exchange rates from all providers.</returns>
        [HttpPost("rates")]
        [ProducesResponseType(typeof(GenericResponse<List<ExchangeResults?>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<List<ExchangeResults?>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRates([FromBody] ExchangeRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null.");

            var result = await _exchangeService.GetRatesAsync(request);
            return StatusCode(result.Statuscode, result);
        }

        /// <summary>
        /// Retrieves the best exchange rate for each request in a list of multiple exchange requests.
        /// </summary>
        /// <param name="requests">A list of exchange requests to evaluate.</param>
        /// <returns>A list containing the best rates for each request.</returns>
        [HttpPost("rate-list")]
        [ProducesResponseType(typeof(GenericResponse<List<ExchangeResults?>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<List<ExchangeResults?>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBetterRates([FromBody] List<ExchangeRequest> requests)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest("At least one exchange request must be provided.");

            var result = await _exchangeService.GetBetterRatesAsync(requests);
            return StatusCode(result.Statuscode, result);
        }
    }
}

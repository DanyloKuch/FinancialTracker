using FinancialTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.API.Controllers
{
    [ApiController]
    [Route("api/v1/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("rates")]
        public IActionResult GetRates()
        {
            return Ok(new
            {
                USD = _currencyService.UsdRate,
                EUR = _currencyService.EurRate,
                UpdatedAt = _currencyService.LastUpdatedAt
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> ForceRefresh()
        {
            await _currencyService.RefreshRatesAsync();
            return Ok("Rates updated manually");
        }
    }
}
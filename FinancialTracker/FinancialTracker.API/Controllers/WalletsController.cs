using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] 
    [Authorize] 
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WalletRequest request)
        {
            var result = await _walletService.CreateWalletAsync(request);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

           
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var wallets = await _walletService.GetWalletsAsync();
            return Ok(wallets); 
        }

      
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _walletService.GetWalletByIdAsync(id);

            if (result.IsFailure)
            {
                return NotFound(result.Error); 
            }

            return Ok(result.Value);
        }

      
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] WalletRequest request)
        {
            var result = await _walletService.UpdateWalletAsync(id, request);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value); 
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _walletService.DeleteWalletAsync(id);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpGet("{id:guid}/details")]
        public async Task<IActionResult> GetWithStats(Guid id)
        {
            var result = await _walletService.GetWalletWithStatsAsync(id);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
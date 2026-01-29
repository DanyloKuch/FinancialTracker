using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.API.Controllers
{
    [ApiController]
    [Route("api/v1/groups")]
    [Authorize]
    public class GroupLimitsController : ControllerBase
    {
        private readonly IGroupLimitService _limitService;

        public GroupLimitsController(IGroupLimitService limitService)
        {
            _limitService = limitService;
        }

        [HttpGet("{groupId}/limits")]
        public async Task<IActionResult> GetLimits(Guid groupId)
        {
            var limits = await _limitService.GetLimitsAsync(groupId);
            return Ok(limits);
        }

        [HttpPost("{groupId}/limits")]
        public async Task<IActionResult> SetLimit(Guid groupId, [FromBody] SetLimitRequest request)
        {
            var result = await _limitService.SetLimitAsync(groupId, request);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok();
        }
    }
}
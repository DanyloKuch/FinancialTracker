using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.API.Controllers
{
    [ApiController]
    [Route("api/v1/invitations")]
    [Authorize]
    public class InvitationsController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationsController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyInvitations()
        {
            var result = await _invitationService.GetMyPendingInvitationsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendInvitation([FromBody] InviteUserRequest request)
        {
            var result = await _invitationService.InviteUserAsync(request);
            if (!result.IsSuccess) return BadRequest(new { message = result.Error });
            return Ok(new { InvitationId = result.Value });
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptInvitation(Guid id)
        {
            var result = await _invitationService.AcceptInvitationAsync(id);
            if (!result.IsSuccess) return BadRequest(new { message = result.Error });
            return Ok(new { message = "Invitation accepted" });
        }
    }
}
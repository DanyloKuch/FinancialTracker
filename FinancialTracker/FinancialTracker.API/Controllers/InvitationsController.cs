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

        [HttpGet("received")]
        public async Task<IActionResult> GetReceived()
        {
            var result = await _invitationService.GetMyReceivedInvitationsAsync();
            return Ok(result);
        }

        [HttpGet("sent")]
        public async Task<IActionResult> GetSent()
        {
            var result = await _invitationService.GetMySentInvitationsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendInvitation([FromBody] InviteUserRequest request)
        {
            var result = await _invitationService.InviteUserAsync(request);
            if (!result.IsSuccess) return BadRequest(new { message = result.Error });
            return Ok(new { InvitationId = result.Value });
        }

        [HttpPost("{id}/respond")]
        public async Task<IActionResult> RespondToInvitation(Guid id, [FromBody] RespondInvitationRequest request)
        {
            var result = await _invitationService.RespondToInvitationAsync(id, request.IsAccepted);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            var statusMessage = request.IsAccepted ? "Accepted" : "Rejected";
            return Ok(new { message = $"Invitation {statusMessage}" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelInvitation(Guid id)
        {
            var result = await _invitationService.CancelInvitationAsync(id);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return NoContent();
        }
    }
}
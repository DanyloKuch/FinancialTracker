using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupRequest request)
        {
            var result = await _groupService.CreateGroupAsync(request);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            
            return CreatedAtAction(nameof(GetGroupById), new { id = result.Value }, result.Value);
        }


        [HttpGet]
        public async Task<IActionResult> GetMyGroups()
        {
            var result = await _groupService.GetAllUserGroupsAsync();

         
            return Ok(result.Value);
        }

       
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            var result = await _groupService.GetGroupByIdAsync(id);

            if (result.IsFailure)
            {
            
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOrLeaveGroup(Guid id)
        {
            var result = await _groupService.DeleteOrLeaveGroupAsync(id);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(new { message = "Successfully processed request." });
        }
    }
}
using FinancialTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.API.Controllers;

[ApiController]
[Route("api/test")]
public class TestAuthController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public TestAuthController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMyId()
    {
        return Ok(new
        {
            Message = "Ти авторизований!",
            UserId = _currentUserService.UserId
        });
    }
}
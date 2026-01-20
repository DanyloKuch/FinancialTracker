using System.Security.Claims;
using FinancialTracker.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FinancialTracker.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var idClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                return Guid.TryParse(idClaim, out var parsedId) ? parsedId : Guid.Empty;
            }
        }
    }
}

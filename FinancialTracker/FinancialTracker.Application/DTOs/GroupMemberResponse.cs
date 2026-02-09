using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Application.DTOs
{
    public record GroupMemberResponse(
        Guid Id,
        Guid UserId,
        string Email, 
        GroupRole Role,
        DateTime JoinedAt
    );
}
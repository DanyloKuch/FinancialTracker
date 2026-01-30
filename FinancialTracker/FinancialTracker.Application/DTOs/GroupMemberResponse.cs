using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Application.DTOs
{
    public record GroupMemberResponse(
        Guid Id,
        Guid UserId,
        GroupRole Role,
        DateTime JoinedAt
    );
}
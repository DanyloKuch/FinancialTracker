namespace FinancialTracker.Application.DTOs
{
    public record GroupResponse(
        Guid Id,
        string Name,
        Guid OwnerId,
        string OwnerEmail, 
        string BaseCurrency,
        decimal? TotalLimit,
        IReadOnlyList<GroupMemberResponse> Members,
        DateTime CreatedAt,
        bool IsOwner 
    );
}
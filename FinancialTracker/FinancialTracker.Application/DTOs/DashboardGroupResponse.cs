namespace FinancialTracker.Application.DTOs
{
    public record DashboardGroupResponse(
        Guid Id,
        string Name,
        decimal TotalLimit,
        decimal SpentAmount
    );
}

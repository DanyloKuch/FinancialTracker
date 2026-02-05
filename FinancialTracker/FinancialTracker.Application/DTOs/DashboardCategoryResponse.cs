namespace FinancialTracker.Application.DTOs
{
    public record DashboardCategoryResponse
    (
        Guid Id,
        string Name,
        decimal TotalLimit,
        decimal SpentAmount,
        decimal RemainingAmount
    );
}

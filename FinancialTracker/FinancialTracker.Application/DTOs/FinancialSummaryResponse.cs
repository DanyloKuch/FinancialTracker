namespace FinancialTracker.Application.DTOs
{
    public record FinancialSummaryResponse(
        decimal TotalIncome,
        decimal TotalExpense
    );
}
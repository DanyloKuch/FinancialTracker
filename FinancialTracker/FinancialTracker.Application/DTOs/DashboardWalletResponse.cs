namespace FinancialTracker.Application.DTOs
{
    public record DashboardWalletResponse(
        Guid Id,
        string Name,
        decimal Balance,
        string Currency,
        decimal TotalIncome,
        decimal TotalExpense
    );
}



namespace FinancialTracker.Application.DTOs
{
    public record WalletWithStatsResponse(
        Guid Id,
        string Name,
        string Type,
        decimal Balance,
        string CurrencyCode,
        decimal TotalIncome,  
        decimal TotalExpense, 
        DateTime UpdatedAt
    );
}
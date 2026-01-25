namespace FinancialTracker.Application.DTOs
{
    public record WalletResponse(
       Guid Id,
       string Name,
       string Type,
       decimal Balance,
       string CurrencyCode,
       DateTime UpdatedAt
   );
}

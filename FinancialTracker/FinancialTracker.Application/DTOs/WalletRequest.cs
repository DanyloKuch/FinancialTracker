namespace FinancialTracker.Application.DTOs
{
        public record WalletRequest(
          string Name,
          string Type,
          decimal Balance,
          string CurrencyCode
      );
}
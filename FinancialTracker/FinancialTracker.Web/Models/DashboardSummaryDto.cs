namespace FinancialTracker.Web.Models
{
    public class DashboardSummaryDto
    {
        public decimal TotalBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public List<WalletDto> Wallets { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<GroupDto> Groups { get; set; } = new();
        public List<TransactionDto> RecentTransactions { get; set; } = new();
        public List<ExchangeRateDto> ExchangeRates { get; set; } = new();
    }
}

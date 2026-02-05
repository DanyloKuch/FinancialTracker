namespace FinancialTracker.Application.DTOs
{
    public class DashboardSummaryResponse
    {
        public decimal TotalBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }

        public List<DashboardWalletResponse> Wallets { get; set; } = new();
        public List<DashboardCategoryResponse> Categories { get; set; } = new();
        public List<DashboardGroupResponse> Groups { get; set; } = new();
        public List<TransactionResponse> RecentTransactions { get; set; } = new();
        public List<DashboardCurrencyRateResponse> ExchangeRates { get; set; } = new();
    }
}

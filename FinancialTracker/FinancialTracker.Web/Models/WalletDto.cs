namespace FinancialTracker.Web.Models
{
    public class WalletDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public string Type { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }
}

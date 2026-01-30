namespace FinancialTracker.Web.Models
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public int Type { get; set; } // 0=Income, 1=Expense, 2=Transfer
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}

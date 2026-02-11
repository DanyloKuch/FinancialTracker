namespace FinancialTracker.Web.Models
{
    public class TransactionUpdateDto
    {
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public Guid? CategoryId { get; set; }
        public string Comment { get; set; }
        public Guid? TargetWalletId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Commission { get; set; }
        public int Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

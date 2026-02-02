namespace FinancialTracker.Web.Models
{
    public class TransactionCreateDto
    {
        public Guid walletid { get; set; } 
        public Guid? targetWalletId { get; set; }
        public Guid categoryId { get; set; }
        public Guid? groupId { get; set; }
        public double amount { get; set; }
        public int type { get; set; } 
        public double? exchangeRate { get; set; }
        public double commission { get; set; }
        public string comment { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; } = DateTime.Now;
    }
}

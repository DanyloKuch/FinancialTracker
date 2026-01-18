namespace FinancialTracker.Infrastructure.Entities
{
    public class WalletEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!; 
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public bool IsArchived { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserEntity User { get; set; } = null!;
        public ICollection<TransactionEntity> Transactions { get; set; }
    }
}

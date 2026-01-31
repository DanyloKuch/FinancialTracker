namespace FinancialTracker.Infrastructure.Entities
{
    public class CategoryEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsArchived { get; set; }
        public decimal TotalLimit { get; set; }
        public UserEntity User { get; set; } = null!;
        public ICollection<TransactionEntity> Transactions { get; set; }
    }
}

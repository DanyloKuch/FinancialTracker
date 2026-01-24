using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Infrastructure.Entities
{
    public class    TransactionEntity
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? GroupId { get; set; }

        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }

        public Guid? TargetWalletId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Commission { get; set; }

        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public WalletEntity Wallet { get; set; } = null!;
        public WalletEntity? TargetWallet { get; set; }
        public UserEntity User { get; set; } = null!;
        public CategoryEntity Category { get; set; } = null!;
        public GroupEntity? Group { get; set; }
    }
}

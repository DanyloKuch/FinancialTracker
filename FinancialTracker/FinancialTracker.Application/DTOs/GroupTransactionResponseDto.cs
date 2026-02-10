using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Application.DTOs
{
    public record GroupTransactionResponseDto
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string UserEmail { get; init; } 
        public Guid WalletId { get; init; }
        public Guid? TargetWalletId { get; init; }
        public Guid CategoryId { get; init; }
        public decimal Amount { get; init; }
        public TransactionType Type { get; init; }
        public string Comment { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}   

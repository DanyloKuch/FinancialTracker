using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Application.DTOs
{
    public record TransactionResponse(
        Guid Id,
        Guid WalletId,
        Guid? TargetWalletId,
        Guid CategoryId,
        Guid? GroupId,
        decimal Amount,
        TransactionType Type,
        decimal? ExchangeRate,
        decimal? Commission,
        string? Comment,
        DateTime CreatedAt
    );
}   

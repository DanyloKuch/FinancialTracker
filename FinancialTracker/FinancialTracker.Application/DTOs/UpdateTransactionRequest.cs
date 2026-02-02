namespace FinancialTracker.Application.DTOs
{
    public record UpdateTransactionRequest
    (
        Guid WalletId,
        decimal Amount, 
        Guid CategoryId, 
        string? Comment,
        Guid? TargetWalletId,
        decimal? ExchangeRate,
        decimal? Commission,
        DateTime CreatedAt
        );
}

namespace FinancialTracker.Application.DTOs
{
    public record UpdateTransactionRequest
    (
        decimal Amount, 
        Guid CategoryId, 
        string? Comment,
        Guid? TargetWalletId,
        decimal? ExchangeRate,
        decimal? Commission
        );
}   

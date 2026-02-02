using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Application.DTOs
{
    public record CreateTransactionRequest(
        Guid Walletid, 
        Guid? TargetWalletId, 
        Guid CategoryId, 
        Guid? GroupId,
        decimal Amount, 
        TransactionType Type, 
        decimal? ExchangeRate, 
        decimal Commission, 
        string Comment,
        DateTime? CreatedAt
        );
}   

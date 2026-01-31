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




//{
//    "id": "7dd98509-94f0-40b4-acac-06d8372b7fa9",
//      "userId": "4a30592f-1bc2-4879-74bb-08de5802fc6b",
//      "walletId": "94464ce9-3786-40f3-9ebd-d10563d44eff",
//      "targetWalletId": null,
//      "categoryId": "21ce8fa2-8fa7-402e-80b0-d71d8ff71b48",
//      "groupId": null,
//      "amount": 300,
//      "type": 1,
//      "exchangeRate": null,
//      "commission": 1,
//      "comment": "",
//      "createdAt": "2026-01-24T12:28:54.6252236"
//    }

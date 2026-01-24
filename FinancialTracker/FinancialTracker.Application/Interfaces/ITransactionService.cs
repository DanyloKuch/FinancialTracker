using FinancialTracker.Application.DTOs;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Services
{
    public interface ITransactionService
    {
        Task<Result<Guid>> CreateTransactionAsync(CreateTransactionRequest request);
        Task<Result<IReadOnlyList<Transaction>>> GetAllTransactionByUser();
        Task<Result<TransactionResponse>> GetTransactionById(Guid id);
    }
}
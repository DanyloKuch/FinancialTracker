using FinancialTracker.Application.DTOs;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Services
{
    public interface ITransactionService
    {
        Task<Result<Guid>> CreateTransactionAsync(CreateTransactionRequest request);
        Task<Result<PagedResult<Transaction>>> GetAllTransactionByUser(int page, int pageSize);
        Task<Result<TransactionResponse>> GetTransactionById(Guid id);
        Task<Result<TransactionResponse>> UpdateTransaction(Guid id, UpdateTransactionRequest request);
        Task<Result> DeleteTransaction(Guid id);
        Task<Result<FinancialSummaryResponse>> GetGeneralFinancialSummary();
        Task<Result<PagedResult<GroupTransactionResponseDto>>> GetTransactionByGroup(Guid groupId, int page, int pageSize);
    }
}
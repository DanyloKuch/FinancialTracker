using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task SaveChangesAsync();
        Task<Result<IReadOnlyList<Transaction>>> GetAllTransactionByUser(Guid userId);
        Task<Result<Guid>> Create(Transaction transaction);
        Task<Result<Transaction>> GetById(Guid userId, Guid transactionId);
        Task<Result> Delete(Guid userId, Guid transactionId);
        Task UpdateAsync(Transaction transaction);
    }
}
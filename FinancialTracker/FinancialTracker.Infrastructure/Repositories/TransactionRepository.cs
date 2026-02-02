using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Infrastructure.Entities;
using FinancialTracker.Application.DTOs;

namespace FinancialTracker.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly FinancialTrackerDbContext _context;

        public TransactionRepository(FinancialTrackerDbContext ftcontext)
        {
            _context = ftcontext;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task <Result<Transaction>> GetById(Guid userId, Guid transactionId)
        {
            var result = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

            if (result == null)
                return Result<Transaction>.Failure("Transaction not found");

            return Transaction.Create(
                result.Id,
                result.UserId,
                result.WalletId,
                result.TargetWalletId,
                result.CategoryId,
                result.GroupId,
                result.Amount,
                result.Type,
                result.ExchangeRate,
                result.Commission ?? 0,
                result.Comment,
                result.CreatedAt
                );
        }


        public async Task<Result<(IReadOnlyList<Transaction> Items, int TotalCount)>> GetAllTransactionByUser(Guid userId, int page, int pageSize)
        {
            var query =  _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId);

            var totalCount = await query.CountAsync();

            var transactionEntities = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var transactions = transactionEntities
                .Select(t => Transaction.Create(
                    id: t.Id,
                    userId: t.UserId,
                    walletid: t.WalletId,
                    targetWalletId: t.TargetWalletId,
                    categoryId: t.CategoryId,
                    groupId: t.GroupId,
                    amount: t.Amount,
                    type: t.Type,
                    exchangeRate: t.ExchangeRate,
                    commission: t.Commission ?? 0m,
                    comment: t.Comment ?? string.Empty,
                    createdAt: t.CreatedAt))
                .Where(r => r.IsSuccess)
                .Select(r => r.Value)
                .ToList();

            return Result<(IReadOnlyList<Transaction>, int)>.Success((transactions, totalCount));
        }

        public async Task<Result<Guid>> Create(Transaction transaction)
        {
            var entity = new TransactionEntity
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                WalletId = transaction.WalletId,
                TargetWalletId = transaction.TargetWalletId,
                CategoryId = transaction.CategoryId,
                GroupId = transaction.GroupId,
                Amount = transaction.Amount,
                Type = transaction.Type,
                ExchangeRate = transaction.ExchangeRate,
                Commission = transaction.Commission,
                Comment = transaction.Comment,
                CreatedAt = transaction.CreatedAt

            };

            await _context.Transactions.AddAsync(entity);
            return Result<Guid>.Success(transaction.Id);
        }

        public async Task<Result> Delete(Guid userId, Guid transactionId)
        {
            var entity = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

            if (entity == null)
                return Result.Failure("Transaction not found.");

            _context.Transactions.Remove(entity);
            return Result.Success();
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            var entity = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transaction.Id);
            if (entity != null)
            {
                entity.WalletId = transaction.WalletId;
                entity.Amount = transaction.Amount; 
                entity.CategoryId = transaction.CategoryId;
                entity.Comment = transaction.Comment;
                entity.ExchangeRate = transaction.ExchangeRate;
                entity.Commission = transaction.Commission;
                entity.TargetWalletId = transaction.TargetWalletId;
                entity.CreatedAt = transaction.CreatedAt;
            }
        }
    }
}

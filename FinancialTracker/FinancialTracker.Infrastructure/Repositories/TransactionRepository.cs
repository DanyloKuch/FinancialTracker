using FinancialTracker.Application.DTOs;
using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task <Result<Transaction>> GetById(Guid transactionId, Guid userId)
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

        public async Task<Result<Dictionary<TransactionType, decimal>>> GetTotalsGroupedByType(Guid userId)
        {
            var totals = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .Where(t => t.Type == TransactionType.Income || t.Type == TransactionType.Expense)
                .GroupBy(t => t.Type)
                .Select(g => new { Type = g.Key, Sum = g.Sum(t => t.Amount) })
                .ToDictionaryAsync(x => x.Type, x => x.Sum);

            return Result<Dictionary<TransactionType, decimal>>.Success(totals);
        }

        public async Task<Result<(IReadOnlyList<Transaction> Items, int TotalCount)>> GetAllTransactionByGroup(Guid groupId, int page, int pageSize)
        {
            var query = _context.Transactions
                .AsNoTracking()
                .Where(t => t.GroupId == groupId);

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

        public async Task<decimal> GetTotalByCategoryIdAsync(Guid userId, Guid categoryId)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.CategoryId == categoryId)
                .SumAsync(t => t.Amount);
        }

        public async Task<Dictionary<Guid, (decimal Income, decimal Expense)>> GetWalletStatsAsync(Guid userId)
        {
            var stats = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.WalletId)
                .Select(g => new
                {
                    WalletId = g.Key,
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .ToListAsync();

            return stats.ToDictionary(k => k.WalletId, v => (v.Income, v.Expense));
        }

        public async Task<Dictionary<Guid, decimal>> GetCategorySpendingAsync(Guid userId)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense)
                .GroupBy(t => t.CategoryId)
                .Select(g => new { CategoryId = g.Key, Spent = g.Sum(t => t.Amount) })
                .ToDictionaryAsync(k => k.CategoryId, v => v.Spent);
        }

        public async Task<Dictionary<Guid, decimal>> GetGroupSpendingAsync(Guid userId)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.GroupId != null && t.Type == TransactionType.Expense)
                .GroupBy(t => t.GroupId!.Value)
                .Select(g => new { GroupId = g.Key, Spent = g.Sum(t => t.Amount) })
                .ToDictionaryAsync(k => k.GroupId, v => v.Spent);
        }

        public async Task<List<Transaction>> GetByWalletIdAsync(Guid walletId, int count)
        {
            var entities = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();

            return entities.Select(e => Transaction.Load(
                e.Id,
                e.WalletId,
                e.TargetWalletId,
                e.UserId,
                e.CategoryId,
                e.GroupId,
                e.Amount,
                e.Type,
                e.ExchangeRate,
                e.Commission,
                e.Comment,
                e.CreatedAt
            )).ToList();
        }
    }
}

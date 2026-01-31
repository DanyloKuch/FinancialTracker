using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Domain.Models
{
    public class Transaction
    {

        private Transaction(Guid id, Guid walletid, Guid? targetWalletId, Guid userId, Guid categoryId, Guid? groupId, 
            decimal amount, TransactionType type, decimal? exchangeRate, decimal? commission, string? comment, DateTime createdAt)
        {
            Id = id;
            UserId = userId;
            WalletId = walletid;
            TargetWalletId = targetWalletId;
            CategoryId = categoryId;
            GroupId = groupId;
            Amount = amount;
            Type = type;
            ExchangeRate = exchangeRate;
            Commission = commission;
            Comment = comment;
            CreatedAt = createdAt;
        }
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid WalletId { get; private set; }
        public Guid? TargetWalletId { get; private set; }
        public Guid CategoryId { get; private set; }
        public Guid? GroupId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public decimal? ExchangeRate { get; private set; }
        public decimal? Commission { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public static Result<Transaction> Create(Guid id, Guid userId, Guid walletid, Guid? targetWalletId, Guid categoryId, Guid? groupId,
            decimal amount, TransactionType type, decimal? exchangeRate, decimal commission, string comment, DateTime createdAt)
        {
            if (amount <= 0)
                return Result<Transaction>.Failure("Amount must be greater than zero.");
            if (userId == Guid.Empty)
                return Result<Transaction>.Failure("UserId is required.");  
            if (walletid == Guid.Empty)
                return Result<Transaction>.Failure("WalletId is required.");
            if (categoryId == Guid.Empty)
                return Result<Transaction>.Failure("CategoryId is required.");
            if (type != TransactionType.Transfer && targetWalletId != null)
            {
                return Result<Transaction>.Failure("Target wallet cannot be set for Income or Expense.");
            }

            if (type == TransactionType.Transfer && targetWalletId == null)
            {
                return Result<Transaction>.Failure("Target wallet is required for transfers.");
            }

            if (type == TransactionType.Transfer && walletid == targetWalletId)
            {
                return Result<Transaction>.Failure("Source and target wallets must be different.");
            }


            var transaction = new Transaction(id, walletid, targetWalletId, userId, categoryId, groupId,
                amount, type, exchangeRate, commission, comment, createdAt);
            return Result<Transaction>.Success(transaction);
        }

        public void Update(Guid walletId ,decimal amount, Guid categoryId, string? comment, Guid? targetWalletId, decimal? exchangeRate, decimal? commission, DateTime createdAt)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

            if (Type == TransactionType.Transfer && targetWalletId == null)
                throw new ArgumentException("Target wallet is required for transfers.");

            WalletId = walletId;
            Amount = amount;
            CategoryId = categoryId;
            Comment = comment;
            TargetWalletId = targetWalletId;
            ExchangeRate = exchangeRate;
            Commission = commission;
            CreatedAt = createdAt;
        }
    }
}

using FinancialTracker.Domain.Enums;
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
            decimal amount, TransactionType type, decimal exchangeRate, decimal commission, string comment, DateTime createdAt)
        {
        }
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public Guid? TargetWalletId { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? GroupId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Commission { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

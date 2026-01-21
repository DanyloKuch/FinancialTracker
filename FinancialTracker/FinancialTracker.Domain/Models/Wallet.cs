using FinancialTracker.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Domain.Models
{
    public class Wallet
    {
        public Guid Id { get;  }
        public Guid UserId { get;  }
        public string Name { get; private set; }
        public string Type { get; private set; } 
        public decimal Balance { get; private set; }
        public string CurrencyCode { get; private set; }
        public bool IsArchived { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        private Wallet(Guid id, Guid userId, string name, string type, decimal balance, string currencyCode, bool isArchived, DateTime updatedAt)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Type = type;
            Balance = balance;
            CurrencyCode = currencyCode;
            IsArchived = isArchived;
            UpdatedAt = updatedAt;
        }
        public static Result<Wallet> Create(Guid id, Guid userId, string name, string type, decimal balance, string currencyCode, bool isArchived, DateTime updatedAt)
        {
           
            if (string.IsNullOrWhiteSpace(name))
                return Result<Wallet>.Failure("Wallet name cannot be empty.");

            if (string.IsNullOrWhiteSpace(type))
                return Result<Wallet>.Failure("Wallet type cannot be empty.");

            if (balance < 0)
                return Result<Wallet>.Failure("Balance cannot be negative.");

            if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Length != 3)
                return Result<Wallet>.Failure("Currency code must be 3 characters.");

            if (userId == Guid.Empty)
                return Result<Wallet>.Failure("UserId is required.");

           
            var wallet = new Wallet(id, userId, name, type, balance, currencyCode, isArchived, updatedAt);

            return Result<Wallet>.Success(wallet);
        }
    }
}

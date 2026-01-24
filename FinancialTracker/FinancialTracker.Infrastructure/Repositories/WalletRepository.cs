using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
using FinancialTracker.Infrastructure.Entities;

namespace FinancialTracker.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly FinancialTrackerDbContext _context;

        public WalletRepository(FinancialTrackerDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Result<Guid>> AddAsync(Wallet wallet)
        {
            var entity = new WalletEntity
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                Name = wallet.Name,
                Type = wallet.Type,
                Balance = wallet.Balance,
                CurrencyCode = wallet.CurrencyCode,
                IsArchived = wallet.IsArchived,
                UpdatedAt = wallet.UpdatedAt
            };

            await _context.Wallets.AddAsync(entity);
            await _context.SaveChangesAsync();

            return Result<Guid>.Success(entity.Id);
        }

        
        public async Task<Result<Wallet>> GetByIdAsync(Guid walletId, Guid userId)
        {
            var entity = await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);

            if (entity == null)
                return Result<Wallet>.Failure("Wallet not found or access denied.");

            return Wallet.Create(
                entity.Id,
                entity.UserId,
                entity.Name,
                entity.Type,
                entity.Balance,
                entity.CurrencyCode,
                entity.IsArchived,
                entity.UpdatedAt
            );
        }


        public async Task<IEnumerable<Wallet>> GetAllByUserIdAsync(Guid userId)
        {
            var entities = await _context.Wallets
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .ToListAsync();

            var wallets = new List<Wallet>();

            foreach (var entity in entities)
            {
                var result = Wallet.Create(
                    entity.Id,
                    entity.UserId,
                    entity.Name,
                    entity.Type,
                    entity.Balance,
                    entity.CurrencyCode,
                    entity.IsArchived,
                    entity.UpdatedAt
                );

                if (result.IsSuccess)
                {
                    wallets.Add(result.Value);
                }
            }

            return wallets;
        }

   
        public async Task<Result> UpdateAsync(Wallet wallet)
        {
            var entity = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == wallet.Id && w.UserId == wallet.UserId);

            if (entity == null)
                return Result.Failure("Wallet not found.");

           
            entity.Name = wallet.Name;
            entity.Type = wallet.Type;
            entity.Balance = wallet.Balance;
            entity.IsArchived = wallet.IsArchived;
            entity.UpdatedAt = wallet.UpdatedAt;
            entity.CurrencyCode = wallet.CurrencyCode;

            return Result.Success();
        }

        
        public async Task<Result> DeleteAsync(Guid walletId, Guid userId)
        {
            var entity = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);

            if (entity == null)
                return Result.Failure("Wallet not found.");

            _context.Wallets.Remove(entity);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}

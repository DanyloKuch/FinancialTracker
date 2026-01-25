using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Domain.Interfaces
{
    public interface IWalletRepository
    {
        Task SaveChangesAsync();
        Task<Result<Guid>> AddAsync(Wallet wallet);
        Task<Result<Wallet>> GetByIdAsync(Guid walletId, Guid userId);       
        Task<IEnumerable<Wallet>> GetAllByUserIdAsync(Guid userId);
        Task<Result> UpdateAsync(Wallet wallet);      
        Task<Result> DeleteAsync(Guid walletId, Guid userId);
    }
}

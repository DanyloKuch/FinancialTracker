using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialTracker.Application.DTOs; 
using FinancialTracker.Domain.Shared;      

namespace FinancialTracker.Application.Interfaces
{
    public interface IWalletService
    {
        
        Task<Result<Guid>> CreateWalletAsync(WalletRequest request);
        Task<IEnumerable<WalletResponse>> GetWalletsAsync();      
        Task<Result<WalletResponse>> GetWalletByIdAsync(Guid id);
        Task<Result> UpdateWalletAsync(Guid id, WalletRequest request);
        Task<Result> DeleteWalletAsync(Guid id);
    }
}
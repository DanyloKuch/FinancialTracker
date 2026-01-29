using FinancialTracker.Domain.Models;

namespace FinancialTracker.Domain.Interfaces
{
    public interface IInvitationRepository
    {
        Task AddAsync(Invitation invitation);
        Task<Invitation?> GetByIdAsync(Guid id);
        Task<List<Invitation>> GetPendingByEmailAsync(string email);
        Task UpdateAsync(Invitation invitation);
    }
}
using FinancialTracker.Domain.Models;

namespace FinancialTracker.Domain.Interfaces
{
    public interface IInvitationRepository
    {
        Task AddAsync(Invitation invitation);
        Task<Invitation?> GetByIdAsync(Guid id);
        Task<List<Invitation>> GetSentByUserIdAsync(Guid userId);
        Task<List<Invitation>> GetReceivedByEmailAsync(string email);
        Task UpdateAsync(Invitation invitation);
        Task DeleteAsync(Invitation invitation);
        Task<bool> HasPendingInvitationAsync(Guid groupId, string email);
    }
}
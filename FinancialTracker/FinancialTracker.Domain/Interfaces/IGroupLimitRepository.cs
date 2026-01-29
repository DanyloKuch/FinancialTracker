using FinancialTracker.Domain.Models;

namespace FinancialTracker.Domain.Interfaces
{
    public interface IGroupLimitRepository
    {
        Task AddAsync(GroupCategoryLimit limit);
        Task UpdateAsync(GroupCategoryLimit limit);
        Task<GroupCategoryLimit?> GetByGroupAndCategoryAsync(Guid groupId, string categoryName);
        Task<List<GroupCategoryLimit>> GetByGroupIdAsync(Guid groupId);
    }
}
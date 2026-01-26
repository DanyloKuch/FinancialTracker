using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Domain.Interfaces
{
    public interface IGroupRepository
    {
        Task<Result<Guid>> CreateAsync(Group group, GroupMember initialMember);
        Task<Result<Group>> GetByIdAsync(Guid groupId, Guid userId);
        Task<Result<IReadOnlyList<Group>>> GetAllByUserIdAsync(Guid userId);
        Task<Result> DeleteGroupAsync(Guid groupId);
        Task<Result> RemoveMemberAsync(Guid groupId, Guid userId);
        Task<Result<Guid>> AddMemberAsync(GroupMember member);
        Task<bool> ExistsAsync(Guid groupId);
    }
}
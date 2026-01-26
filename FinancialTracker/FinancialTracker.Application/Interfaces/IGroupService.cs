using FinancialTracker.Application.DTOs;
using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Shared;
using System.Threading.Tasks;

namespace FinancialTracker.Application.Interfaces
{
    public interface IGroupService
    {
     
        Task<Result<Guid>> CreateGroupAsync(GroupRequest request);
        Task<Result<GroupResponse>> GetGroupByIdAsync(Guid groupId);
        Task<Result<IReadOnlyList<GroupResponse>>> GetAllUserGroupsAsync();
        Task<Result<Guid>> AddMemberInternalAsync(Guid groupId, Guid userId, GroupRole role);
        Task<Result> DeleteOrLeaveGroupAsync(Guid groupId);
    }
}
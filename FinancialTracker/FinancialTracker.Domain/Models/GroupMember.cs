using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Domain.Models
{
    public class GroupMember
    {
        public Guid Id { get; }
        public Guid GroupId { get; }
        public Guid UserId { get; }
        public GroupRole Role { get; }
        public DateTime JoinedAt { get; }

      
        private GroupMember(Guid id, Guid groupId, Guid userId, GroupRole role, DateTime joinedAt)
        {
            Id = id;
            GroupId = groupId;
            UserId = userId;
            Role = role;
            JoinedAt = joinedAt;
        }

        // Фабричний метод
        public static Result<GroupMember> Create(Guid id, Guid groupId, Guid userId, GroupRole role, DateTime joinedAt)
        {
            if (userId == Guid.Empty)
                return Result<GroupMember>.Failure("UserId cannot be empty.");

            return Result<GroupMember>.Success(new GroupMember(id, groupId, userId, role, joinedAt));
        }
    }
}
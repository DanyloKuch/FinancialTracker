using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Domain.Models
{
    public class GroupMember
    {
        public Guid Id { get; }
        public Guid GroupId { get; }
        public Guid UserId { get; }
        public string? Email { get; }
        public GroupRole Role { get; }
        public DateTime JoinedAt { get; }

      
        private GroupMember(Guid id, Guid groupId, Guid userId, string? email, GroupRole role, DateTime joinedAt)
        {
            Id = id;
            GroupId = groupId;
            UserId = userId;
            Email = email;
            Role = role;
            JoinedAt = joinedAt;
        }

        public static Result<GroupMember> Create(Guid id, Guid groupId, Guid userId, GroupRole role, DateTime joinedAt, string? email = null)
        {
            if (userId == Guid.Empty)
                return Result<GroupMember>.Failure("UserId cannot be empty.");

            return Result<GroupMember>.Success(new GroupMember(id, groupId, userId, email, role, joinedAt));
        }
    }
}
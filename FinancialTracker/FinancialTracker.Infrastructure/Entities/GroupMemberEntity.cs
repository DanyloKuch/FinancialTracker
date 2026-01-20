using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Infrastructure.Entities
{
    public class GroupMemberEntity
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public GroupRole Role { get; set; }
        public DateTime JoinedAt { get; set; }

        public GroupEntity Group { get; set; } = null!;
        public UserEntity User { get; set; } = null!;
    }
}

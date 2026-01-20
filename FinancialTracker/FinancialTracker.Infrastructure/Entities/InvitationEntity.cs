using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Infrastructure.Entities
{
    public class InvitationEntity
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid InviterId { get; set; }
        public string InviteeEmail { get; set; } = null!;
        public InvitationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public GroupEntity Group { get; set; } = null!;
        public UserEntity Inviter { get; set; } = null!;
    }
}

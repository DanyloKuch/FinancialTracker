using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Domain.Models
{
    public class Invitation
    {
        public Guid Id { get; private set; }
        public Guid GroupId { get; private set; }
        public Guid InviterId { get; private set; }
        public string InviteeEmail { get; private set; } = null!;
        public InvitationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Invitation(Guid id, Guid groupId, Guid inviterId, string inviteeEmail, InvitationStatus status, DateTime createdAt)
        {
            Id = id;
            GroupId = groupId;
            InviterId = inviterId;
            InviteeEmail = inviteeEmail;
            Status = status;
            CreatedAt = createdAt;
        }

        public static Result<Invitation> Create(Guid groupId, Guid inviterId, string inviteeEmail)
        {
            if (string.IsNullOrWhiteSpace(inviteeEmail))
                return Result<Invitation>.Failure("Email is required.");

            return Result<Invitation>.Success(new Invitation(
                Guid.NewGuid(), groupId, inviterId, inviteeEmail, InvitationStatus.Pending, DateTime.UtcNow));
        }

        public static Invitation Load(Guid id, Guid groupId, Guid inviterId, string inviteeEmail, InvitationStatus status, DateTime createdAt)
        {
            return new Invitation(id, groupId, inviterId, inviteeEmail, status, createdAt);
        }

        public Result Accept()
        {
            if (Status != InvitationStatus.Pending)
                return Result.Failure("Invitation is not pending.");

            Status = InvitationStatus.Accepted;
            return Result.Success();
        }

        public void Reject()
        {
            Status = InvitationStatus.Rejected;
        }
    }
}
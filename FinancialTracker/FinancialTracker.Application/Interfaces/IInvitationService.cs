using FinancialTracker.Application.DTOs;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Interfaces
{
    public interface IInvitationService
    {
        Task<Result<Guid>> InviteUserAsync(InviteUserRequest request);
        Task<Result> RespondToInvitationAsync(Guid invitationId, bool isAccepted);
        Task<Result> CancelInvitationAsync(Guid invitationId);
        Task<List<InvitationResponse>> GetMyReceivedInvitationsAsync();
        Task<List<InvitationResponse>> GetMySentInvitationsAsync();
    }
}
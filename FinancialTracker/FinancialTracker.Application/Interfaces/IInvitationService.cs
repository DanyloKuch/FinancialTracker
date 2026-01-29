using FinancialTracker.Application.DTOs;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Interfaces
{
    public interface IInvitationService
    {
        Task<Result<Guid>> InviteUserAsync(InviteUserRequest request);
        Task<Result> AcceptInvitationAsync(Guid invitationId);
        Task<List<InvitationResponse>> GetMyPendingInvitationsAsync();
    }
}
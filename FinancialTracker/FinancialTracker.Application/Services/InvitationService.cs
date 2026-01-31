using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Application.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IGroupRepository _groupRepository; 
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public InvitationService(
            IInvitationRepository invitationRepository,
            IGroupRepository groupRepository,
            ICurrentUserService currentUserService,
            IUserRepository userRepository)
        {
            _invitationRepository = invitationRepository;
            _groupRepository = groupRepository;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<Result<Guid>> InviteUserAsync(InviteUserRequest request)
        {
            var groupResult = await _groupRepository.GetByIdAsync(request.GroupId, _currentUserService.UserId);

            if (!groupResult.IsSuccess)
                return Result<Guid>.Failure("Group not found or you don't have access.");

            var group = groupResult.Value!;

            var inviteeId = await _userRepository.GetUserIdByEmailAsync(request.Email);

            if (inviteeId.HasValue)
            {
                if (group.Members.Any(m => m.UserId == inviteeId.Value))
                    return Result<Guid>.Failure("User is already a member.");
            }

            var invitationResult = Invitation.Create(request.GroupId, _currentUserService.UserId, request.Email);
            if (!invitationResult.IsSuccess) return Result<Guid>.Failure(invitationResult.Error!);

            await _invitationRepository.AddAsync(invitationResult.Value!);
            return Result<Guid>.Success(invitationResult.Value!.Id);
        }

        public async Task<Result> RespondToInvitationAsync(Guid invitationId, bool isAccepted)
        {
            var invitation = await _invitationRepository.GetByIdAsync(invitationId);
            if (invitation == null) return Result.Failure("Invitation not found.");

            var currentUserEmail = await _userRepository.GetUserEmailByIdAsync(_currentUserService.UserId);
            if (invitation.InviteeEmail != currentUserEmail)
                return Result.Failure("Access denied.");

            Result result;

            if (isAccepted)
            {
                result = invitation.Accept();
                if (!result.IsSuccess) return result;

                var newMemberResult = GroupMember.Create(Guid.NewGuid(), invitation.GroupId, _currentUserService.UserId, GroupRole.Member, DateTime.UtcNow);
                if (!newMemberResult.IsSuccess) return Result.Failure(newMemberResult.Error!);

                await _groupRepository.AddMemberAsync(newMemberResult.Value);
            }
            else
            {
                result = invitation.Reject();
                if (!result.IsSuccess) return result;
            }

            await _invitationRepository.UpdateAsync(invitation);
            return Result.Success();
        }

        public async Task<List<InvitationResponse>> GetMySentInvitationsAsync()
        {
            var invitations = await _invitationRepository.GetSentByUserIdAsync(_currentUserService.UserId);
            return invitations.Select(i => new InvitationResponse(i.Id, i.GroupId, i.InviteeEmail, i.Status.ToString())).ToList();
        }

        public async Task<List<InvitationResponse>> GetMyReceivedInvitationsAsync()
        {
            var email = await _userRepository.GetUserEmailByIdAsync(_currentUserService.UserId);
            if (string.IsNullOrEmpty(email)) return new List<InvitationResponse>();

            var invitations = await _invitationRepository.GetReceivedByEmailAsync(email);

            return invitations.Select(i => new InvitationResponse(i.Id, i.GroupId, i.InviterId.ToString(), i.Status.ToString())).ToList();
        }

        public async Task<Result> CancelInvitationAsync(Guid invitationId)
        {
            var invitation = await _invitationRepository.GetByIdAsync(invitationId);
            if (invitation == null) return Result.Failure("Invitation not found.");

            if (invitation.InviterId != _currentUserService.UserId)
                return Result.Failure("You can only cancel your own invitations.");

            await _invitationRepository.DeleteAsync(invitation);
            return Result.Success();
        }
    }
}
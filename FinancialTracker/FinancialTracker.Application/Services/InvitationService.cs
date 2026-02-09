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
                return Result<Guid>.Failure("Group not found or access denied.");

            var group = groupResult.Value;

            var isMember = await _groupRepository.IsMemberAsync(group.Id, request.Email);
            if (isMember)
            {
                return Result<Guid>.Failure("User is already a member of this group.");
            }

            var hasPending = await _invitationRepository.HasPendingInvitationAsync(group.Id, request.Email);
            if (hasPending)
            {
                return Result<Guid>.Failure("This user already has a pending invitation to this group.");
            }

            var invitationResult = Invitation.Create(
                group.Id,
                _currentUserService.UserId,
                request.Email
            );

            if (!invitationResult.IsSuccess)
                return Result<Guid>.Failure(invitationResult.Error!);

            await _invitationRepository.AddAsync(invitationResult.Value);

            return Result<Guid>.Success(invitationResult.Value.Id);
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
            var userId = _currentUserService.UserId;
            var invitations = await _invitationRepository.GetSentByUserIdAsync(userId);

            var myEmail = await _userRepository.GetUserEmailByIdAsync(userId);

            return invitations.Select(i => new InvitationResponse(
                i.Id,
                i.GroupId,
                myEmail,       
                i.InviteeEmail, 
                i.Status.ToString()
            )).ToList();
        }

        public async Task<List<InvitationResponse>> GetMyReceivedInvitationsAsync()
        {
            var userId = _currentUserService.UserId;
            var myEmail = await _userRepository.GetUserEmailByIdAsync(userId);

            if (string.IsNullOrEmpty(myEmail)) return new List<InvitationResponse>();

            var invitations = await _invitationRepository.GetReceivedByEmailAsync(myEmail);

            var responseList = new List<InvitationResponse>();

            foreach (var invitation in invitations)
            {
                var inviterEmail = await _userRepository.GetUserEmailByIdAsync(invitation.InviterId);

                responseList.Add(new InvitationResponse(
                    invitation.Id,
                    invitation.GroupId,
                    inviterEmail ?? "Unknown", 
                    invitation.InviteeEmail,
                    invitation.Status.ToString()
                ));
            }

            return responseList;
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
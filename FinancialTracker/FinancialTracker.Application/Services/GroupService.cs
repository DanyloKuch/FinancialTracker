using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ICurrentUserService _currentUserService;

        public GroupService(IGroupRepository groupRepository, ICurrentUserService currentUserService)
        {
            _groupRepository = groupRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> CreateGroupAsync(GroupRequest request)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
                return Result<Guid>.Failure("User not authorized");

            var groupId = Guid.NewGuid();


            var groupResult = Group.Create(
                groupId,
                userId,
                request.Name,
                request.BaseCurrency,
                request.TotalLimit,
                DateTime.UtcNow
            );

            if (groupResult.IsFailure)
                return Result<Guid>.Failure(groupResult.Error);


            var ownerResult = GroupMember.Create(
                Guid.NewGuid(),
                groupId,
                userId,
                GroupRole.Owner,
                DateTime.UtcNow
            );

            if (ownerResult.IsFailure)
                return Result<Guid>.Failure(ownerResult.Error);

            var result = await _groupRepository.CreateAsync(groupResult.Value, ownerResult.Value);

            return result;
        }

        public async Task<Result<GroupResponse>> GetGroupByIdAsync(Guid groupId)
        {
            var userId = _currentUserService.UserId;

           
            var groupResult = await _groupRepository.GetByIdAsync(groupId, userId);

            if (groupResult.IsFailure)
                return Result<GroupResponse>.Failure(groupResult.Error);

            var group = groupResult.Value;

  
            bool isOwner = group.OwnerId == userId;


            var membersDto = group.Members.Select(m => new GroupMemberResponse(
                m.Id,
                m.UserId,
                m.Email ?? "Unknown", 
                m.Role,
                m.JoinedAt
            )).ToList();

            
            var response = new GroupResponse(
                group.Id,
                group.Name,
                group.OwnerId,
                group.OwnerEmail ?? "Unknown", 
                group.BaseCurrency,
                group.TotalLimit,
                membersDto,
                group.CreatedAt,
                isOwner 
            );

            return Result<GroupResponse>.Success(response);
        }

        public async Task<Result<IReadOnlyList<GroupResponse>>> GetAllUserGroupsAsync()
        {
            var userId = _currentUserService.UserId;
            var groupsResult = await _groupRepository.GetAllByUserIdAsync(userId);

            if (groupsResult.IsFailure)
                return Result<IReadOnlyList<GroupResponse>>.Failure(groupsResult.Error);

            var responseList = groupsResult.Value.Select(group =>
            {
               
                bool isOwner = group.OwnerId == userId;

                var membersDto = group.Members.Select(m => new GroupMemberResponse(
                    m.Id,
                    m.UserId,
                    m.Email ?? "Unknown",
                    m.Role,
                    m.JoinedAt
                )).ToList();

                return new GroupResponse(
                    group.Id,
                    group.Name,
                    group.OwnerId,
                    group.OwnerEmail ?? "Unknown",
                    group.BaseCurrency,
                    group.TotalLimit,
                    membersDto,
                    group.CreatedAt,
                    isOwner
                );
            }).ToList();

            return Result<IReadOnlyList<GroupResponse>>.Success(responseList);
        }

        public async Task<Result<Guid>> AddMemberInternalAsync(Guid groupId, Guid userId, GroupRole role)
        {
            var groupExists = await _groupRepository.ExistsAsync(groupId);
            if (!groupExists)
                return Result<Guid>.Failure("Group not found.");

            var newMemberResult = GroupMember.Create(
                Guid.NewGuid(),
                groupId,
                userId,
                role,
                DateTime.UtcNow
            );

            if (newMemberResult.IsFailure)
                return Result<Guid>.Failure(newMemberResult.Error);

            return await _groupRepository.AddMemberAsync(newMemberResult.Value);
        }

        public async Task<Result> KickMemberAsync(Guid groupId, Guid memberId)
        {
            var currentUserId = _currentUserService.UserId;

            var groupResult = await _groupRepository.GetByIdAsync(groupId, currentUserId);
            if (groupResult.IsFailure)
                return Result.Failure("Group not found or access denied.");

            var group = groupResult.Value;

            if (group.OwnerId != currentUserId)
                return Result.Failure("Access denied. Only the owner can remove members.");

            if (memberId == currentUserId)
                return Result.Failure("You cannot kick yourself. Use 'Leave' or 'Delete Group' functionality.");

            var memberToRemove = group.Members.FirstOrDefault(m => m.UserId == memberId);
            if (memberToRemove == null)
                return Result.Failure("User is not a member of this group.");

            return await _groupRepository.RemoveMemberAsync(groupId, memberId);
        }

        public async Task<Result> LeaveGroupAsync(Guid groupId)
        {
            var currentUserId = _currentUserService.UserId;

            var groupResult = await _groupRepository.GetByIdAsync(groupId, currentUserId);
            if (groupResult.IsFailure)
                return Result.Failure("Group not found or you are not a member.");

            var group = groupResult.Value;

            if (group.OwnerId == currentUserId)
                return Result.Failure("Owner cannot leave the group. You must delete the group instead.");

            return await _groupRepository.RemoveMemberAsync(groupId, currentUserId);
        }

        public async Task<Result<GroupResponse>> UpdateGroupAsync(Guid groupId, GroupRequest request)
        {
            var userId = _currentUserService.UserId;

         
            var groupResult = await _groupRepository.GetByIdAsync(groupId, userId);

            if (groupResult.IsFailure)
                return Result<GroupResponse>.Failure(groupResult.Error);

            var group = groupResult.Value;


            if (group.OwnerId != userId)
                return Result<GroupResponse>.Failure("Access denied. Only owner can update group.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<GroupResponse>.Failure("Name cannot be empty");

            if (request.TotalLimit < 0)
                return Result<GroupResponse>.Failure("Limit cannot be negative");

        
            group.Name = request.Name;
            group.TotalLimit = request.TotalLimit;

            await _groupRepository.UpdateAsync(group);

    
            var membersDto = group.Members.Select(m => new GroupMemberResponse(
                m.Id,
                m.UserId,
                m.Email ?? "Unknown",
                m.Role,
                m.JoinedAt
            )).ToList();

            return Result<GroupResponse>.Success(new GroupResponse(
                group.Id,
                group.Name,
                group.OwnerId,
                group.OwnerEmail ?? "Unknown",
                group.BaseCurrency,
                group.TotalLimit,
                membersDto,
                group.CreatedAt,
                true 
            ));
        }

        public async Task<Result> DeleteGroupAsync(Guid groupId)
        {
            var currentUserId = _currentUserService.UserId;

            var groupResult = await _groupRepository.GetByIdAsync(groupId, currentUserId);
            if (groupResult.IsFailure)
                return Result.Failure("Group not found or access denied.");

            var group = groupResult.Value;

            if (group.OwnerId != currentUserId)
                return Result.Failure("Access denied. Only the owner can delete the group.");

            return await _groupRepository.DeleteGroupAsync(groupId);
        }
    }
}
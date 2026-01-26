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

            var membersDto = group.Members.Select(m => new GroupMemberResponse(
                m.Id,
                m.UserId,
                m.Role,
                m.JoinedAt
            )).ToList();

            var response = new GroupResponse(
                group.Id,
                group.Name,
                group.OwnerId,
                group.BaseCurrency,
                group.TotalLimit,
                membersDto,
                group.CreatedAt
            );

            return Result<GroupResponse>.Success(response);
        }

        public async Task<Result<IReadOnlyList<GroupResponse>>> GetAllUserGroupsAsync()
        {
            var userId = _currentUserService.UserId;
            var groupsResult = await _groupRepository.GetAllByUserIdAsync(userId);

            if (groupsResult.IsFailure)
                return Result<IReadOnlyList<GroupResponse>>.Failure(groupsResult.Error);

 
            var responseList = groupsResult.Value.Select(g => new GroupResponse(
                g.Id,
                g.Name,
                g.OwnerId,
                g.BaseCurrency,
                g.TotalLimit,
                g.Members.Select(m => new GroupMemberResponse(m.Id, m.UserId, m.Role, m.JoinedAt)).ToList(),
                g.CreatedAt
            )).ToList();

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
        public async Task<Result> DeleteOrLeaveGroupAsync(Guid groupId)
        {
            var userId = _currentUserService.UserId;

           
            var groupResult = await _groupRepository.GetByIdAsync(groupId, userId);

            if (groupResult.IsFailure)
                return Result.Failure("Group not found or you are not a member.");

            var group = groupResult.Value;

          
            if (group.OwnerId == userId)
            {
        
                return await _groupRepository.DeleteGroupAsync(groupId);
            }
            else
            {
                
                return await _groupRepository.RemoveMemberAsync(groupId, userId);
            }
        }
    }
}
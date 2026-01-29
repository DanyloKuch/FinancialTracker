using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Services
{
    public class GroupLimitService : IGroupLimitService
    {
        private readonly IGroupLimitRepository _limitRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ICurrentUserService _currentUserService;

        public GroupLimitService(IGroupLimitRepository limitRepository, IGroupRepository groupRepository, ICurrentUserService currentUserService)
        {
            _limitRepository = limitRepository;
            _groupRepository = groupRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> SetLimitAsync(Guid groupId, SetLimitRequest request)
        {
            // ВИПРАВЛЕННЯ: Шукаємо групу в контексті поточного юзера
            var groupResult = await _groupRepository.GetByIdAsync(groupId, _currentUserService.UserId);

            if (!groupResult.IsSuccess)
                return Result.Failure("Group not found or access denied.");

            var group = groupResult.Value!;

            // Перевірка на власника
            if (group.OwnerId != _currentUserService.UserId)
                return Result.Failure("Only the group owner can set limits.");

            var existingLimit = await _limitRepository.GetByGroupAndCategoryAsync(groupId, request.CategoryName);

            if (existingLimit != null)
            {
                existingLimit.UpdateAmount(request.LimitAmount);
                await _limitRepository.UpdateAsync(existingLimit);
            }
            else
            {
                var newLimitResult = GroupCategoryLimit.Create(groupId, request.CategoryName, request.LimitAmount);
                if (!newLimitResult.IsSuccess) return Result.Failure(newLimitResult.Error!);
                await _limitRepository.AddAsync(newLimitResult.Value!);
            }

            return Result.Success();
        }

        public async Task<List<LimitResponse>> GetLimitsAsync(Guid groupId)
        {
            // Перевіряємо, чи має юзер доступ до групи (чи є учасником)
            var groupResult = await _groupRepository.GetByIdAsync(groupId, _currentUserService.UserId);

            if (!groupResult.IsSuccess)
                return new List<LimitResponse>(); // Або кинути помилку, залежно від логіки фронтенду

            var limits = await _limitRepository.GetByGroupIdAsync(groupId);
            return limits.Select(l => new LimitResponse(l.Id, l.CategoryName, l.LimitAmount)).ToList();
        }
    }
}
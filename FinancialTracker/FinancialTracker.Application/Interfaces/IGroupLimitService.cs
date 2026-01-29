using FinancialTracker.Application.DTOs;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Interfaces
{
    public interface IGroupLimitService
    {
        Task<Result> SetLimitAsync(Guid groupId, SetLimitRequest request);
        Task<List<LimitResponse>> GetLimitsAsync(Guid groupId);
    }
}
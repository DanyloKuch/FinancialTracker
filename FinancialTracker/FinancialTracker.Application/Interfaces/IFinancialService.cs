using FinancialTracker.Application.DTOs;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Interfaces
{
    public interface IFinancialService
    {
        Task<Result<DashboardSummaryResponse>> GetDashboardSummaryAsync();
    }
}
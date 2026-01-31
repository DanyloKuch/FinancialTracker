namespace FinancialTracker.Application.DTOs
{
    public record LimitResponse(Guid Id, string CategoryName, decimal LimitAmount);
}

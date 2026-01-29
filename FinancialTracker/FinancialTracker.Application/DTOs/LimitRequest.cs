namespace FinancialTracker.Application.DTOs
{
    public record SetLimitRequest(string CategoryName, decimal LimitAmount);
}
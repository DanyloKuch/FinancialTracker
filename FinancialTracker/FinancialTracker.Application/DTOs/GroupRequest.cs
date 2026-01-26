namespace FinancialTracker.Application.DTOs
{
    public record GroupRequest(
        string Name,
        string BaseCurrency,
        decimal? TotalLimit
    );
}
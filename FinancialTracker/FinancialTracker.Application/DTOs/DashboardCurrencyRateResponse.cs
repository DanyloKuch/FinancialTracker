namespace FinancialTracker.Application.DTOs
{
    public record DashboardCurrencyRateResponse(
        string Code,    
        decimal Rate,    
        DateTime UpdatedAt
    );
}

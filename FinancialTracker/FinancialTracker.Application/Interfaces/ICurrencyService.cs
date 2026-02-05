namespace FinancialTracker.Application.Interfaces
{
    public interface ICurrencyService
    {
        decimal UsdRate { get; }
        decimal EurRate { get; }
        DateTime LastUpdatedAt { get; }

       
        Task RefreshRatesAsync();
    }
}
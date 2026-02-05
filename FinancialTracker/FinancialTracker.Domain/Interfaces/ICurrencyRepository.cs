using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Domain.Interfaces
{
    public interface ICurrencyRepository
    {
        Task<List<CurrencyRate>> GetAllRatesAsync();
        Task<Result> UpsertRatesAsync(List<CurrencyRate> rates);
    }
}
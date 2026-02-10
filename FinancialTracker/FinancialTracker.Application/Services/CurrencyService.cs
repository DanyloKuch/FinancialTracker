using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FinancialTracker.Application.Services
{
  
    public class CurrencyService : ICurrencyService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CurrencyService> _logger;


        private decimal _usdRate;
        private decimal _eurRate;
        private DateTime _lastUpdatedAt;

        public decimal UsdRate => _usdRate;
        public decimal EurRate => _eurRate;
        public DateTime LastUpdatedAt => _lastUpdatedAt;

        public CurrencyService(IServiceScopeFactory scopeFactory, ILogger<CurrencyService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task RefreshRatesAsync()
        {
            
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

            var rates = await repository.GetAllRatesAsync();

            var usd = rates.FirstOrDefault(r => r.Code == "USD");
            var eur = rates.FirstOrDefault(r => r.Code == "EUR");

            if (usd != null) _usdRate = usd.Rate;
            if (eur != null) _eurRate = eur.Rate;

            if (usd != null) _lastUpdatedAt = usd.UpdatedAt;
        }


        public decimal GetExchangeRate(string fromCode, string toCode)
        {
            if (fromCode == toCode) return 1m;

            decimal fromRate = GetRateToUah(fromCode);
            decimal toRate = GetRateToUah(toCode);

            if (toRate == 0) return 0; 

            return fromRate / toRate;
        }


        private decimal GetRateToUah(string code)
        {
            return code.ToUpper() switch
            {
                "UAH" => 1m,
                "USD" => _usdRate,
                "EUR" => _eurRate,
                _ => throw new Exception($"Exchange rate for currency {code} not found")
            };
        }
    }
}
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
    }
}
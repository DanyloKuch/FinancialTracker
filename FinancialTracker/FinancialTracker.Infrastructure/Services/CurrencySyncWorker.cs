using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http; 
using System.Net.Http.Json; 
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Infrastructure.External.Monobank;

namespace FinancialTracker.Infrastructure.Services
{
    public class CurrencySyncWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CurrencySyncWorker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICurrencyService _currencyService;

        
        private const int UPDATE_INTERVAL_MINUTES = 60;
        private const int DATA_FRESHNESS_HOURS = 3;

        public CurrencySyncWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<CurrencySyncWorker> logger,
            IHttpClientFactory httpClientFactory,
            ICurrencyService currencyService)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _currencyService = currencyService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _currencyService.RefreshRatesAsync();
                _logger.LogInformation("Currency rates loaded from DB into memory.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing rates during startup.");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessCurrencySyncAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while syncing currencies.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(UPDATE_INTERVAL_MINUTES), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task ProcessCurrencySyncAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

            var existingRates = await repository.GetAllRatesAsync();
            var needsUpdate = false;

            if (!existingRates.Any())
            {
                needsUpdate = true;
            }
            else
            {
                var oldestUpdate = existingRates.Min(r => r.UpdatedAt);
                var hoursPassed = (DateTime.UtcNow - oldestUpdate).TotalHours;

                if (hoursPassed >= DATA_FRESHNESS_HOURS)
                {
                    needsUpdate = true;
                }
            }

            if (!needsUpdate)
            {
                _logger.LogInformation("Currency rates are up to date (fresh enough).");
                return;
            }

            _logger.LogInformation("Fetching fresh rates from Monobank API...");

            try
            {
                var client = _httpClientFactory.CreateClient("Monobank");
                var monoRates = await client.GetFromJsonAsync<List<MonobankCurrencyDto>>("/bank/currency", stoppingToken);

                if (monoRates == null) return;

                var newRates = new List<CurrencyRate>();

                var usdInfo = monoRates.FirstOrDefault(r => r.CurrencyCodeA == 840 && r.CurrencyCodeB == 980);
                var eurInfo = monoRates.FirstOrDefault(r => r.CurrencyCodeA == 978 && r.CurrencyCodeB == 980);

                if (usdInfo != null)
                {
                    var rate = usdInfo.RateSell > 0 ? usdInfo.RateSell : usdInfo.RateCross;
                    newRates.Add(CurrencyRate.Create("USD", rate));
                }

                if (eurInfo != null)
                {
                    var rate = eurInfo.RateSell > 0 ? eurInfo.RateSell : eurInfo.RateCross;
                    newRates.Add(CurrencyRate.Create("EUR", rate));
                }

      
                if (newRates.Any())
                {
                 
                    await repository.UpsertRatesAsync(newRates);
                    _logger.LogInformation("Currency rates updated in Database.");

                    await _currencyService.RefreshRatesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch from Monobank API. App will use old data from DB.");
               
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http; // Для IHttpClientFactory
using System.Net.Http.Json; // ВАЖНО: Для GetFromJsonAsync
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting; // Для BackgroundService
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

// Импорты из других слоев (теперь они должны быть видны)
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;

// Импорт DTO из соседней папки Infrastructure
using FinancialTracker.Infrastructure.External.Monobank;

namespace FinancialTracker.Infrastructure.Services
{
    public class CurrencySyncWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CurrencySyncWorker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICurrencyService _currencyService; // Наш Singleton-сервис (кэш в памяти)

        // Настройки частоты обновлений
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
            // 1. При старте пытаемся сразу загрузить данные в память из БД
            try
            {
                // Тут не нужен Scope, так как _currencyService - Singleton (или Scoped, созданный в DI)
                // Но внутри RefreshRatesAsync он сам создаст Scope для доступа к репозиторию
                await _currencyService.RefreshRatesAsync();
                _logger.LogInformation("Currency rates loaded from DB into memory.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing rates during startup.");
            }

            // 2. Запускаем бесконечный цикл
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

                // Ждем 1 час перед следующей попыткой
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(UPDATE_INTERVAL_MINUTES), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Нормальное завершение при остановке сервера
                    break;
                }
            }
        }

        private async Task ProcessCurrencySyncAsync(CancellationToken stoppingToken)
        {
            // BackgroundService - это Singleton, а DbContext - Scoped.
            // Поэтому мы ОБЯЗАНЫ создавать scope вручную.
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

            // --- ШАГ 1: Проверяем, нужно ли обновление ---
            var existingRates = await repository.GetAllRatesAsync();
            var needsUpdate = false;

            if (!existingRates.Any())
            {
                needsUpdate = true; // База пустая
            }
            else
            {
                // Проверяем дату самого старого обновления
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

            // --- ШАГ 2: Запрашиваем Монобанк ---
            _logger.LogInformation("Fetching fresh rates from Monobank API...");

            try
            {
                var client = _httpClientFactory.CreateClient("Monobank");
                // Запрос к API
                var monoRates = await client.GetFromJsonAsync<List<MonobankCurrencyDto>>("/bank/currency", stoppingToken);

                if (monoRates == null) return;

                var newRates = new List<CurrencyRate>();

                // Коды валют (ISO 4217): 840=USD, 978=EUR, 980=UAH
                var usdInfo = monoRates.FirstOrDefault(r => r.CurrencyCodeA == 840 && r.CurrencyCodeB == 980);
                var eurInfo = monoRates.FirstOrDefault(r => r.CurrencyCodeA == 978 && r.CurrencyCodeB == 980);

                if (usdInfo != null)
                {
                    // Берем курс продажи (RateSell) или кросс-курс, если продажи нет
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
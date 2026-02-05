using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Infrastructure.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly FinancialTrackerDbContext _context;

        public CurrencyRepository(FinancialTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<List<CurrencyRate>> GetAllRatesAsync()
        {
            var entities = await _context.CurrencyCache.AsNoTracking().ToListAsync();
            return entities.Select(e => CurrencyRate.FromEntity(e.CurrencyCode, e.RateToUah, e.UpdatedAt)).ToList();
        }

        public async Task<Result> UpsertRatesAsync(List<CurrencyRate> rates)
        {
           
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var rateModel in rates)
                {
                    var entity = await _context.CurrencyCache
                        .FirstOrDefaultAsync(c => c.CurrencyCode == rateModel.Code);

                    if (entity == null)
                    {
                       
                        entity = new CurrencyCacheEntity
                        {
                            CurrencyCode = rateModel.Code,
                            RateToUah = rateModel.Rate,
                            UpdatedAt = rateModel.UpdatedAt
                        };
                        await _context.CurrencyCache.AddAsync(entity);
                    }
                    else
                    {
                       
                        entity.RateToUah = rateModel.Rate;
                        entity.UpdatedAt = rateModel.UpdatedAt;
                        _context.CurrencyCache.Update(entity);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure($"Failed to upsert rates: {ex.Message}");
            }
        }
    }
}
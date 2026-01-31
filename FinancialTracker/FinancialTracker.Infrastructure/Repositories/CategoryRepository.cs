using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FinancialTrackerDbContext _context;

        public CategoryRepository(FinancialTrackerDbContext context)
        {
            _context = context;
        }

        private Category MapToDomain(CategoryEntity entity)
        {
            return Category.Load(entity.Id, entity.Name, entity.UserId, entity.IsArchived, entity.TotalLimit);
        }

        public async Task<Category?> GetByIdAsync(Guid categoryId, Guid userId)
        {
            var entity = await _context.Categories
                .AsNoTracking() 
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId); 

            return entity == null ? null : MapToDomain(entity);
        }

        public async Task<List<Category>> GetAllAsync(Guid userId)
        {
            var entities = await _context.Categories
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return entities.Select(MapToDomain).ToList();
        }

        public async Task AddAsync(Category category)
        {
            var entity = new CategoryEntity
            {
                Id = category.Id,
                Name = category.Name,
                UserId = category.UserId,
                IsArchived = category.IsArchived,
                TotalLimit = category.TotalLimit
            };

            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var entity = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == category.Id && c.UserId == category.UserId);

            if (entity != null)
            {
                entity.Name = category.Name;
                entity.IsArchived = category.IsArchived;
                entity.TotalLimit = category.TotalLimit;
                _context.Categories.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid categoryId, Guid userId)
        {
            var entity = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

            if (entity != null)
            {
                _context.Categories.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
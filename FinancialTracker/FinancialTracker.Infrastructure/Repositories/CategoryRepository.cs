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

        // Mapping (Read): Entity -> Domain Model
        private Category MapToDomain(CategoryEntity entity)
        {
            // Використовуємо фабричний метод або рефлексію, якщо фабрика занадто сувора для читання з БД.
            // Тут припустимо, що дані в БД валідні.
            var result = Category.Create(entity.Id, entity.Name, entity.UserId);
            // У реальному проекті для відновлення з БД часто використовують окремий конструктор або AutoMapper,
            // щоб уникнути повторної бізнес-валідації. Але тут використаємо Create.
            return result.Value!;
        }

        public async Task<Category?> GetByIdAsync(Guid categoryId, Guid userId)
        {
            var entity = await _context.Categories
                .AsNoTracking() // Optimization
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId); // Secure Querying

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
            // Mapping (Write): Domain -> Entity
            var entity = new CategoryEntity
            {
                Id = category.Id,
                Name = category.Name,
                UserId = category.UserId,
                IsArchived = false
            };

            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            // Mapping (Write): Update existing entity
            var entity = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == category.Id && c.UserId == category.UserId);

            if (entity != null)
            {
                entity.Name = category.Name;
                _context.Categories.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid categoryId, Guid userId)
        {
            // Secure Querying for delete
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
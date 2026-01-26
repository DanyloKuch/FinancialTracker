using FinancialTracker.Domain.Models;

namespace FinancialTracker.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid categoryId, Guid userId);
        Task<List<Category>> GetAllAsync(Guid userId);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Guid categoryId, Guid userId);
    }
}

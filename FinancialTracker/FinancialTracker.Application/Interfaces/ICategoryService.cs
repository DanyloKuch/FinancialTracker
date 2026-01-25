using FinancialTracker.Application.DTOs;
using FinancialTracker.Domain.Models;

namespace FinancialTracker.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetUserCategoriesAsync();
        Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id);
        Task<Result<Guid>> CreateCategoryAsync(CategoryRequest request);
        Task<Result<CategoryResponse>> UpdateCategoryAsync(Guid id, CategoryRequest request);
        Task<Result> DeleteCategoryAsync(Guid id);
    }
}

using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;

namespace FinancialTracker.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public CategoryService(ICategoryRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        private Guid CurrentUserId => _currentUserService.UserId;

        public async Task<List<CategoryResponse>> GetUserCategoriesAsync()
        {
            var categories = await _repository.GetAllAsync(CurrentUserId);
            // Mapping: Domain -> DTO
            return categories.Select(c => new CategoryResponse(c.Id, c.Name)).ToList();
        }

        public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id)
        {
            var category = await _repository.GetByIdAsync(id, CurrentUserId);
            if (category == null)
                return Result<CategoryResponse>.Failure("Category not found or access denied");

            return Result<CategoryResponse>.Success(new CategoryResponse(category.Id, category.Name));
        }

        public async Task<Result<Guid>> CreateCategoryAsync(CategoryRequest request)
        {
            // Factory Method: DTO -> Domain Model
            var result = Category.Create(Guid.NewGuid(), request.Name, CurrentUserId);

            if (!result.IsSuccess)
                return Result<Guid>.Failure(result.Error!);

            await _repository.AddAsync(result.Value!);
            return Result<Guid>.Success(result.Value!.Id);
        }

        public async Task<Result> UpdateCategoryAsync(Guid id, CategoryRequest request)
        {
            var category = await _repository.GetByIdAsync(id, CurrentUserId);
            if (category == null)
                return Result.Failure("Category not found or access denied");

            // Оновлюємо доменну модель
            try
            {
                category.UpdateName(request.Name);
            }
            catch (ArgumentException)
            {
                return Result.Failure("Invalid category name");
            }

            await _repository.UpdateAsync(category);
            return Result.Success();
        }

        public async Task<Result> DeleteCategoryAsync(Guid id)
        {
            // Перевірка існування перед видаленням (або можна довірити це репозиторію)
            var category = await _repository.GetByIdAsync(id, CurrentUserId);
            if (category == null)
                return Result.Failure("Category not found or access denied");

            await _repository.DeleteAsync(id, CurrentUserId);
            return Result.Success();
        }
    }
}
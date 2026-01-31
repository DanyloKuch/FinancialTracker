using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
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
            return categories.Select(c => new CategoryResponse(c.Id, c.Name, c.IsArchived, c.TotalLimit)).ToList();
        }

        public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id)
        {
            var category = await _repository.GetByIdAsync(id, CurrentUserId);
            if (category == null) return Result<CategoryResponse>.Failure("...");

            return Result<CategoryResponse>.Success(new CategoryResponse(category.Id, category.Name, category.IsArchived, category.TotalLimit));
        }

        public async Task<Result<Guid>> CreateCategoryAsync(CategoryRequest request)
        {
            var result = Category.Create(Guid.NewGuid(), request.Name, CurrentUserId, request.TotalLimit);

            if (!result.IsSuccess)
                return Result<Guid>.Failure(result.Error!);

            await _repository.AddAsync(result.Value!);
            return Result<Guid>.Success(result.Value!.Id);
        }

        public async Task<Result<CategoryResponse>> UpdateCategoryAsync(Guid id, CategoryRequest request)
        {
            var category = await _repository.GetByIdAsync(id, CurrentUserId);

            if (category == null)
                return Result<CategoryResponse>.Failure("Category not found or access denied");

            var updateResult = category.Update(request.Name, request.IsArchived, request.TotalLimit);

            if (!updateResult.IsSuccess)
            {
                return Result<CategoryResponse>.Failure(updateResult.Error!);
            }

            await _repository.UpdateAsync(category);

            var response = new CategoryResponse(category.Id, category.Name, category.IsArchived, category.TotalLimit);

            return Result<CategoryResponse>.Success(response);
        }

        public async Task<Result> DeleteCategoryAsync(Guid id)
        {
            var category = await _repository.GetByIdAsync(id, CurrentUserId);
            if (category == null)
                return Result.Failure("Category not found or access denied");

            await _repository.DeleteAsync(id, CurrentUserId);
            return Result.Success();
        }
    }
}
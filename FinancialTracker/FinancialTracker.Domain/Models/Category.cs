using FinancialTracker.Domain.Shared;
namespace FinancialTracker.Domain.Models
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public bool IsArchived { get; private set; }
        public decimal TotalLimit { get; private set; }

        private Category(Guid id, string name, Guid userId, bool isArchived, decimal totalLimit)
        {
            Id = id;
            Name = name;
            UserId = userId;
            IsArchived = isArchived;
            TotalLimit = totalLimit;
        }

        public static Result<Category> Create(Guid id, string name, Guid userId, decimal totalLimit)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Category>.Failure("Category name cannot be empty.");

            if (name.Length > 50)
                return Result<Category>.Failure("Category name cannot exceed 50 characters.");

            if (id == Guid.Empty)
                return Result<Category>.Failure("Category ID is invalid.");

            if (userId == Guid.Empty)
                return Result<Category>.Failure("User ID is invalid.");

            if (totalLimit < 0)
                return Result<Category>.Failure("Limit cannot be negative.");

            return Result<Category>.Success(new Category(id, name, userId, false, totalLimit));
        }

        public static Category Load(Guid id, string name, Guid userId, bool isArchived, decimal totalLimit)
        {
            return new Category(id, name, userId, isArchived, totalLimit);
        }

        public Result Update(string name, bool isArchived, decimal totalLimit)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Invalid name");

            if (totalLimit < 0)
                return Result.Failure("Limit cannot be negative.");

            Name = name;
            IsArchived = isArchived;
            TotalLimit = totalLimit;

            return Result.Success();
        }
    }
}
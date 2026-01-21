
namespace FinancialTracker.Domain.Models
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public Guid UserId { get; private set; }

        // Приватний конструктор для EF Core та Фабричного методу
        private Category(Guid id, string name, Guid userId)
        {
            Id = id;
            Name = name;
            UserId = userId;
        }

        // Factory Method з валідацією
        public static Result<Category> Create(Guid id, string name, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Category>.Failure("Category name cannot be empty.");

            if (name.Length > 50)
                return Result<Category>.Failure("Category name cannot exceed 50 characters.");

            if (id == Guid.Empty)
                return Result<Category>.Failure("Category ID is invalid.");

            if (userId == Guid.Empty)
                return Result<Category>.Failure("User ID is invalid.");

            return Result<Category>.Success(new Category(id, name, userId));
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) || newName.Length > 50)
                throw new ArgumentException("Invalid name");

            Name = newName;
        }
    }
}
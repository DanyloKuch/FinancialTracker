
namespace FinancialTracker.Domain.Models
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public bool IsArchived { get; private set; }

        private Category(Guid id, string name, Guid userId, bool isArchived)
        {
            Id = id;
            Name = name;
            UserId = userId;
            IsArchived = isArchived;
        }

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

            return Result<Category>.Success(new Category(id, name, userId, false));
        }

        public static Category Load(Guid id, string name, Guid userId, bool isArchived)
        {
            return new Category(id, name, userId, isArchived);
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) || newName.Length > 50)
                throw new ArgumentException("Invalid name");

            Name = newName;
        }

        public void SetArchived(bool isArchived)
        {
            IsArchived = isArchived;
        }
    }
}
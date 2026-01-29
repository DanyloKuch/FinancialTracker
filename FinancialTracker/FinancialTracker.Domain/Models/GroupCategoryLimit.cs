using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Domain.Models
{
    public class GroupCategoryLimit
    {
        public Guid Id { get; private set; }
        public Guid GroupId { get; private set; }
        public string CategoryName { get; private set; } = null!;
        public decimal LimitAmount { get; private set; }

        private GroupCategoryLimit(Guid id, Guid groupId, string categoryName, decimal limitAmount)
        {
            Id = id;
            GroupId = groupId;
            CategoryName = categoryName;
            LimitAmount = limitAmount;
        }

        public static Result<GroupCategoryLimit> Create(Guid groupId, string categoryName, decimal limitAmount)
        {
            if (limitAmount < 0)
                return Result<GroupCategoryLimit>.Failure("Limit cannot be negative.");

            return Result<GroupCategoryLimit>.Success(
                new GroupCategoryLimit(Guid.NewGuid(), groupId, categoryName, limitAmount));
        }

        public static GroupCategoryLimit Load(Guid id, Guid groupId, string categoryName, decimal limitAmount)
        {
            return new GroupCategoryLimit(id, groupId, categoryName, limitAmount);
        }

        public void UpdateAmount(decimal newAmount)
        {
            if (newAmount < 0) throw new ArgumentException("Limit cannot be negative");
            LimitAmount = newAmount;
        }
    }
}
namespace FinancialTracker.Infrastructure.Entities
{
    public class GroupCategoryLimitEntity
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal LimitAmount { get; set; }
        public GroupEntity Group { get; set; } = null!;
    }
}

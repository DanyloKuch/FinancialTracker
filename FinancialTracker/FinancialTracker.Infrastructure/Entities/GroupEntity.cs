namespace FinancialTracker.Infrastructure.Entities
{
    public class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid OwnerId { get; set; }
        public string BaseCurrency { get; set; } = "UAH";
        public DateTime CreatedAt { get; set; }

        public UserEntity Owner { get; set; } = null!;
        public ICollection<GroupMemberEntity> Members { get; set; } = new List<GroupMemberEntity>();
        public ICollection<InvitationEntity> Invitations { get; set; } = new List<InvitationEntity>();
        public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
        public ICollection<GroupCategoryLimitEntity> Limits { get; set; } = new List<GroupCategoryLimitEntity>();
    }
}

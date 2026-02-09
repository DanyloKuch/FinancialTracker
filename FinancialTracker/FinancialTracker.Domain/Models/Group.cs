using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Domain.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? TotalLimit { get; set; }
        public string BaseCurrency { get; set; }
        public Guid OwnerId { get; set; }
        public string? OwnerEmail { get; private set; }
        public DateTime CreatedAt { get; set; }

        private readonly List<GroupMember> _members;
        public IReadOnlyList<GroupMember> Members => _members.AsReadOnly();

        private Group(Guid id, Guid ownerId, string? ownerEmail, string name, string baseCurrency, decimal? totalLimit, DateTime createdAt, List<GroupMember> members)
        {
            Id = id;
            OwnerId = ownerId;
            OwnerEmail = ownerEmail;
            Name = name;
            BaseCurrency = baseCurrency;
            TotalLimit = totalLimit;
            CreatedAt = createdAt;
            _members = members;
        }

        public static Result<Group> Create(Guid id, Guid ownerId, string name, string baseCurrency, decimal? totalLimit, DateTime createdAt, List<GroupMember>? members = null, string? ownerEmail = null)
        {
            
            if (string.IsNullOrWhiteSpace(name))
                return Result<Group>.Failure("Group name cannot be empty.");

            if (name.Length > 100)
                return Result<Group>.Failure("Group name cannot exceed 100 characters.");

            if (totalLimit.HasValue && totalLimit.Value < 0)
                return Result<Group>.Failure("Total limit cannot be negative.");

            var groupMembers = members ?? new List<GroupMember>();

            var group = new Group(id, ownerId, ownerEmail, name, baseCurrency, totalLimit, createdAt, groupMembers);

            return Result<Group>.Success(group);
        }
    }
}
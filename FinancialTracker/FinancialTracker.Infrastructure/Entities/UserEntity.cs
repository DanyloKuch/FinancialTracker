using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker.Infrastructure.Entities
{
    public class UserEntity : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; }

        public ICollection<GroupEntity> OwnedGroups { get; set; } = new List<GroupEntity>();
        public ICollection<GroupMemberEntity> Memberships { get; set; } = new List<GroupMemberEntity>();
        public ICollection<WalletEntity> Wallets { get; set; } = new List<WalletEntity>();
        public ICollection<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();
        public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
        public ICollection<InvitationEntity> SentInvitations { get; set; } = new List<InvitationEntity>();
    }
}

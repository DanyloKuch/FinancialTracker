using FinancialTracker.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Infrastructure
{
    public class FinancialTrackerDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {
        public FinancialTrackerDbContext(DbContextOptions<FinancialTrackerDbContext> options)
        : base(options) 
        { 
        }

        public DbSet<GroupEntity> Groups => Set<GroupEntity>();
        public DbSet<GroupMemberEntity> GroupMembers => Set<GroupMemberEntity>();
        public DbSet<InvitationEntity> Invitations => Set<InvitationEntity>();
        public DbSet<WalletEntity> Wallets => Set<WalletEntity>();
        public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
        public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();
        public DbSet<GroupCategoryLimitEntity> GroupCategoryLimits => Set<GroupCategoryLimitEntity>();
        public DbSet<CurrencyCacheEntity> CurrencyCache => Set<CurrencyCacheEntity>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 
            builder.ApplyConfigurationsFromAssembly(typeof(FinancialTrackerDbContext).Assembly);
        }

    }
}

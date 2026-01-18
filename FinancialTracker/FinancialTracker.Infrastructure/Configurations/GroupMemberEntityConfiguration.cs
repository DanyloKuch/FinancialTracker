using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialTracker.Infrastructure.Configurations
{
    public class GroupMemberEntityConfiguration : IEntityTypeConfiguration<GroupMemberEntity>
    {
        public void Configure(EntityTypeBuilder<GroupMemberEntity> builder)
        {
            builder.ToTable("GroupMembers");
            builder.HasKey(gm => gm.Id);

            builder.HasIndex(gm => new { gm.GroupId, gm.UserId }).IsUnique();

            builder.Property(gm => gm.Role)
                .HasConversion<string>();

            builder.HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(gm => gm.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

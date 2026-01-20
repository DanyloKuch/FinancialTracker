using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialTracker.Infrastructure.Configurations
{
    public class GroupCategoryLimitEntityConfiguration : IEntityTypeConfiguration<GroupCategoryLimitEntity>
    {
        public void Configure(EntityTypeBuilder<GroupCategoryLimitEntity> builder)
        {
            builder.ToTable("GroupCategoryLimits");

            builder.HasKey(l => l.Id);
            builder.Property(l => l.CategoryName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.LimitAmount)
                .HasPrecision(18, 2);

            builder.HasOne(l => l.Group)
                   .WithMany(g => g.Limits)
                   .HasForeignKey(l => l.GroupId);
        }
    }
}

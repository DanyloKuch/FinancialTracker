using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialTracker.Infrastructure.Configurations
{
    public class GroupEntityConfiguration : IEntityTypeConfiguration<GroupEntity>
    {
        public void Configure(EntityTypeBuilder<GroupEntity> builder)
        {
            builder.ToTable("Groups");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.BaseCurrency)
                .HasMaxLength(3)
                .HasDefaultValue("UAH");
            
            builder.Property(g => g.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.Property(g => g.TotalLimit)
                .HasPrecision(18, 2);


            builder.HasOne(g => g.Owner)
                   .WithMany(u => u.OwnedGroups)
                   .HasForeignKey(g => g.OwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(g => g.Members)
                   .WithOne(m => m.Group)
                   .HasForeignKey(m => m.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(g => g.Invitations)
                   .WithOne(i => i.Group)
                   .HasForeignKey(i => i.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(g => g.Transactions)
                   .WithOne(tr => tr.Group)
                   .HasForeignKey(tr => tr.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialTracker.Infrastructure.Configurations
{
    public class InvitationEntityConfiguration : IEntityTypeConfiguration<InvitationEntity>
    {
        public void Configure(EntityTypeBuilder<InvitationEntity> builder)
        {
            builder.ToTable("Invitations");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.InviteeEmail)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(i => i.Status)
                .HasConversion<string>();

            builder.Property(i => i.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(i => i.Inviter)
                   .WithMany(u => u.SentInvitations)
                   .HasForeignKey(i => i.InviterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Group)
                   .WithMany(g => g.Invitations)
                   .HasForeignKey(i => i.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

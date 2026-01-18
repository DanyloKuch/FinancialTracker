using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialTracker.Infrastructure.Configurations
{
    public class WalletEntityConfiguration : IEntityTypeConfiguration<WalletEntity>
    {
        public void Configure(EntityTypeBuilder<WalletEntity> builder)
        {
            builder.ToTable("Wallets");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(w => w.Balance)
                .HasPrecision(18, 2);
            
            builder.Property(w => w.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(w => w.Type)
                .IsRequired()
                .HasMaxLength(20); 
            
            builder.Property(w => w.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(w => w.User)
                .WithMany(u => u.Wallets)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.Transactions)
                .WithOne(t => t.Wallet)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

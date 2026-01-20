using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialTracker.Infrastructure.Configurations
{
    public class TransactionEntityConfiguration : IEntityTypeConfiguration<TransactionEntity>
    {
        public void Configure(EntityTypeBuilder<TransactionEntity> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(t => t.ExchangeRate).HasPrecision(18, 4);
            builder.Property(t => t.Commission)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(t => t.Type)
                .HasConversion<string>()
                .HasMaxLength(20);


            builder.HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.TargetWallet)
                .WithMany()
                .HasForeignKey(t => t.TargetWalletId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Group)
                .WithMany(g => g.Transactions)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}

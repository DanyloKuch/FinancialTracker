using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialTracker.Infrastructure.Configurations
{
    public class CurrencyCacheEntityConfiguration : IEntityTypeConfiguration<CurrencyCacheEntity>
    {
        public void Configure(EntityTypeBuilder<CurrencyCacheEntity> builder)
        {
            builder.ToTable("CurrencyCaches");
            builder.HasKey(c => c.CurrencyCode);
            
            builder.Property(c => c.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(c => c.RateToUah)
                .HasPrecision(18, 4);

            builder.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}

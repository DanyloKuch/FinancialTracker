using System.ComponentModel.DataAnnotations;

namespace FinancialTracker.Infrastructure.Entities
{
    public class CurrencyCacheEntity
    {
        [Key]
        public string CurrencyCode { get; set; } = null!; 
        public decimal RateToUah { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

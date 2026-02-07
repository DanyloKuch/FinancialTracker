namespace FinancialTracker.Web.Models
{
    public class ExchangeRateDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

namespace FinancialTracker.Domain.Models
{
    public class CurrencyRate
    {
        public string Code { get; private set; }
        public decimal Rate { get; private set; }
        public DateTime UpdatedAt { get; private set; }

 
        private CurrencyRate(string code, decimal rate, DateTime updatedAt)
        {
            Code = code;
            Rate = rate;
            UpdatedAt = updatedAt;
        }

        public static CurrencyRate Create(string code, decimal rate)
        {
          
            return new CurrencyRate(code, rate, DateTime.UtcNow);
        }

        
        public static CurrencyRate FromEntity(string code, decimal rate, DateTime updatedAt)
        {
            return new CurrencyRate(code, rate, updatedAt);
        }
    }
}
namespace FinancialTracker.Web.Models
{
    public class WalletCreateRequest
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "General";
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; } = "UAH";
    }
}
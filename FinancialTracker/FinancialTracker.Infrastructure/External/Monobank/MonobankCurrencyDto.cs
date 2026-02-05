using System.Text.Json.Serialization;

namespace FinancialTracker.Infrastructure.External.Monobank
{
    public class MonobankCurrencyDto
    {
        [JsonPropertyName("currencyCodeA")]
        public int CurrencyCodeA { get; set; } 

        [JsonPropertyName("currencyCodeB")]
        public int CurrencyCodeB { get; set; } 

        [JsonPropertyName("rateBuy")]
        public decimal RateBuy { get; set; }

        [JsonPropertyName("rateSell")]
        public decimal RateSell { get; set; }

        [JsonPropertyName("rateCross")]
        public decimal RateCross { get; set; }
    }
}
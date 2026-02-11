using System.Text.Json.Serialization;

namespace FinancialTracker.Web.Models
{
    public class TransactionListResponse
    {
        [JsonPropertyName("items")]
        public List<TransactionDto> Items { get; set; } = new();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}
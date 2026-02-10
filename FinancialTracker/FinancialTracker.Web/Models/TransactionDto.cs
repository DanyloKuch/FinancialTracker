using System.Text.Json.Serialization;

namespace FinancialTracker.Web.Models
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public int Type { get; set; } 
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid WalletId { get; set; }
        public Guid? TargetWalletId { get; set; }

        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; } = string.Empty;
        public Guid? GroupId { get; set; }
    }
}

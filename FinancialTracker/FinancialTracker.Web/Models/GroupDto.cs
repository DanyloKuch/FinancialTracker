using System.Text.Json.Serialization;

namespace FinancialTracker.Web.Models
{
    public class GroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal TotalLimit { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("baseCurrency")] 
        public string BaseCurrency { get; set; }
        public List<string> ParticipantNames { get; set; } = new();

    }
}

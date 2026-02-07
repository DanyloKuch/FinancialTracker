using System.Text.Json.Serialization;

namespace FinancialTracker.Web.Models
{
    public class GroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal? TotalLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal SpentAmount { get; set; }
        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; }
        public string? BaseCurrency { get; set; }

        [JsonPropertyName("members")]
        public List<GroupMemberDto> Members { get; set; } = new();
        public List<string> ParticipantNames => Members?.Select(m => m.UserId.ToString()).ToList() ?? new List<string>();
        public bool IsUserOwner(string userId) => OwnerId == userId;
    }


  
}


using System.Text.Json.Serialization;

namespace FinancialTracker.Web.Models
{
    public class GroupMemberDto
    {
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [JsonPropertyName("userName")] 
        public string? UserName { get; set; }

        [JsonPropertyName("isOwner")] 
        public bool IsOwner { get; set; }
    }
}

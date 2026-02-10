using System.Text.Json.Serialization;

namespace FinancialTracker.Web.Models
{
    public class GroupMemberDto
    {
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [JsonPropertyName("userName")] 
        public string? UserName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        public int Role { get; set; }

        [JsonPropertyName("isOwner")] 
        public bool IsOwner { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace FinancialTracker.Web.Models
{
    public class InviteUserRequest
    {
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = "";
    }

}

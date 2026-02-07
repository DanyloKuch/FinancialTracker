using System.Text.Json.Serialization;

namespace FinancialTracker.Web.Models
{
    public class RespondInvitationRequest
    {
        [JsonPropertyName("isAccepted")]
        public bool IsAccepted { get; set; }
    }
}

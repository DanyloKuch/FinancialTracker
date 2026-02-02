using System.Text.Json.Serialization;

namespace FinancialTracker.Web.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("isSuccess")] public bool IsSuccess { get; set; }
        [JsonPropertyName("value")] public T Value { get; set; }
        public string Error { get; set; }
    }
}

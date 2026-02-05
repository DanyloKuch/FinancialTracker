namespace FinancialTracker.Web.Models
{
    public class InvitationDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string InviterId { get; set; } = ""; // Хто запросив (email)
        public string Status { get; set; } = "Pending";
    }
}

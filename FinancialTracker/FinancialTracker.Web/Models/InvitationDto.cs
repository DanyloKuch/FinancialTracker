namespace FinancialTracker.Web.Models
{
    public class InvitationDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string InviterId { get; set; } = "";
        public string InviterEmail { get; set; } = ""; 
        public string InviteeEmail { get; set; } = "";
        public string Status { get; set; } = "Pending";
    }
}

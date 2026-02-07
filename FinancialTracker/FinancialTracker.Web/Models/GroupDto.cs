namespace FinancialTracker.Web.Models
{
    public class GroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal TotalLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal SpentAmount { get; set; }
    }
}

namespace FinancialTracker.Web.Models
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal TotalLimit { get; set; } 
        public decimal SpentAmount { get; set; } 
        public bool IsArchived { get; set; }
        public string Color { get; set; } = "#FFFFFF";
        public string Icon { get; set; } = "fa-box";
    }
}

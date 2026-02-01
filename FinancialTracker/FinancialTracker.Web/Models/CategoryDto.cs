namespace FinancialTracker.Web.Models
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal TotalLimit { get; set; }
    }
}

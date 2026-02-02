namespace FinancialTracker.Application.DTOs
{
    public record PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
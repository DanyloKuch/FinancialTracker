namespace FinancialTracker.Application.DTOs
{
    public record CategoryRequest(string Name);

    public record CategoryResponse(Guid Id, string Name);
}
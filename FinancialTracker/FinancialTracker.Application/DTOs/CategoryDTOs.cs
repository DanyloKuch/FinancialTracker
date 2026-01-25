namespace FinancialTracker.Application.DTOs
{
    public record CategoryRequest(string Name, bool IsArchived);

    public record CategoryResponse(Guid Id, string Name, bool IsArchived);
}
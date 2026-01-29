namespace FinancialTracker.Application.DTOs
{
    public record CategoryResponse(Guid Id, string Name, bool IsArchived);
}
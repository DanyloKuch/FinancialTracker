namespace FinancialTracker.Application.DTOs
{
    public record InviteUserRequest(Guid GroupId, string Email);
}
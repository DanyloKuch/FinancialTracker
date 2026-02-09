namespace FinancialTracker.Application.DTOs
{
    public record InvitationResponse(
        Guid Id,
        Guid GroupId,
        string InviterEmail, 
        string InviteeEmail, 
        string Status
    );
}
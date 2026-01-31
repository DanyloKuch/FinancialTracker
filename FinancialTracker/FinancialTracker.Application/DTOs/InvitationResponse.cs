namespace FinancialTracker.Application.DTOs
{
    public record InvitationResponse(Guid Id, Guid GroupId, string InviterId, string Status);

}
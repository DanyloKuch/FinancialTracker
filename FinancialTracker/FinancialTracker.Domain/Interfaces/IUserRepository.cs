namespace FinancialTracker.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<Guid?> GetUserIdByEmailAsync(string email);
        Task<string?> GetUserEmailByIdAsync(Guid userId);
        Task<bool> IsUserExistsAsync(string email);
    }
}
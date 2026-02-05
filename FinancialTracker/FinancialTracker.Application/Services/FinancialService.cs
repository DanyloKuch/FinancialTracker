using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly ICurrentUserService _currentUserService;

        public FinancialService(
            ITransactionRepository transactionRepo,
            IWalletRepository walletRepo,
            ICategoryRepository categoryRepo,
            IGroupRepository groupRepo,
            ICurrentUserService currentUserService)
        {
            _transactionRepo = transactionRepo;
            _walletRepo = walletRepo;
            _categoryRepo = categoryRepo;
            _groupRepo = groupRepo;
            _currentUserService = currentUserService;
        }

        public async Task<Result<DashboardSummaryResponse>> GetDashboardSummaryAsync()
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
                return Result<DashboardSummaryResponse>.Failure("User not identified");

            var wallets = await _walletRepo.GetAllByUserIdAsync(userId);
            var categories = await _categoryRepo.GetAllAsync(userId);
            var groupsResult = await _groupRepo.GetAllByUserIdAsync(userId);
            var transactionsResult = await _transactionRepo.GetAllTransactionByUser(userId, 1, 10);

            var globalStatsResult = await _transactionRepo.GetTotalsGroupedByType(userId);
            var walletStats = await _transactionRepo.GetWalletStatsAsync(userId);
            var catStats = await _transactionRepo.GetCategorySpendingAsync(userId);
            var groupStats = await _transactionRepo.GetGroupSpendingAsync(userId);

            var globalStats = globalStatsResult.IsSuccess ? globalStatsResult.Value : new Dictionary<TransactionType, decimal>();

            var walletDtos = wallets.Select(w => {
                var stats = walletStats.ContainsKey(w.Id) ? walletStats[w.Id] : (Income: 0m, Expense: 0m);
                return new DashboardWalletResponse(
                    w.Id, w.Name, w.Balance, w.CurrencyCode, stats.Income, stats.Expense);
            }).ToList();

            var categoryDtos = categories.Select(c => {
                var spent = catStats.ContainsKey(c.Id) ? catStats[c.Id] : 0m;
                return new DashboardCategoryResponse(
                    c.Id, c.Name, c.TotalLimit, spent, c.TotalLimit - spent);
            }).ToList();

            var groupDtos = new List<DashboardGroupResponse>();
            if (groupsResult.IsSuccess)
            {
                groupDtos = groupsResult.Value.Select(g => {
                    var spent = groupStats.ContainsKey(g.Id) ? groupStats[g.Id] : 0m;
                    return new DashboardGroupResponse(g.Id, g.Name, g.TotalLimit ?? 0, spent);
                }).ToList();
            }

            var transactionDtos = new List<TransactionResponse>();
            if (transactionsResult.IsSuccess)
            {
                transactionDtos = transactionsResult.Value.Items.Select(t => new TransactionResponse(
                    t.Id,
                    t.WalletId,
                    t.TargetWalletId,
                    t.CategoryId,
                    t.GroupId,
                    t.Amount,
                    t.Type,
                    t.ExchangeRate,
                    t.Commission,
                    t.Comment,
                    t.CreatedAt
                )).ToList();
            }

            var totalIncome = globalStats.ContainsKey(TransactionType.Income) ? globalStats[TransactionType.Income] : 0m;
            var totalExpense = globalStats.ContainsKey(TransactionType.Expense) ? globalStats[TransactionType.Expense] : 0m;

            var response = new DashboardSummaryResponse
            {
                TotalBalance = wallets.Sum(w => w.Balance),
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Wallets = walletDtos,
                Categories = categoryDtos,
                Groups = groupDtos,
                RecentTransactions = transactionDtos
            };

            return Result<DashboardSummaryResponse>.Success(response);
        }
    }
}
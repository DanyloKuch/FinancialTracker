using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Shared;
using System.Linq;

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

            var walletsTask = _walletRepo.GetAllByUserIdAsync(userId);
            var categoriesTask = _categoryRepo.GetAllAsync(userId);
            var groupsTask = _groupRepo.GetAllByUserIdAsync(userId);

            var transactionsTask = _transactionRepo.GetAllTransactionByUser(userId, 1, 10);

            var globalStatsTask = _transactionRepo.GetTotalsGroupedByType(userId);
            var walletStatsTask = _transactionRepo.GetWalletStatsAsync(userId);
            var catStatsTask = _transactionRepo.GetCategorySpendingAsync(userId);
            var groupStatsTask = _transactionRepo.GetGroupSpendingAsync(userId);

            await Task.WhenAll(
                walletsTask, categoriesTask, groupsTask, transactionsTask,
                globalStatsTask, walletStatsTask, catStatsTask, groupStatsTask);

            var wallets = walletsTask.Result;
            var categories = categoriesTask.Result;
            var groupsResult = groupsTask.Result;
            var transactionsResult = transactionsTask.Result;

            var globalStats = globalStatsTask.Result.Value;
            var walletStats = walletStatsTask.Result;
            var catStats = catStatsTask.Result;
            var groupStats = groupStatsTask.Result;

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
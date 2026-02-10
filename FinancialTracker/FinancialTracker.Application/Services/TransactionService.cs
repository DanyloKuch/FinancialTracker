using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
using System.Collections.Generic;
using System.IO.Pipelines;

namespace FinancialTracker.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ICurrentUserService _currentUserService;
        public ITransactionRepository _transactionRepository;
        public IWalletRepository _walletRepository;
        public IGroupRepository _groupRepository;

        public ICurrencyService _currencyService;

        public TransactionService(IWalletRepository walletRepository, ITransactionRepository transactionRepository, ICurrentUserService currentUserService, IGroupRepository groupRepository, ICurrencyService currencyService)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _currentUserService = currentUserService;
            _groupRepository = groupRepository;
            _currencyService = currencyService;
        }

        public async Task<Result<PagedResult<Transaction>>> GetAllTransactionByUser(int page, int pageSize)
        {
            var userId = _currentUserService.UserId;
            var repoResult = await _transactionRepository.GetAllTransactionByUser(userId, page, pageSize);
            
            if (repoResult.IsFailure)
            {
                return Result<PagedResult<Transaction>>.Failure(repoResult.Error);
            }

            var (items, totalCount) = repoResult.Value;

            var pagedResult = new PagedResult<Transaction>
            {
                Items = items,
                TotalCount = totalCount
            };

            return Result<PagedResult<Transaction>>.Success(pagedResult);
        }

        public async Task<Result<TransactionResponse>> GetTransactionById(Guid id)
        {
            var userId = _currentUserService.UserId;
            var result = await _transactionRepository.GetById(userId, id);

            if (result.IsFailure)
                return Result<TransactionResponse>.Failure(result.Error);
            var transaction = result.Value;

            var response = new TransactionResponse(
                Id: transaction.Id,
                WalletId: transaction.WalletId,
                TargetWalletId: transaction.TargetWalletId,
                CategoryId: transaction.CategoryId,
                GroupId: transaction.GroupId,
                Amount: transaction.Amount,
                Type: transaction.Type,
                ExchangeRate: transaction.ExchangeRate,
                Commission: transaction.Commission,
                Comment: transaction.Comment,
                CreatedAt: transaction.CreatedAt
            );

            return Result<TransactionResponse>.Success(response);
        }

        public async Task<Result<Guid>> CreateTransactionAsync(CreateTransactionRequest request)
        {
            var userId = _currentUserService.UserId;
            if (request.Amount <= 0) return Result<Guid>.Failure("The sum must be greater than zero..");

            return request.Type switch
            {
                TransactionType.Transfer => await HandleTransferAsync(request, userId),
                TransactionType.Income or TransactionType.Expense => await HandleSimpleTransactionAsync(request, userId),
                _ => Result<Guid>.Failure("Unknown transaction type")
            };
        }

        private async Task<Result<Guid>> HandleSimpleTransactionAsync(CreateTransactionRequest request, Guid userId)
        {
            var walletRes = await _walletRepository.GetByIdAsync(request.Walletid, userId);
            if (walletRes.IsFailure) return Result<Guid>.Failure("Wallet not found");

            if(request.Type == TransactionType.Expense && walletRes.Value.Balance < (request.Amount + request.Commission))
            {
                return Result<Guid>.Failure("Not enough funds in your wallet.");
            }

            var wallet = walletRes.Value;
            if (wallet.IsArchived) return Result<Guid>.Failure("You cannot use an archived wallet..");

            if (request.CreatedAt.HasValue && request.CreatedAt.Value.Date > DateTime.UtcNow.Date)
            {
                return Result<Guid>.Failure("Transaction date cannot be in the future.");
            }

            var transactionDate = request.CreatedAt ?? DateTime.UtcNow;
            decimal exchangeRate = 1m;

            var transaction = Transaction.Create(
                id: Guid.NewGuid(),
                userId: userId,
                walletid: request.Walletid,
                targetWalletId: request.TargetWalletId,
                categoryId: request.CategoryId,
                groupId: request.GroupId,
                amount: request.Amount,
                type: request.Type,
                exchangeRate: exchangeRate,
                commission: request.Commission,
                comment: request.Comment,
                createdAt: transactionDate
               );

            if (!transaction.IsSuccess) return Result<Guid>.Failure(transaction.Error);

            var res = wallet.ApplyTransaction(request.Amount, request.Type, request.Commission);
            if (res.IsFailure) return Result<Guid>.Failure(res.Error); 

            await _transactionRepository.Create(transaction.Value);
            await _walletRepository.UpdateAsync(wallet);
            await _transactionRepository.SaveChangesAsync();

            return Result<Guid>.Success(transaction.Value.Id); 
        }

        private async Task<Result<Guid>> HandleTransferAsync(CreateTransactionRequest request, Guid userId)
        {
            if (request.TargetWalletId == null)
                return Result<Guid>.Failure("Target wallet is required");
            if (request.Walletid == request.TargetWalletId)
                return Result<Guid>.Failure("You cannot transfer funds to the same wallet..");

            var sourceResult = await _walletRepository.GetByIdAsync(request.Walletid, userId);
            var targetResult = await _walletRepository.GetByIdAsync(request.TargetWalletId.Value, userId);

            if (sourceResult.IsFailure) return Result<Guid>.Failure($"Sender's wallet: {sourceResult.Error}");
            if (targetResult.IsFailure) return Result<Guid>.Failure($"Recipient's wallet: {targetResult.Error}");


            var sourceWallet = sourceResult.Value;
            
            if (sourceWallet.IsArchived) return Result<Guid>.Failure("Sender's wallet archived.");

            var targetWallet = targetResult.Value;
            if (targetWallet.IsArchived) return Result<Guid>.Failure("Recipient's wallet archived.");

            if (request.CreatedAt.HasValue && request.CreatedAt.Value.Date > DateTime.UtcNow.Date)
            {
                return Result<Guid>.Failure("Transaction date cannot be in the future.");
            }

            decimal exchangeRate = _currencyService.GetExchangeRate(sourceWallet.CurrencyCode, targetWallet.CurrencyCode);

            decimal totalToDebit = request.Amount + request.Commission;
            if (sourceWallet.Balance < totalToDebit)
            {
                return Result<Guid>.Failure($"Not enough funds. Need {totalToDebit} {sourceWallet.CurrencyCode}, but only {sourceWallet.Balance} is on the balance");
            }

            decimal amountToCredit = request.Amount * exchangeRate;

            var transactionDate = request.CreatedAt ?? DateTime.UtcNow;

            var transactionResult = Transaction.Create(
                id: Guid.NewGuid(),
                userId: userId,
                walletid: request.Walletid,
                targetWalletId: request.TargetWalletId,
                categoryId: request.CategoryId,
                groupId: request.GroupId,
                amount: request.Amount,
                type: request.Type,
                exchangeRate: exchangeRate,
                commission: request.Commission,
                comment: request.Comment,
                createdAt: transactionDate
                 );

            if (transactionResult.IsFailure) return Result<Guid>.Failure(transactionResult.Error);

            var sourceRes = sourceWallet.ApplyTransaction(request.Amount, TransactionType.Transfer, request.Commission);
            if (sourceRes.IsFailure) return Result<Guid>.Failure(sourceRes.Error); 

            var targetRes = targetWallet.ApplyTransaction(amountToCredit, TransactionType.Income, 0);
            if (targetRes.IsFailure) return Result<Guid>.Failure(targetRes.Error); 

            await _transactionRepository.Create(transactionResult.Value);
            await _walletRepository.UpdateAsync(sourceWallet);
            await _walletRepository.UpdateAsync(targetWallet);

            await _transactionRepository.SaveChangesAsync();

            return Result<Guid>.Success(transactionResult.Value.Id); 
        }

        public async Task<Result> DeleteTransaction(Guid id)
        {
            var userId = _currentUserService.UserId;

            var transactionRes = await _transactionRepository.GetById(id, userId);

            if (transactionRes.IsFailure) return Result.Failure(transactionRes.Error);
            var transaction = transactionRes.Value;

            var walletRes = await _walletRepository.GetByIdAsync(transaction.WalletId, userId);
            if (walletRes.IsFailure) return Result.Failure("Гаманець не знайдено");
            var wallet = walletRes.Value;

            wallet.UndoTransaction(transaction.Amount, transaction.Type, transaction.Commission ?? 0);

            if (transaction.Type == TransactionType.Transfer && transaction.TargetWalletId.HasValue)
            {
                var targetWalletRes = await _walletRepository.GetByIdAsync(transaction.TargetWalletId.Value, userId);
                if (targetWalletRes.IsSuccess)
                {
                    var amountForTarget = transaction.Amount * (transaction.ExchangeRate ?? 1m);
                    targetWalletRes.Value.UndoTransaction(amountForTarget, TransactionType.Income, 0);
                    await _walletRepository.UpdateAsync(targetWalletRes.Value);
                }
            }

            var resultOfDelete = await _transactionRepository.Delete(userId, transaction.Id);
            if (resultOfDelete.IsFailure) return Result.Failure(resultOfDelete.Error);

                await _walletRepository.UpdateAsync(wallet);
            await _transactionRepository.SaveChangesAsync();

            return Result.Success();

        }

        public async Task<Result<TransactionResponse>> UpdateTransaction(Guid id, UpdateTransactionRequest request)
        {
            var userId = _currentUserService.UserId;

            var transactionRes = await _transactionRepository.GetById(userId, id);
            if (transactionRes.IsFailure) return Result<TransactionResponse>.Failure(transactionRes.Error);
            var transaction = transactionRes.Value;

            var walletRes = await _walletRepository.GetByIdAsync(transaction.WalletId, userId);
            if (walletRes.IsFailure) return Result<TransactionResponse>.Failure("Гаманець не знайдено.");
            var wallet = walletRes.Value;

            wallet.UndoTransaction(transaction.Amount, transaction.Type, transaction.Commission ?? 0);

            if (transaction.Type == TransactionType.Transfer && transaction.TargetWalletId.HasValue)
            {
                var targetWalletRes = await _walletRepository.GetByIdAsync(transaction.TargetWalletId.Value, userId);
                if (targetWalletRes.IsSuccess)
                {
                    var targetWallet = targetWalletRes.Value;
                    var oldTargetAmount = transaction.Amount * (transaction.ExchangeRate ?? 1m);

                    targetWallet.UndoTransaction(oldTargetAmount, TransactionType.Income, 0);

                    await _walletRepository.UpdateAsync(targetWallet);
                }
                else 
                {
                    return Result<TransactionResponse>.Failure("Попередній цільовий гаманець не знайдено для скасування операції");
                }
            }

            if (request.CreatedAt.Date > DateTime.UtcNow.Date)
            {
                return Result<TransactionResponse>.Failure("Дата не може бути в майбутньому.");
            }

            transaction.Update(
                request.WalletId,
                request.Amount,
                request.CategoryId,
                request.Comment,
                request.TargetWalletId,
                request.ExchangeRate,
                request.Commission,
                request.CreatedAt
                );


            var currentWallet = wallet;

            if (transaction.WalletId != wallet.Id)
            {
                var newWalletRes = await _walletRepository.GetByIdAsync(transaction.WalletId, userId);
                if (newWalletRes.IsFailure) return Result<TransactionResponse>.Failure("Новий гаманець не знайдено.");
                await _walletRepository.UpdateAsync(wallet);

                currentWallet = newWalletRes.Value;
            }

            var applyRes = currentWallet.ApplyTransaction(transaction.Amount, transaction.Type, transaction.Commission ?? 0);
            if (applyRes.IsFailure) return Result<TransactionResponse>.Failure(applyRes.Error); 

            if (transaction.Type == TransactionType.Transfer && transaction.TargetWalletId.HasValue)
            {
                var targetWalletRes = await _walletRepository.GetByIdAsync(transaction.TargetWalletId.Value, userId);
                if (targetWalletRes.IsFailure)
                {
                    return Result<TransactionResponse>.Failure("Цільовий гаманець не знайдено");
                }
                var targetWallet = targetWalletRes.Value;
                var newTargetAmount = transaction.Amount * (transaction.ExchangeRate ?? 1m);

                targetWallet.ApplyTransaction(newTargetAmount, TransactionType.Income, 0);

                await _walletRepository.UpdateAsync(targetWallet);
            }

            await _walletRepository.UpdateAsync(wallet);
            await _transactionRepository.UpdateAsync(transaction);

            await _transactionRepository.SaveChangesAsync();

            var response = new TransactionResponse(
                transaction.Id,
                transaction.WalletId,
                transaction.TargetWalletId,
                transaction.CategoryId,
                transaction.GroupId,
                transaction.Amount,
                transaction.Type,
                transaction.ExchangeRate,
                transaction.Commission,
                transaction.Comment,
                transaction.CreatedAt
            );

            return Result<TransactionResponse>.Success(response);
        }

        public async Task<Result<FinancialSummaryResponse>> GetGeneralFinancialSummary()
        {
            var userId = _currentUserService.UserId;
            var repoResult = await _transactionRepository.GetTotalsGroupedByType(userId);

            if (repoResult.IsFailure)
            {
                return Result<FinancialSummaryResponse>.Failure(repoResult.Error);
            }

            var totals = repoResult.Value;

            var response = new FinancialSummaryResponse(
                TotalIncome: totals.GetValueOrDefault(TransactionType.Income, 0m),
                TotalExpense: Math.Abs(totals.GetValueOrDefault(TransactionType.Expense, 0m))
            );

            return Result<FinancialSummaryResponse>.Success(response);
        }

        public async Task<Result<PagedResult<GroupTransactionResponseDto>>> GetTransactionByGroup(Guid groupId, int page, int pageSize)
        {
            var userId = _currentUserService.UserId;

            var isUserInGroup = await _groupRepository.IsUserInGroup(userId, groupId);
            if (isUserInGroup.IsFailure) return Result<PagedResult<GroupTransactionResponseDto>>.Failure(isUserInGroup.Error);

            var repoResult = await _transactionRepository.GetAllTransactionByGroup(groupId, page, pageSize);

            var (items, totalCount) = repoResult.Value;

            var dto = items.Select(x => new GroupTransactionResponseDto
            {
                Id = x.Transaction.Id,
                Amount = x.Transaction.Amount,
                UserEmail = x.Email,
                UserId = x.Transaction.UserId,
                CreatedAt = x.Transaction.CreatedAt,
                Comment = x.Transaction.Comment,
                WalletId = x.Transaction.WalletId,
                CategoryId = x.Transaction.CategoryId
            })
            .ToList();

            return Result<PagedResult<GroupTransactionResponseDto>>.Success(new PagedResult<GroupTransactionResponseDto>
            {
                Items = dto,
                TotalCount = totalCount
            });
        }
    }
}

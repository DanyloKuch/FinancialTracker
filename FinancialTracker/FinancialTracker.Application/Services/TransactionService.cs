using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
using System.Collections.Generic;

namespace FinancialTracker.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ICurrentUserService _currentUserService;
        public ITransactionRepository _transactionRepository;
        public IWalletRepository _walletRepository;

        public TransactionService(IWalletRepository walletRepository, ITransactionRepository transactionRepository, ICurrentUserService currentUserService)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<IReadOnlyList<Transaction>>> GetAllTransactionByUser()
        {
            var userId = _currentUserService.UserId;
            var transactions = await _transactionRepository.GetAllTransactionByUser(userId);

            return transactions;

        }

        public async Task<Result<TransactionResponse>> GetTransactionById(Guid id)
        {
            var userId = _currentUserService.UserId;
            var result = await _transactionRepository.GetById(id, userId);

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
            if (request.Amount <= 0) return Result<Guid>.Failure("Сума повинна бути більшою за нуль.");

            return request.Type switch
            {
                // Змінено на повернення Result<Guid>
                TransactionType.Transfer => await HandleTransferAsync(request, userId),
                TransactionType.Income or TransactionType.Expense => await HandleSimpleTransactionAsync(request, userId),
                _ => Result<Guid>.Failure("Невідомий тип транзакції")
            };
        }

        private async Task<Result<Guid>> HandleSimpleTransactionAsync(CreateTransactionRequest request, Guid userId)
        {
            var walletRes = await _walletRepository.GetByIdAsync(request.Walletid, userId);
            if (walletRes.IsFailure) return Result<Guid>.Failure("Гаманець не знайдено");

            var wallet = walletRes.Value;
            if (wallet.IsArchived) return Result<Guid>.Failure("Не можна використовувати заархівований гаманець.");

            var transaction = Transaction.Create(
                id: Guid.NewGuid(),
                userId: userId,
                walletid: request.Walletid,
                targetWalletId: request.TargetWalletId,
                categoryId: request.CategoryId,
                groupId: request.GroupId,
                amount: request.Amount,
                type: request.Type,
                exchangeRate: request.ExchangeRate,
                commission: request.Commission,
                comment: request.Comment,
                createdAt: DateTime.UtcNow
               );

            if (!transaction.IsSuccess) return Result<Guid>.Failure(transaction.Error);

            var res = wallet.ApplyTransaction(request.Amount, request.Type, request.Commission);
            if (res.IsFailure) return Result<Guid>.Failure(res.Error); // Каст до Result<Guid>

            await _transactionRepository.Create(transaction.Value);
            await _walletRepository.UpdateAsync(wallet);
            await _transactionRepository.SaveChangesAsync();

            return Result<Guid>.Success(transaction.Value.Id); // Повертаємо Guid транзакції
        }

        private async Task<Result<Guid>> HandleTransferAsync(CreateTransactionRequest request, Guid userId)
        {
            if (request.TargetWalletId == null)
                return Result<Guid>.Failure("Цільовий гаманець обов'язковий");
            if (request.Walletid == request.TargetWalletId)
                return Result<Guid>.Failure("Не можна переказати кошти на той самий гаманець.");

            var sourceResult = await _walletRepository.GetByIdAsync(request.Walletid, userId);
            var targetResult = await _walletRepository.GetByIdAsync(request.TargetWalletId.Value, userId);

            if (sourceResult.IsFailure) return Result<Guid>.Failure($"Гаманець відправника: {sourceResult.Error}");
            if (targetResult.IsFailure) return Result<Guid>.Failure($"Гаманець отримувача: {targetResult.Error}");

            var sourceWallet = sourceResult.Value;
            if (sourceWallet.IsArchived) return Result<Guid>.Failure("Гаманець відправника заархівований.");

            var targetWallet = targetResult.Value;
            if (targetWallet.IsArchived) return Result<Guid>.Failure("Гаманець отримувача заархівований.");

            var transactionResult = Transaction.Create(
                id: Guid.NewGuid(),
                userId: userId,
                walletid: request.Walletid,
                targetWalletId: request.TargetWalletId,
                categoryId: request.CategoryId,
                groupId: request.GroupId,
                amount: request.Amount,
                type: request.Type,
                exchangeRate: request.ExchangeRate ?? 1m,
                commission: request.Commission,
                comment: request.Comment,
                createdAt: DateTime.UtcNow
                 );

            if (transactionResult.IsFailure) return Result<Guid>.Failure(transactionResult.Error);

            var sourceRes = sourceWallet.ApplyTransaction(request.Amount, TransactionType.Transfer, request.Commission);
            if (sourceRes.IsFailure) return Result<Guid>.Failure(sourceRes.Error); // Каст помилки

            var amountForTarget = request.Amount * (request.ExchangeRate ?? 1m);
            var targetRes = targetWallet.ApplyTransaction(amountForTarget, TransactionType.Income, 0);
            if (targetRes.IsFailure) return Result<Guid>.Failure(targetRes.Error); // Каст помилки

            await _transactionRepository.Create(transactionResult.Value);
            await _walletRepository.UpdateAsync(sourceWallet);
            await _walletRepository.UpdateAsync(targetWallet);

            await _transactionRepository.SaveChangesAsync();

            return Result<Guid>.Success(transactionResult.Value.Id); // Повертаємо Guid
        }
    }
}

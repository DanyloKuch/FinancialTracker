using System;
using System.Collections.Generic;
using System.Linq; // Для Select
using System.Threading.Tasks;
using FinancialTracker.Application.DTOs;
using FinancialTracker.Application.Interfaces;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;

namespace FinancialTracker.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrentUserService _currentUserService;

        public WalletService(IWalletRepository walletRepository, ICurrentUserService currentUserService)
        {
            _walletRepository = walletRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> CreateWalletAsync(WalletRequest request)
        {
            var userId = _currentUserService.UserId;

            // 1. DTO -> Domain Model
            // Вся бизнес-валидация происходит внутри Wallet.Create
            var walletResult = Wallet.Create(
                Guid.NewGuid(),
                userId,
                request.Name,
                request.Type,
                request.Balance,
                request.CurrencyCode,
                false, // IsArchived
                DateTime.UtcNow
            );

            if (walletResult.IsFailure)
                return Result<Guid>.Failure(walletResult.Error);

            // 2. Сохраняем через репозиторий
            await _walletRepository.AddAsync(walletResult.Value);

            return Result<Guid>.Success(walletResult.Value.Id);
        }

        public async Task<IEnumerable<WalletResponse>> GetWalletsAsync()
        {
            var userId = _currentUserService.UserId;
            var wallets = await _walletRepository.GetAllByUserIdAsync(userId);

            // 3. Domain Model -> DTO (Record)
            // Используем синтаксис конструктора, так как это позиционный record
            return wallets.Select(w => new WalletResponse(
                w.Id,
                w.Name,
                w.Type,
                w.Balance,
                w.CurrencyCode,
                w.UpdatedAt
            ));
        }

        public async Task<Result<WalletResponse>> GetWalletByIdAsync(Guid id)
        {
            var userId = _currentUserService.UserId;
            var result = await _walletRepository.GetByIdAsync(id, userId);

            if (result.IsFailure)
                return Result<WalletResponse>.Failure(result.Error);

            var w = result.Value;

            // Domain Model -> DTO (Record)
            return Result<WalletResponse>.Success(new WalletResponse(
                w.Id,
                w.Name,
                w.Type,
                w.Balance,
                w.CurrencyCode,
                w.UpdatedAt
            ));
        }

        public async Task<Result<WalletResponse>> UpdateWalletAsync(Guid id, WalletRequest request)
        {
            var userId = _currentUserService.UserId;

           
            var existingResult = await _walletRepository.GetByIdAsync(id, userId);

          
            if (existingResult.IsFailure)
                return Result<WalletResponse>.Failure("Wallet not found");

            var existingWallet = existingResult.Value;

      
            var updatedWalletResult = Wallet.Create(
                existingWallet.Id,
                existingWallet.UserId,
                request.Name,
                request.Type,
                request.Balance, 
                request.CurrencyCode,
                existingWallet.IsArchived,
                DateTime.UtcNow
            );

            if (updatedWalletResult.IsFailure)
                return Result<WalletResponse>.Failure(updatedWalletResult.Error);

            var updatedWallet = updatedWalletResult.Value;

           
            var dbResult = await _walletRepository.UpdateAsync(updatedWallet);

          
            if (dbResult.IsFailure)
                return Result<WalletResponse>.Failure(dbResult.Error);

            
            var response = new WalletResponse(
                updatedWallet.Id,
                updatedWallet.Name,
                updatedWallet.Type,
                updatedWallet.Balance,
                updatedWallet.CurrencyCode,
                updatedWallet.UpdatedAt
            );

          
            return Result<WalletResponse>.Success(response);
        }

        public async Task<Result> DeleteWalletAsync(Guid id)
        {
            var userId = _currentUserService.UserId;
            return await _walletRepository.DeleteAsync(id, userId);
        }
    }
}
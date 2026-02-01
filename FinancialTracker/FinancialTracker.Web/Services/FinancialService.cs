using Blazored.LocalStorage;
using FinancialTracker.Web.Models; 
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinancialTracker.Web.Services
{
    public class FinancialService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public FinancialService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        private async Task SetTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<WalletDto>> GetWalletsAsync()
        {
            await SetTokenAsync();
            try
            {
                var response = await _http.GetFromJsonAsync<List<WalletDto>>("api/v1/Wallets");
                return response ?? new List<WalletDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching wallets: {ex.Message}");
                return new List<WalletDto>();
            }
        }
        public async Task<WalletDto?> CreateWalletAsync(WalletCreateRequest request)
        {
            await SetTokenAsync(); 
            try
            {
                var response = await _http.PostAsJsonAsync("api/v1/Wallets", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<WalletDto>();
                }
            }
            catch { return null; }

            return null;
        }
        public async Task<bool> UpdateWalletAsync(WalletDto wallet)
        {
            await SetTokenAsync();
            try
            {
                var response = await _http.PutAsJsonAsync($"api/v1/Wallets/{wallet.Id}", wallet);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> DeleteWalletAsync(Guid walletId)
        {
            await SetTokenAsync(); 
            try
            {
                var response = await _http.DeleteAsync($"api/v1/Wallets/{walletId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<TransactionDto>> GetTransactionsAsync()
        {
            await SetTokenAsync();
            try
            {
                var response = await _http.GetFromJsonAsync<ApiResponse<List<TransactionDto>>>("api/v1/Transaction");
                return response != null && response.IsSuccess ? response.Value : new List<TransactionDto>();
            }
            catch { return new List<TransactionDto>(); }
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            await SetTokenAsync();
            try
            {
                return await _http.GetFromJsonAsync<List<CategoryDto>>("api/v1/categories") ?? new List<CategoryDto>();
            }
            catch { return new List<CategoryDto>(); }
        }

        public async Task<List<GroupDto>> GetGroupsAsync()
        {
            await SetTokenAsync();
            try
            {
                return await _http.GetFromJsonAsync<List<GroupDto>>("api/v1/groups") ?? new List<GroupDto>();
            }
            catch { return new List<GroupDto>(); }
        }

        public async Task<Guid?> CreateTransactionAsync(TransactionCreateDto model)
        {
            await SetTokenAsync();
            try
            {
                var response = await _http.PostAsJsonAsync("api/v1/Transaction", model);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Guid>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
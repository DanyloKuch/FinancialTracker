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
                return await _http.GetFromJsonAsync<List<WalletDto>>("api/v1/Wallets") ?? new List<WalletDto>();
            }
            catch { return new List<WalletDto>(); }
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
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

        public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(int page = 1, int pageSize = 20)
        {
            await SetTokenAsync();
            try
            {
                var url = $"api/v1/Transaction?page={page}&pageSize={pageSize}";

                var response = await _http.GetFromJsonAsync<ApiResponse<PagedResult<TransactionDto>>>(url);
                return response != null && response.IsSuccess 
                    ? response.Value 
                    : new PagedResult<TransactionDto>();
            }
            catch
            {
                return new PagedResult<TransactionDto>();
            }
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
                var responseString = await _http.GetStringAsync("api/v1/groups");

                if (responseString.Trim().StartsWith("["))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<GroupDto>>(responseString,
                        new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase })
                        ?? new List<GroupDto>();
                }

                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<List<GroupDto>>>(responseString,
                    new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });

                return apiResponse?.Value ?? new List<GroupDto>();
            }
            catch (Exception ex)
            {
                return new List<GroupDto>();
            }


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


        public async Task<bool> UpdateGroupAsync(GroupDto group)
        {
            await SetTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/v1/groups/{group.Id}", group);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"SERVER ERROR: {errorContent}");
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteGroupAsync(string groupId)
        {
            await SetTokenAsync();
            try
            {
                var response = await _http.DeleteAsync($"api/v1/groups/{groupId}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
                    response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return (false, "Ви не можете видалити цю групу, бо ви не є власником");
                }

                return (false, "Сталася помилка при видаленні");
            }
            catch
            {
                return (false, "Помилка з'єднання з сервером");
            }
        }

        public async Task<GroupDto?> CreateGroupAsync(GroupDto newGroup)
        {
            await SetTokenAsync();
            var response = await _http.PostAsJsonAsync("api/v1/groups", newGroup);

            if (response.IsSuccessStatusCode)
            {
                var createdId = await response.Content.ReadAsStringAsync();

                createdId = createdId.Trim('"');

                return new GroupDto
                {
                    Id = createdId, 
                    Name = newGroup.Name
                };
            }
            return null;
        }

        public async Task<FinancialSummaryResponse> GetFinancialSummaryAsync()
        {
            await SetTokenAsync();
            try
            {
                var response = await _http.GetFromJsonAsync<FinancialSummaryResponse>("api/v1/Transaction/summary");

                Console.WriteLine($"Direct Response: Income={response?.TotalIncome}, Expense={response?.TotalExpense}");

                return response ?? new FinancialSummaryResponse(0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching summary: {ex.Message}");

                try
                {
                    var wrappedResponse = await _http.GetFromJsonAsync<ApiResponse<FinancialSummaryResponse>>("api/v1/Transaction/summary");
                    return wrappedResponse != null && wrappedResponse.IsSuccess
                        ? wrappedResponse.Value
                        : new FinancialSummaryResponse(0, 0);
                }
                catch
                {
                    return new FinancialSummaryResponse(0, 0);
                }
            }
        }

        public async Task<bool> UpdateTransactionAsync(Guid transactionId, TransactionUpdateDto model)
        {
            await SetTokenAsync();
            try
            {
                var response = await _http.PutAsJsonAsync($"api/v1/Transaction/{transactionId}", model);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTransactionAsync(Guid transactionId)
        {
            await SetTokenAsync();
            try
            {
                var response = await _http.DeleteAsync($"api/v1/Transaction/{transactionId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


        public async Task<DashboardSummaryDto?> GetDashboardSummaryAsync()
        {
            await SetTokenAsync();
            try
            {
                var response = await _http.GetFromJsonAsync<DashboardSummaryDto>("api/v1/financial/dashboard-summary");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching dashboard summary: {ex.Message}");
                return null;
            }
        }
        public async Task SendInvitationAsync(InviteUserRequest request)
        {
            await SetTokenAsync();
            var response = await _http.PostAsJsonAsync("api/v1/invitations", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Сервер повернув помилку: {response.StatusCode} - {error}");
            }
        }

        public async Task<bool> AddMemberByEmailAsync(string groupId, string email)
        {
            await SetTokenAsync();
            var request = new InviteUserRequest
            {
                GroupId = Guid.Parse(groupId),
                Email = email 
            };

            var response = await _http.PostAsJsonAsync("api/v1/invitations", request);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<InvitationDto>> GetMyInvitationsAsync()
        {
            await SetTokenAsync(); 
            try
            {
                var response = await _http.GetFromJsonAsync<List<InvitationDto>>("api/v1/invitations/received");
                return response ?? new List<InvitationDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні запрошень: {ex.Message}");
                return new List<InvitationDto>();
            }
        }

        public async Task<bool> RespondToInvitationAsync(Guid invitationId, bool accept)
        {
            var request = new RespondInvitationRequest { IsAccepted = accept };
            var response = await _http.PostAsJsonAsync($"api/v1/invitations/{invitationId}/respond", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Помилка сервера: {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> KickMemberAsync(string groupId, string memberId)
        {
            await SetTokenAsync(); 
            var response = await _http.DeleteAsync($"api/v1/Groups/{groupId}/kikcmembers/{memberId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LeaveGroupAsync(string groupId)
        {
            var response = await _http.PostAsync($"/api/v1/Groups/{groupId}/leave", null);
            return response.IsSuccessStatusCode;
        }

    }

}
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace FinancialTracker.Web.Providers
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public JwtAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Читаємо токен
                var token = await _localStorage.GetItemAsync<string>("authToken");

                // ДЕБАГ: Пишемо в консоль браузера (F12)
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("AUTH DEBUG: Токен в LocalStorage порожній або null.");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                Console.WriteLine($"AUTH DEBUG: Токен знайдено! Довжина: {token.Length}");

                // Вставляємо токен в заголовок
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Створюємо юзера
                var identity = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, "Користувач"),
            new Claim(ClaimTypes.AuthenticationMethod, "Bearer")
        }, "Bearer"); // <--- ВАЖЛИВО: "Bearer" тут обов'язковий, щоб IsAuthenticated було true

                var user = new ClaimsPrincipal(identity);
                Console.WriteLine($"AUTH DEBUG: Користувач створений. IsAuthenticated: {user.Identity.IsAuthenticated}");

                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AUTH DEBUG ERROR: {ex.Message}");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyUserAuthentication(string token)
        {
            // 1. Обов'язково додаємо токен в HTTP клієнт, щоб наступні запити йшли з ним
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. Повідомляємо Blazor, що стан змінився
            var identity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.Name, "Користувач"),
        new Claim(ClaimTypes.AuthenticationMethod, "Bearer")
    }, "Bearer");

            var user = new ClaimsPrincipal(identity);
            var authState = Task.FromResult(new AuthenticationState(user));

            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
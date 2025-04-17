using ASM.Client.Models;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ASM.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(LoginModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", model);
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result?.Token != null)
            {
                await _localStorage.SetItemAsync("authToken", result.Token);
                return true;
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
        }

        public async Task<string?> GetProfileAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token)) return null;

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.GetAsync("api/auth/GetProfile");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsStringAsync(); // hoặc bạn có thể deserialize thành object
        }

        public async Task<UserProfileModel?> GetUserProfileAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await _http.GetFromJsonAsync<UserProfileModel>("api/auth/GetProfile");
            return result;
        }

        public async Task<bool> UpdateUserProfileAsync(UserProfileModel model)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PutAsJsonAsync("api/auth/UpdateProfile", model);
            return response.IsSuccessStatusCode;
        }

    }
}

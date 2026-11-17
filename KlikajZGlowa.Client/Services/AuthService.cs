using KlikajZGlowa.Client.Interfaces;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.ApplicationUser;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace KlikajZGlowa.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            HttpClient http,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(LoginApplicationUserDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/ApplicationUser/login", dto);
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (result == null || string.IsNullOrEmpty(result.AccessToken)) return false;

            await _localStorage.SetItemAsync("accessToken", result.AccessToken);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogin(result.AccessToken);

            return true;
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(CreateApplicationUserDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/ApplicationUser/register", dto);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            try
            {
                var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (errorObj.TryGetProperty("message", out var msg))
                {
                    return (false, msg.GetString());
                }
            }
            catch
            {
                // Ignorujemy błędy parsowania
            }

            return (false, "Rejestracja nie powiodła się. Sprawdź poprawność danych.");
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("accessToken");
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public async Task<ApplicationUserDto?> GetAsync()
        {
            try
            {
                var currentUser = await _http.GetFromJsonAsync<ApplicationUserDto>("api/ApplicationUser/me");


                return currentUser;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(string id, UpdateApplicationUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }


            try
            {
                var response = await _http.PutAsJsonAsync($"api/ApplicationUser/{id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<List<ApplicationUserDto>> GetAllAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/ApplicationUser");
                if (!response.IsSuccessStatusCode) return new List<ApplicationUserDto>();
                return await response.Content.ReadFromJsonAsync<List<ApplicationUserDto>>() ?? new List<ApplicationUserDto>();
            }
            catch (HttpRequestException)
            {
                return new List<ApplicationUserDto>();
            }
        }

        public async Task<ApplicationUserDto?> GetByIdAsync(string id)
        {
            try
            {
                var response = await _http.GetAsync($"api/ApplicationUser/{id}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<ApplicationUserDto>();
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<List<string>> GetRolesAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return new List<string>();
            try
            {
                var roles = await _http.GetFromJsonAsync<List<string>> ($"api/ApplicationUser/{id}/roles");
                return roles ?? new List<string>();
            }
            catch (HttpRequestException)
            {
                return new List<string>();
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateRolesAsync(string id, List<string> roles)
        {
            if (string.IsNullOrWhiteSpace(id)) return (false, "Niepoprawny identyfikator.");

            try
            {
                var response = await _http.PutAsJsonAsync($"api/ApplicationUser/{id}/roles", new ManageUserRolesDto { Roles = roles });
                if (response == null) return (true, null);


                try
                {
                    var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (errorObj.TryGetProperty("message", out var msg))
                    {
                        return (false, msg.GetString());
                    }
                }
                catch { }

                return (false, "Nie udało się zaktualizować ról użytkownika.");
            }
            catch (HttpRequestException)
            {
                return (false, "Błąd połączenia z serwerem.");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return (false, "Niepoprawny identyfikator.");

            try
            {
                var response = await _http.DeleteAsync($"api/ApplicationUser/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                try
                {
                    var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (errorObj.TryGetProperty("message", out var msg))
                    {
                        return (false, msg.GetString());
                    }
                }
                catch { }

                return (false, "Nie udało się usunąć użytkownika.");
            }
            catch (HttpRequestException)
            {
                return (false, "Błąd połączenia z serwerem.");
            }
        }
    }
}
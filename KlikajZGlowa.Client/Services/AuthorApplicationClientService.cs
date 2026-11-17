
using KlikajZGlowa.Client.Interfaces;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.AuthorApplication;
using System.Net.Http.Json;

namespace KlikajZGlowa.Client.Services
{
    public class AuthorApplicationClientService : IAuthorApplicationClientService
    {
        private readonly HttpClient _http;

        public AuthorApplicationClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<AuthorApplicationDto>> GetPendingApplicationsAsync()
        {
            try
            {
                var pendings = await _http.GetFromJsonAsync<List<AuthorApplicationDto>>("api/authorapplication/pending");
                return pendings ?? new List<AuthorApplicationDto> { };
            }
            catch (HttpRequestException)
            {
                return new List<AuthorApplicationDto> { };
            }
        }

        public async Task<bool> ApproveAsync(int id)
        {
            try
            {
                var pending = await _http.PostAsync($"api/authorapplication/{id}/approve", null);
                return pending.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> RejectAsync(int id, string reason)
        {
            try
            {
                var rejection = await _http.PostAsJsonAsync($"api/authorapplication/{id}/reject", new RejectAuthorApplicationDto { Reason = reason });
                return rejection.IsSuccessStatusCode;
            }
            catch(HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> SubmitAsync(string motivation)
        {
            try
            {
                var submit = await _http.PostAsJsonAsync($"api/authorapplication", new CreateAuthorApplicationDto { Motivation = motivation});
                return submit.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> HasPendingApplicationAsync()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<PendingStatusResponse>("api/authorapplication/my-status");
                return response?.HasPending ?? false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> HasPendingApplicationByIdAsync(string userId)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<PendingStatusResponse>($"api/authorapplication/{userId}/status");
                return response?.HasPending ?? false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
        private class PendingStatusResponse
        {
            public bool HasPending { get; set; }
        }
    }
    
}


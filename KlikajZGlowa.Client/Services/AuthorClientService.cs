using KlikajZGlowa.Client.Interfaces;
using KlikajZGlowa.Service.DTO.Author;
using System.Net.Http.Json;

namespace KlikajZGlowa.Client.Services
{
    public class AuthorClientService : IAuthorClientService
    {
        private readonly HttpClient _http;

        public AuthorClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<CreateAuthorDto?> CreateAsync(CreateAuthorDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Author", dto);
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<CreateAuthorDto>();
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}

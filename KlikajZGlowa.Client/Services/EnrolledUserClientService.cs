using KlikajZGlowa.Client.Interfaces;
using KlikajZGlowa.Service.DTO.EnrolledUser;
using System.Net.Http.Json;

namespace KlikajZGlowa.Client.Services
{
    public class EnrolledUserClientService : IEnrolledUserClientService
    {
        private readonly HttpClient _http;
        public EnrolledUserClientService(HttpClient http)
        {
            _http = http;
        }
        public async Task<List<EnrolledUserDto>> GetAll()
        {
            try
            {
                var response = await _http.GetAsync("api/EnrolledUser");
                if (!response.IsSuccessStatusCode) return new List<EnrolledUserDto>();
                return await response.Content.ReadFromJsonAsync<List<EnrolledUserDto>>() ?? new List<EnrolledUserDto>();
            }
            catch (HttpRequestException)
            {
                return new List<EnrolledUserDto>();
            }
        }

        public async Task<DetailsEnrolledUserDto?> GetById(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/EnrolledUser/{id}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<DetailsEnrolledUserDto>();
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}

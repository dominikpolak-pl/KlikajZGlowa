using KlikajZGlowa.Client.Interfaces;
using KlikajZGlowa.Service.DTO.Course;
using KlikajZGlowa.Service.DTO.Lesson;
using KlikajZGlowa.Service.Service;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace KlikajZGlowa.Client.Services
{
    public class LessonClientService : ILessonClientService
    {
        private readonly HttpClient _http;

        public LessonClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<LessonDto?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/lesson/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<LessonDto>();
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/lesson/{id}");
                return response.IsSuccessStatusCode;
            }
            catch( HttpRequestException )
            {
                return false;
            }
        }

        public async Task<CreateLessonDto?> CreateAsync(CreateLessonDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Lesson", dto);
                if (!response.IsSuccessStatusCode) { return null; }
                return await response.Content.ReadFromJsonAsync<CreateLessonDto>();
            }catch(HttpRequestException)
            {
                return null;
            }
        }
    }
}

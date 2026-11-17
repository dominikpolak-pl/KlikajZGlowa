
using KlikajZGlowa.Client.Interfaces;
using KlikajZGlowa.Service.DTO.Course;
using System.Net;
using System.Net.Http.Json;

namespace KlikajZGlowa.Client.Services
{
    public class CourseClientService : ICourseClientService
    {
        private readonly HttpClient _http;

        public CourseClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CourseDto>> GetAllAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<CourseDto>>("api/Course");
                return result ?? new List<CourseDto>();
            }
            catch (HttpRequestException  )
            {
                return new List<CourseDto>();
            }
        }

        public async Task<CourseDto?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/Course/{id}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<CourseDto>();
            }catch(HttpRequestException  )
            {
                return null;
            }
        }

        public async Task<DetailsCourseDto?> GetDetailsAsync(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/Course/{id}/details");
                if (response.StatusCode == HttpStatusCode.Forbidden || !response.IsSuccessStatusCode)
                {
                    return null;
                }
                return await response.Content.ReadFromJsonAsync<DetailsCourseDto>();
            }
            catch(HttpRequestException  )
            {
                return null;
            }
        }

        public async Task<bool> EnrollAsync(int id)
        {
            try
            {
                var response = await _http.PostAsync($"api/Course/{id}/enroll", null);
                return response.IsSuccessStatusCode;
            }catch (HttpRequestException  )
            {
                return false;
            }
        }

        public async Task<CourseDto?> CreateAsync(CreateCourseDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Course", dto);
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<CourseDto>();
            }
            catch(HttpRequestException  )
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/Course/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateCourseDto dto)
        {
            if (id <= 0 || dto is null)
            {
                return false;
            }

            try
            {
                var response = await _http.PutAsJsonAsync($"api/Course/{id}", dto);
                return response.IsSuccessStatusCode;

            }
            catch (HttpRequestException)
            {
                return false;
            }

        }


        public async Task<bool> AddAuthorAsync(int courseId, int authorId)
        {
            try
            {
                var response = await _http.PostAsync(
                    $"api/Course/{courseId}/authors/{authorId}",
                    null);

                return response.IsSuccessStatusCode;
            }
            catch(HttpRequestException )
            {
                return false;
            }
        }

        public async Task<bool> RemoveAuthorAsync(int courseId, int authorId)
        {
            try
            {
                var response = await _http.DeleteAsync(
                    $"api/Course/{courseId}/authors/{authorId}");

                return response.IsSuccessStatusCode;
            }
            catch(HttpRequestException)
            {
                return false;
            }
        }
    }
}
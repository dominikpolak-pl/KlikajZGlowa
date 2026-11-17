using KlikajZGlowa.Client.Interfaces;
using KlikajZGlowa.Service.DTO.Lesson;
using System.Net.Http.Json;
using System.Text.Json;

namespace KlikajZGlowa.Client.Services
{
    public class AiClientService : IAiClientService
    {
        private readonly HttpClient _http;
        public AiClientService(HttpClient http)
        {
            _http = http;
        }
        public async Task<(bool Success, GeneratedLessonResultDto? Result, string? ErrorMassage)> GenerateLessonDraftAsync(GenerateLessonRequestDto request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/AIClient/generate-lesson", request);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GeneratedLessonResultDto>();
                    return (true, result, null);
                }
                else
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (errorResponse.TryGetProperty("message", out var msg))
                    {
                        return (false, null, msg.GetString());
                    }
                }
                return (false, null, "Nieznany błąd serwera.");
            }
            catch (HttpRequestException)
            {
                return (false, null, "Błąd połączenia z serwerem.");
            }
        }
    }
}

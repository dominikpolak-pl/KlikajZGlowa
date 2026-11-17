using KlikajZGlowa.Service.DTO.Lesson;

namespace KlikajZGlowa.Client.Interfaces
{
        public interface IAiClientService
    {
        public Task<(bool Success, GeneratedLessonResultDto? Result, string? ErrorMassage)> GenerateLessonDraftAsync(GenerateLessonRequestDto request);
    }


}

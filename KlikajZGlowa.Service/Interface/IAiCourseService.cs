using KlikajZGlowa.Service.DTO.Lesson;

namespace KlikajZGlowa.Service.Interface
{
    public interface IAiCourseService 
    {
        Task<GeneratedLessonResultDto> GenerateLessonDraftAsync(GenerateLessonRequestDto request);
    }
}

using KlikajZGlowa.Service.DTO.Lesson;

namespace KlikajZGlowa.Client.Interfaces
{
    public interface ILessonClientService
    {
        Task<LessonDto?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<CreateLessonDto?> CreateAsync(CreateLessonDto dto);
    }
}

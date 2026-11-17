using KlikajZGlowa.Service.DTO.Course;

namespace KlikajZGlowa.Client.Interfaces
{
    public interface ICourseClientService
    {
        Task<List<CourseDto>> GetAllAsync();
        Task<CourseDto?> GetByIdAsync(int id);
        Task<DetailsCourseDto?> GetDetailsAsync(int id);
        Task<bool> EnrollAsync(int id);
        Task<CourseDto?> CreateAsync(CreateCourseDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateCourseDto dto);
        Task<bool> AddAuthorAsync(int courseId, int authorId);
        Task<bool> RemoveAuthorAsync(int courseId, int authorId);
    }
}

using KlikajZGlowa.Service.DTO.Course;
using KlikajZGlowa.Service.DTO.Lesson;
using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.Interface
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetAllAsync();
        Task<CourseDto> CreateAsync(CreateCourseDto createCourseDto);
        Task<CourseDto?> GetByIdAsync(int CourseId);
        Task<DetailsCourseDto?> GetDetailsByIdAsync(int CourseId);
        Task<bool> UpdateAsync(int id,UpdateCourseDto updateCourseDto);
        Task<bool> DeleteAsync(int CourseId);
        Task<bool> EnrollCurrentUserAsync(int courseId);
        Task<bool> AddAuthorAsync(int courseId, int authorId);
        Task<bool> RemoveAuthorAsync(int courseId, int authorId);
    }
}

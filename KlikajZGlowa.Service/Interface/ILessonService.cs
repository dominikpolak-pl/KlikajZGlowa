using System;
using System.Collections.Generic;
using System.Text;
using KlikajZGlowa.Service.DTO.Lesson;

namespace KlikajZGlowa.Service.Interface
{
    public interface ILessonService
    {
        Task<List<LessonDto>> GetAllAsync();
        Task<LessonDto?> GetByIdAsync(int LessonId);
        Task<DetailsLessonDto?> GetDetailsByIdAsync(int LessonId);
        Task<List<LessonDto>> GetByCourseIdAsync(int CourseId);        
        Task<bool> CreateAsync(CreateLessonDto createLessonDto);
        Task<bool> UpdateAsync(UpdateLessonDto updateLessonDto);
        Task<bool> DeleteAsync(int LessonId);

    }
}

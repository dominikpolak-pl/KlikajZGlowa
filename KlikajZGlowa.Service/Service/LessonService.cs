using AutoMapper;
using AutoMapper.QueryableExtensions;
using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.Lesson;
using KlikajZGlowa.Service.Interface;
using KlikajZGlowa.Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using System;
using System.Collections.Generic;


namespace KlikajZGlowa.Service.Service
{
    public class LessonService : BaseService, ILessonService
    {
        public LessonService(Db dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor) 
            : base(dbContext, mapper, httpContextAccessor) { }

        public async Task<List<LessonDto>> GetAllAsync()
        {
            return await _dbContext.Lessons
                .AsNoTracking()
                .OrderBy(l => l.Title)
                .ProjectTo<LessonDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<LessonDto?> GetByIdAsync(int LessonId)
        {
            return await _dbContext.Lessons
                .AsNoTracking()
                .Where(l => l.LessonId == LessonId)
                .ProjectTo<LessonDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<DetailsLessonDto?> GetDetailsByIdAsync(int LessonId)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId)) return null;

            var hasAccess = await _dbContext.Lessons
                .AsNoTracking()
                .AnyAsync(l => l.LessonId == LessonId && (
                    l.Course!.Authors.Any(a => a.ApplicationUserId == userId) ||
                    l.Course.EnrolledUsers.Any(e => e.ApplicationUserId == userId)
                ));

            if (!hasAccess)
            {
                return null;
            }

            return await _dbContext.Lessons
                .AsNoTracking()
                .Where(l => l.LessonId == LessonId)
                .ProjectTo<DetailsLessonDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<List<LessonDto>> GetByCourseIdAsync(int CourseId)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby dodać lekcję.");
            }

            var hasAccess = await _dbContext.Courses.AsNoTracking().AnyAsync(c => c.CourseId == CourseId && (
                c.Authors.Any(a => a.ApplicationUserId == userId)
                || c.EnrolledUsers.Any(e => e.ApplicationUserId == userId)));

            if(!hasAccess)
            {
                throw new UnauthorizedAccessException("Nie masz dostępu do tego kursu.");
            }

            return await _dbContext.Lessons
                .AsNoTracking()
                .Where(l => l.CourseId == CourseId)
                .OrderBy(l => l.LessonId)
                .ProjectTo<LessonDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(CreateLessonDto createLessonDto)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby dodać lekcję.");
            }

            var isCourseAuthor = await _dbContext.Courses
                .AnyAsync(c => c.CourseId == createLessonDto.CourseId &&
                               c.Authors.Any(a => a.ApplicationUserId == userId));

            if (!isCourseAuthor)
            {
                return false;
            }

            var lesson = _mapper.Map<Lesson>(createLessonDto);
            _dbContext.Lessons.Add(lesson);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        

        public async Task<bool> UpdateAsync(UpdateLessonDto updateLessonDto)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby edytować lekcję.");
            }

            var lesson = await _dbContext.Lessons
                .Include(l => l.Course)
                    .ThenInclude(c => c!.Authors)
                .FirstOrDefaultAsync(l => l.LessonId == updateLessonDto.LessonId &&
                                          l.Course!.Authors.Any(a => a.ApplicationUserId == userId));

            if (lesson == null)
            {
                return false;
            }
            _mapper.Map(updateLessonDto, lesson);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int LessonId)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby usunąć lekcję.");
            }

            var lesson = await _dbContext.Lessons
                .Include(l => l.Course)
                    .ThenInclude(c => c!.Authors)
                .FirstOrDefaultAsync(l => l.LessonId == LessonId);

            if (lesson == null)
            {
                return false;
            }

            var isAuthor = lesson.Course?.Authors.Any(c => c.ApplicationUserId == userId) ?? false;
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isAuthor && !isAdmin)
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tej lekcji.");
            }

            _dbContext.Lessons.Remove(lesson);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        
    }
}

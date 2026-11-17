using KlikajZGlowa.Service.Interface;
using AutoMapper;
using KlikajZGlowa.Database;
using KlikajZGlowa.Service.DTO.Course;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using KlikajZGlowa.Model;
using Microsoft.AspNetCore.Http;

namespace KlikajZGlowa.Service.Service
{
    public class CourseService : BaseService, ICourseService
    {
        public CourseService(Db dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor) 
            : base(dbContext, mapper, httpContextAccessor) { }

        public async Task<List<CourseDto>> GetAllAsync()
        {
            var courses = await _dbContext.Courses
                .AsNoTracking()
                .OrderBy(c => c.Title)
                .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId) || courses.Count == 0)
            {
                return courses;
            }

            var accessibleCourseIds = await _dbContext.Courses
                .AsNoTracking()
                .Where(c => c.Authors.Any(a => a.ApplicationUserId == userId)
                    || c.EnrolledUsers.Any(e => e.ApplicationUserId == userId))
                .Select(c => c.CourseId)
                .ToListAsync();

            foreach (var course in courses)
            {
                course.IsEnrolled = accessibleCourseIds.Contains(course.CourseId);
            }

            return courses;
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto createCourseDto)
        {
            var userId = GetCurrentUserId;
            if(string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowanym użytkownikiem, aby stworzyć kurs.");
            }

            var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            if (author == null)
            {
                throw new UnauthorizedAccessException("Tylko konta z uprawnieniami autora mogą tworzyć kursy.");
            }

            var course = _mapper.Map<Course>(createCourseDto);

            course.Authors.Add(author);

            _dbContext.Courses.Add(course);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<CourseDto>(course);
        }

        public async Task<CourseDto?> GetByIdAsync(int CourseId)
        {
            return await _dbContext.Courses
                .AsNoTracking()
                .Where(c => c.CourseId == CourseId)
                .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<DetailsCourseDto?> GetDetailsByIdAsync(int CourseId)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowanym użytkownikiem, aby wejść na kurs.");
            }

            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            var hasAccess = isAdmin || await _dbContext.Courses.AsNoTracking().AnyAsync(c => c.CourseId == CourseId && (
                c.Authors.Any(a => a.ApplicationUserId == userId)
                || c.EnrolledUsers.Any(e => e.ApplicationUserId == userId)));

            if(!hasAccess)
            {
                return null;
            }

            var details = await _dbContext.Courses
                .AsNoTracking()
                .Where(c => c.CourseId == CourseId)
                .ProjectTo<DetailsCourseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (details != null)
            {
                details.IsEnrolled = true;
                details.IsAuthor = isAdmin || await _dbContext.Courses
                    .AsNoTracking()
                    .AnyAsync(c => c.CourseId == CourseId
                        && c.Authors.Any(a => a.ApplicationUserId == userId));
            }

            return details;
        }

        

        public async Task<bool> UpdateAsync(int CourseId, UpdateCourseDto updateCourseDto)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowanym użytkownikiem, aby edytować kurs.");
            }

            var course = await _dbContext.Courses.Include(c => c.Authors).FirstOrDefaultAsync(c => c.CourseId == CourseId);

            if (course == null)
            {
                return false;
            }

            var isAuthor = course.Authors.Any(c => c.ApplicationUserId == userId);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isAuthor && !isAdmin)
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do edycji tego kursu.");
            }
            _mapper.Map(updateCourseDto, course);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int courseId)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowanym użytkownikiem, aby usunąć kurs.");
            }

            var course = await _dbContext.Courses
                .Include(c => c.Authors)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return false;
            }

            var isAuthor = course.Authors.Any(a => a.ApplicationUserId == userId);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isAuthor && !isAdmin)
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tego kursu.");
            }

            var hasLesson = await _dbContext.Lessons.AnyAsync(l => l.CourseId == courseId);
            if (hasLesson)
            {
                throw new InvalidOperationException("Nie można usunąć kursu z przypisanymi lekcjami.");
            }
            _dbContext.Courses.Remove(course);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EnrollCurrentUserAsync(int courseId)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowanym użytkownikiem, aby dołączyć do kursu.");
            }

            var course = await _dbContext.Courses.Include(c => c.EnrolledUsers)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null) return false;

            var enrolledUser = await _dbContext.EnrolledUsers
                .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);

            if (enrolledUser == null)
            {
                enrolledUser = new EnrolledUser { ApplicationUserId = userId };
                await _dbContext.EnrolledUsers.AddAsync(enrolledUser);
            }

            if (!course.EnrolledUsers.Any(e => e.ApplicationUserId == userId))
            {
                course.EnrolledUsers.Add(enrolledUser);
                await _dbContext.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> AddAuthorAsync(int courseId, int authorId)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany.");
            }

            var course = await _dbContext.Courses
                .Include(c => c.Authors)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return false;
            }

            var isAuthor = course.Authors.Any(a => a.ApplicationUserId == userId);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isAuthor && !isAdmin)
            {
                return false;
            }

            var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.AuthorId == authorId);
            if (author == null)
            {
                return false;
            }

            if (!course.Authors.Any(a => a.AuthorId == authorId))
            {
                course.Authors.Add(author);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> RemoveAuthorAsync(int courseId, int authorId)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany.");
            }

            var course = await _dbContext.Courses
                .Include(c => c.Authors)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if(course == null)
            {
                return false;
            }

            var isAuthor = course.Authors.Any(a => a.ApplicationUserId == userId);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isAuthor && !isAdmin)
            {
                return false;
            }

            var author = course.Authors
                .FirstOrDefault(a => a.AuthorId == authorId);

            if (author == null)
            {
                return false;
            }

            if (course.Authors.Count <= 1)
            {
                return false;
            }
            if (!isAdmin && author.ApplicationUserId == userId)
            {
                return false;
            }

            if (course.Authors.Any(a => a.AuthorId == authorId))
            {
                course.Authors.Remove(author);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }
    }
}

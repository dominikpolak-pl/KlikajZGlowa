using AutoMapper.QueryableExtensions;
using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.AuthorApplication;
using KlikajZGlowa.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace KlikajZGlowa.Service.Service
{
    public class AuthorApplicationService : BaseService, IAuthorApplicationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorApplicationService(
            Db db,
            AutoMapper.IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager) : base(db, mapper, httpContextAccessor)
        {
            _userManager = userManager;
        }
        public async Task<bool> GetMyApplicationStatusAsync()
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            return await _dbContext.AuthorApplications
                .AnyAsync(a => a.ApplicationUserId == userId && a.Status == ApplicationStatus.Pending);
        }

        public async Task<bool> GetApplicationStatusByIdAsync(string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var currentUserId = GetCurrentUserId;
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;
            if (currentUserId != userId && !isAdmin)
            {
                throw new UnauthorizedAccessException("Brak uprawnień do sprawdzenia statusu wniosku.");
            }
            
            return await _dbContext.AuthorApplications
                .AnyAsync(a => a.ApplicationUserId == userId && a.Status == ApplicationStatus.Pending);
        }

        public async Task<List<AuthorApplicationDto>> GetPendingApplicationsAsync()
        {
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Tylko administrator może zarządzać wnioskami.");
            }

            return await _dbContext.AuthorApplications
                .AsNoTracking()
                .Include(a => a.ApplicationUser)
                .Where(a => a.Status == ApplicationStatus.Pending)
                .OrderByDescending(a => a.CreatedAt)
                .ProjectTo<AuthorApplicationDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<bool> SubmitApplicationAsync(CreateAuthorApplicationDto dto)
        {
            var currentUser = GetCurrentUserId;
            if(string.IsNullOrEmpty(currentUser))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany.");
            }

            var isAuthor = await _dbContext.Authors.AnyAsync(a => a.ApplicationUserId == currentUser);
            if (isAuthor)
            {
                throw new InvalidOperationException("Posiadasz już uprawnienia autora.");
            }

            var hasPending = await _dbContext.AuthorApplications.AnyAsync(a => a.ApplicationUserId == currentUser && a.Status == ApplicationStatus.Pending);
            if (hasPending)
            {
                throw new InvalidOperationException("Twoje poprzednie zgłoszenie jest jeszcze w trakcie weryfikacji.");
            }

            var authorApplication = _mapper.Map<AuthorApplication>(dto);
            authorApplication.ApplicationUserId = currentUser;

            _dbContext.AuthorApplications.Add(authorApplication);
            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> ApproveApplicationAsync(int applicationId)
        {
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Tylko administrator może zarządzać wnioskami.");
            }

            var application = await _dbContext.AuthorApplications
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null || application.Status != ApplicationStatus.Pending)
            {
                return false;
            }

            var authorExist = await _dbContext.Authors.AnyAsync(a => a.ApplicationUserId == application.ApplicationUserId);
            if (!authorExist)
            {
                var author = _mapper.Map<Author>(application.ApplicationUser);
                _dbContext.Authors.Add(author);
            }

            var user = await _userManager.FindByIdAsync(application.ApplicationUserId);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, AppRoles.Author))
                {
                    await _userManager.AddToRoleAsync(user, AppRoles.Author);
                }
            }

            application.Status = ApplicationStatus.Approved;
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectApplicationAsync(int applicationId, RejectAuthorApplicationDto dto)
        {
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Tylko administrator może zarządzać wnioskami.");
            }

            var application = await _dbContext.AuthorApplications
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null || application.Status != ApplicationStatus.Pending)
            {
                return false;
            }

            application.Status = ApplicationStatus.Rejected;
            application.RejectionReason = dto.Reason;

            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
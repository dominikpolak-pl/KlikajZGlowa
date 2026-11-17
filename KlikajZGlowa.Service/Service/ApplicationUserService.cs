using AutoMapper;
using AutoMapper.QueryableExtensions;
using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.ApplicationUser;
using KlikajZGlowa.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace KlikajZGlowa.Service.Service
{
    public class ApplicationUserService : BaseService, IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;


        public ApplicationUserService(
            Db db,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) : base(db, mapper, httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ApplicationUserDto?> GetUserByIdAsync(string userId)
        {
            var currentUserId = GetCurrentUserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby zobaczyć szczegóły profilu.");
            }

            var isOwner = userId == currentUserId;
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do przeglądania tego profilu.");
            }

            var applicationUser = await _dbContext.ApplicationUsers
                .Include(u => u.EnrolledUser)
                .Include(u => u.Author)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (applicationUser == null) return null;

            return _mapper.Map<ApplicationUserDto>(applicationUser);
        }

        public async Task<ApplicationUserDto?> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId)) return null;

            return await GetUserByIdAsync(userId);
        }

        public async Task<List<ApplicationUserDto>> GetAllUsersAsync()
        {
            var currentUserId = GetCurrentUserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby pobrać listę użytkowników.");
            }

            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Tylko administrator może przeglądać pełną listę użytkowników.");
            }

            return await _dbContext.ApplicationUsers
                .AsNoTracking()
                .OrderBy(u => u.Email)
                .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ApplicationUserDto?> UpdateCurrentUserAsync(string userId, UpdateApplicationUserDto updateDto)
        {
            var currentUserId = GetCurrentUserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby edytować profil.");
            }

            if (updateDto == null) return null;


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var isOwner = currentUserId == userId;
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do edycji tego profilu.");
            }

            _mapper.Map(updateDto, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return null;
            }

            return _mapper.Map<ApplicationUserDto>(user);
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordApplicationUserDto changePasswordDto)
        {
            if (string.IsNullOrEmpty(userId)) return false;

            var currentUserId = GetCurrentUserId;
            if (currentUserId != userId) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            return result.Succeeded;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginApplicationUserDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return null;

            var accessToken = await _tokenService.CreateAccessTokenAsync(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            return new AuthResponseDto
            {
                TokenType = "Bearer",
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<bool> RegisterAsync(CreateApplicationUserDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return false;
            }

            var user = _mapper.Map<ApplicationUser>(registerDto);
            user.EnrolledUser = new EnrolledUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return false;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.EnrolledUser);

            if (!roleResult.Succeeded) { }

            return result.Succeeded;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            try
            {
                var decodedBytes = WebEncoders.Base64UrlDecode(token);
                var normalToken = Encoding.UTF8.GetString(decodedBytes);
                var result = await _userManager.ConfirmEmailAsync(user, normalToken);
                return result.Succeeded;
            }
            catch
            {
                var decodedToken = System.Net.WebUtility.UrlDecode(token);
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                return result.Succeeded;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return new List<string>();

            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Tylko administrator może zarządzać rolami.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return new List<string>();
            var rules = await _userManager.GetRolesAsync(user);
            return rules.ToList();
        }

        public async Task<bool> UpdateUserRolesAsync(string userId, List<string> roles)
        {
            var currentUserId = GetCurrentUserId;

            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Tylko administrator może zarządzać rolami.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var distinctRoles = roles?.Distinct().Where(r => !string.IsNullOrWhiteSpace(r)).ToList() ?? new List<string>();
            if (!distinctRoles.Contains(AppRoles.EnrolledUser))
            {
                distinctRoles.Add(AppRoles.EnrolledUser);
            }
            if (!distinctRoles.Any())
            {
                throw new InvalidOperationException("Użytkownik musi posiadać przynajmniej jedną rolę w systemie.");
            }

            if (user.Id == currentUserId && !distinctRoles.Contains(AppRoles.Admin))
            {
                throw new InvalidOperationException("Nie możesz odebrać sam sobie uprawnień administratora.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded) return false;

            var addResult = await _userManager.AddToRolesAsync(user, distinctRoles);
            if (!addResult.Succeeded) return false;

            if (distinctRoles.Contains(AppRoles.Author))
            {
                var authorExist = await _dbContext.Authors.AnyAsync(a => a.ApplicationUserId == user.Id);
                if (!authorExist)
                {
                    var author = new Author
                    {
                        ApplicationUserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    };
                    _dbContext.Authors.Add(author);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var currentUserId = GetCurrentUserId;
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Tylko administrator może usuwać użytkowników.");
            }
            if (userId == currentUserId)
            {
                throw new InvalidOperationException("Nie możesz usunąć swojego własnego konta.");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);
            if (author != null)
            {
                var hasCourses = await _dbContext.Courses.AnyAsync(c => c.Authors.Any(a => a.AuthorId == author.AuthorId));
                if (hasCourses)
                {
                    throw new InvalidOperationException("Nie można usunąć użytkownika, ponieważ jest autorem istniejących kursów.");
                }
                _dbContext.Authors.Remove(author);
            }

            var applications = await _dbContext.AuthorApplications.Where(a => a.ApplicationUserId == userId).ToListAsync();
            if (applications.Any()) _dbContext.AuthorApplications.RemoveRange(applications);
            
            var enrolled = await _dbContext.EnrolledUsers.FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
            if (enrolled != null) _dbContext.EnrolledUsers.Remove(enrolled);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _dbContext.SaveChangesAsync();

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

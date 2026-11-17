using AutoMapper;
using AutoMapper.QueryableExtensions;
using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.EnrolledUser;
using KlikajZGlowa.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;

namespace KlikajZGlowa.Service.Service
{
    public class EnrolledUserService : BaseService, IEnrolledUserService
    {
        public EnrolledUserService(Db db, IMapper mapper, IHttpContextAccessor _httpContextAccessor) : base(db, mapper, _httpContextAccessor) { }

        public async Task<List<EnrolledUserDto>> GetAllAsync()
        {
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do wyświetlenia wszystkich profili.");
            }

            return await _dbContext.EnrolledUsers
                .AsNoTracking()
                .OrderBy(e => e.LastName)
                .ProjectTo<EnrolledUserDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<EnrolledUserDto?> GetByIdAsync(int id)
        {
            return await _dbContext.EnrolledUsers
                .AsNoTracking()
                .Where(e => e.EnrolledUserId == id)
                .ProjectTo<EnrolledUserDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
        public async Task<DetailsEnrolledUserDto?> GetDetailsByIdAsync(int id)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby zobaczyć szczegóły profilu.");
            }
            var enrolled = await _dbContext.EnrolledUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrolledUserId == id);

            if (enrolled == null) return null;

            var isOwner = enrolled.ApplicationUserId == userId;
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do przeglądania tego profilu.");
            }

            return _mapper.Map<DetailsEnrolledUserDto>(enrolled);
        }

        public async Task<DetailsEnrolledUserDto?> GetMyProfileAsync()
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return await _dbContext.EnrolledUsers
                .AsNoTracking()
                .Where(e => e.ApplicationUserId == userId)
                .ProjectTo<DetailsEnrolledUserDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<EnrolledUserDto> CreateAsync(CreateEnrolledUserDto createEnrolledUserDto)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby utworzyć profil kursanta.");
            }

            var alreadyExists = await _dbContext.EnrolledUsers
                .AnyAsync(e => e.ApplicationUserId == userId);

            if (alreadyExists)
            {
                throw new InvalidOperationException("Twoje konto ma już przypisany profil kursanta.");
            }

            var enrolled = _mapper.Map<EnrolledUser>(createEnrolledUserDto);
            enrolled.ApplicationUserId = userId;

            _dbContext.EnrolledUsers.Add(enrolled);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<EnrolledUserDto>(enrolled);
        }

        public async Task<bool> UpdateAsync(int id, UpdateEnrolledUserDto updateEnrolledUserDto)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby edytować profil kursanta.");
            }

            var enrolled = await _dbContext.EnrolledUsers.FirstOrDefaultAsync(e => e.EnrolledUserId == id);
            if (enrolled == null) return false;

            var isOwner = enrolled.ApplicationUserId == userId;
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do edycji tego profilu kursanta.");
            }

            _mapper.Map(updateEnrolledUserDto, enrolled);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby usunąć profil kursanta.");
            }

            var enrolled = await _dbContext.EnrolledUsers
                .Include(e => e.Courses)
                .FirstOrDefaultAsync(e => e.EnrolledUserId == id);

            if (enrolled == null)
            {
                return false;
            }

            var isOwner = enrolled.ApplicationUserId == userId;
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(AppRoles.Admin) ?? false;

            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tego profilu kursanta.");
            }            

            enrolled.Courses.Clear();

            _dbContext.EnrolledUsers.Remove(enrolled);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

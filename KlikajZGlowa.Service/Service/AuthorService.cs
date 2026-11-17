using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.Author;
using KlikajZGlowa.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper.QueryableExtensions;


namespace KlikajZGlowa.Service.Service
{
    public class AuthorService : BaseService, IAuthorService
    {
        public AuthorService(Db db, IMapper mapper, IHttpContextAccessor httpContextAccessor) 
            : base(db, mapper, httpContextAccessor) { }

        public async Task<List<AuthorDto>> GetAllAsync()
        {
            return await _dbContext.Authors
                .AsNoTracking()
                .OrderBy(a => a.LastName)
                .ProjectTo<AuthorDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<AuthorDto?> GetByIdAsync(int id)
        {
            return await _dbContext.Authors
                .AsNoTracking()
                .Where(a => a.AuthorId == id)
                .ProjectTo<AuthorDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<DetailsAuthorDto?> GetDetailsByIdAsync(int AuthorId)
        {
            return await _dbContext.Authors
                .AsNoTracking()
                .Where(a => a.AuthorId == AuthorId)
                .ProjectTo<DetailsAuthorDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<DetailsAuthorDto?> GetMyProfileAsync()
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId)) return null;

            return await _dbContext.Authors
                .AsNoTracking()
                .Where(a => a.ApplicationUserId == userId)
                .ProjectTo<DetailsAuthorDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
        public async Task<AuthorDto> CreateAsync(CreateAuthorDto createAuthorDto)
        {
            var userId = GetCurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowanym użytkownikiem, aby stworzyć kurs.");
            }

            var alreadyExists = await _dbContext.Authors
                .AnyAsync(a => a.ApplicationUserId == userId);

            if (alreadyExists)
            {
                throw new InvalidOperationException("Twoje konto ma już przypisany profil autora.");
            }

            var author = _mapper.Map<Author>(createAuthorDto);
            author.ApplicationUserId = userId;

            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<AuthorDto>(author);
        }
        public async Task<bool> UpdateAsync(int AuthorId, UpdateAuthorDto updateAuthorDto)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby edytować profil autora.");
            }

            var author = await _dbContext.Authors
                .FirstOrDefaultAsync(a => a.AuthorId == AuthorId && 
                a.ApplicationUserId == userId);

            if (author == null) return false;

            _mapper.Map(updateAuthorDto, author);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int AuthorId)
        {
            var userId = GetCurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Musisz być zalogowany, aby usunąć profil autora.");
            }

            var author = await _dbContext.Authors
                .Include(a => a.Courses)
                .FirstOrDefaultAsync(a => a.AuthorId == AuthorId && a.ApplicationUserId == userId);

            if (author == null) return false;

            if (author.Courses.Any())
            {
                throw new InvalidOperationException("Nie można usunąć profilu autora, który posiada przypisane kursy.");
            }

            _dbContext.Authors.Remove(author);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        
    }
}

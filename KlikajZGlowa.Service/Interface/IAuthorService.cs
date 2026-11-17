using KlikajZGlowa.Service.DTO.Author;
using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.Interface
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAllAsync();
        Task<AuthorDto?> GetByIdAsync(int id);
        Task<DetailsAuthorDto?> GetDetailsByIdAsync(int AuthorId);
        Task<DetailsAuthorDto?> GetMyProfileAsync();
        Task<AuthorDto> CreateAsync(CreateAuthorDto createAuthorDto);
        Task<bool> UpdateAsync(int AuthorId, UpdateAuthorDto updateAuthorDto);
        Task<bool> DeleteAsync(int AuthorId);
        
    }
}

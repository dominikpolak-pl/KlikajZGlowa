using KlikajZGlowa.Service.DTO.EnrolledUser;
using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.Interface
{
    public interface IEnrolledUserService
    {
        Task<List<EnrolledUserDto>> GetAllAsync();
        Task<EnrolledUserDto?> GetByIdAsync(int id);
        Task<DetailsEnrolledUserDto?> GetDetailsByIdAsync(int id);
        Task<DetailsEnrolledUserDto?> GetMyProfileAsync();
        Task<EnrolledUserDto> CreateAsync(CreateEnrolledUserDto createEnrolledUserDto);
        Task<bool> UpdateAsync(int id, UpdateEnrolledUserDto updateEnrolledUserDto);
        Task<bool> DeleteAsync(int id);
    }
}

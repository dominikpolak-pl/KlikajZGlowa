using System;
using System.Collections.Generic;
using System.Text;
using KlikajZGlowa.Service.DTO.ApplicationUser;

namespace KlikajZGlowa.Service.Interface
{
    public interface IApplicationUserService
    {
        Task<ApplicationUserDto?> GetCurrentUserAsync();
        Task<ApplicationUserDto?> UpdateCurrentUserAsync(string userId,  UpdateApplicationUserDto updateDto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordApplicationUserDto changePasswordDto);
        Task<AuthResponseDto?> LoginAsync(LoginApplicationUserDto loginDto);
        Task LogoutAsync();
        Task<bool> RegisterAsync(CreateApplicationUserDto registerDto);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<ApplicationUserDto?> GetUserByIdAsync(string userId);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<List<ApplicationUserDto>> GetAllUsersAsync();
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<bool> UpdateUserRolesAsync(string userId, List<string> roles);
        Task<bool> DeleteUserAsync(string userId);
    }
}

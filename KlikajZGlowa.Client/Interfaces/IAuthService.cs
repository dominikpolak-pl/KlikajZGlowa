using KlikajZGlowa.Service.DTO.ApplicationUser;

namespace KlikajZGlowa.Client.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginApplicationUserDto dto);
        Task<(bool Success, string? ErrorMessage)> RegisterAsync(CreateApplicationUserDto dto);
        Task LogoutAsync();
        Task<ApplicationUserDto?> GetAsync();
        Task<bool> UpdateAsync(string id, UpdateApplicationUserDto dto);
        Task<List<ApplicationUserDto>> GetAllAsync();
        Task<ApplicationUserDto?> GetByIdAsync(string id);
        Task<List<string>> GetRolesAsync(string id);
        Task<(bool Success, string? ErrorMessage)> UpdateRolesAsync(string id, List<string> roles);
        Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(string id);

    }
}

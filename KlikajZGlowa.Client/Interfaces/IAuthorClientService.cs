using KlikajZGlowa.Service.DTO.Author;
namespace KlikajZGlowa.Client.Interfaces
{
    public interface IAuthorClientService
    {
        Task<CreateAuthorDto?> CreateAsync(CreateAuthorDto dto);
    }
}

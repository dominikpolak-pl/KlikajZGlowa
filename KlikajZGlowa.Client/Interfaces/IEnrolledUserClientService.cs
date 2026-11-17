using KlikajZGlowa.Service.DTO.EnrolledUser;

namespace KlikajZGlowa.Client.Interfaces
{
    public interface IEnrolledUserClientService
    {
        Task<List<EnrolledUserDto>> GetAll();
        Task<DetailsEnrolledUserDto?> GetById(int id);
    }
}

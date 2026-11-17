using KlikajZGlowa.Service.DTO.AuthorApplication;


namespace KlikajZGlowa.Client.Interfaces
{
    public interface IAuthorApplicationClientService
    {
        Task<List<AuthorApplicationDto>> GetPendingApplicationsAsync();
        Task<bool> ApproveAsync(int id);
        Task<bool> RejectAsync(int id, string reason);
        Task<bool> SubmitAsync(string motivation);
        Task<bool> HasPendingApplicationAsync();
        Task<bool> HasPendingApplicationByIdAsync(string userId);
    }
}

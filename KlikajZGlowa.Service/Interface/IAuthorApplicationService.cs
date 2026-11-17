using KlikajZGlowa.Service.DTO.AuthorApplication;
using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.Interface
{
    public interface IAuthorApplicationService
    {
        Task<bool> GetMyApplicationStatusAsync();
        Task<bool> GetApplicationStatusByIdAsync(string userId);
        Task<List<AuthorApplicationDto>> GetPendingApplicationsAsync();
        Task<bool> SubmitApplicationAsync(CreateAuthorApplicationDto dto);
        Task<bool> ApproveApplicationAsync(int applicationId);
        Task<bool> RejectApplicationAsync(int applicationId, RejectAuthorApplicationDto dto);
    }
}

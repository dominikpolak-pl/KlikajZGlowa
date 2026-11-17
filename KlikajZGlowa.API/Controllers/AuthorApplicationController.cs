using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.AuthorApplication;
using KlikajZGlowa.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KlikajZGlowa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorApplicationController : ControllerBase
    {
        private readonly IAuthorApplicationService _applicationService;

        public AuthorApplicationController(IAuthorApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet("my-status")]
        [Authorize]
        public async Task<IActionResult> GetMyApplicationStatus()
        {
            var status = await _applicationService.GetMyApplicationStatusAsync();
            return Ok(new { hasPending = status });
        }

        [HttpGet("{userId}/status")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> GetApplicationStatusById(string userId)
        {
            try
            {
                var status = await _applicationService.GetApplicationStatusByIdAsync(userId);
                return Ok(new { hasPending = status });
            }   
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Submit([FromBody] CreateAuthorApplicationDto dto)
        {
            try
            {
                var result = await _applicationService.SubmitApplicationAsync(dto);
                return Ok(new { message = "Zgłoszenie zostało pomyślnie wysłane." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpGet("pending")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var list = await _applicationService.GetPendingApplicationsAsync();
                return Ok(list);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/approve")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var success = await _applicationService.ApproveApplicationAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Nie odnaleziono oczekującego wniosku o podanym ID." });
                }

                return Ok(new { message = "Wniosek został zaakceptowany, uprawnienia nadane." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/reject")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectAuthorApplicationDto dto)
        {
            try
            {
                var success = await _applicationService.RejectApplicationAsync(id, dto);
                if (!success)
                {
                    return NotFound(new { message = "Nie odnaleziono oczekującego wniosku o podanym ID." });
                }

                return Ok(new { message = "Wniosek został odrzucony." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }
    }
}

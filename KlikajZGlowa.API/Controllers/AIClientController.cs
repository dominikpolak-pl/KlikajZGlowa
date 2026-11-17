using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KlikajZGlowa.Service.Interface;
using KlikajZGlowa.Service.DTO.Lesson;
using KlikajZGlowa.Model;

namespace KlikajZGlowa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{AppRoles.Admin}, {AppRoles.Author}")]
    public class AIClientController : ControllerBase
    {
        private readonly IAiCourseService _aiCourseService;
        public AIClientController(IAiCourseService aiCourseService)
        {
            _aiCourseService = aiCourseService;
        }

        [HttpPost("generate-lesson")]
        public async Task<IActionResult> GenerateLessonDraft([FromBody] GenerateLessonRequestDto request)
        {
            try
            {
                var result = await _aiCourseService.GenerateLessonDraftAsync(request);
                return Ok(result);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd serwera.", details = ex.Message });
            }
        }
    }
}

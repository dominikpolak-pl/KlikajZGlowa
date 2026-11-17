using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.Lesson;
using KlikajZGlowa.Service.Interface;
using KlikajZGlowa.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;


namespace KlikajZGlowa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _lessonService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var lesson = await _lessonService.GetByIdAsync(id);
            if (lesson == null) return NotFound();
            return Ok(lesson);
        }

        [HttpGet("{id}/details")]
        [Authorize]
        public async Task<IActionResult> GetDetails(int id)
        {
            var details = await _lessonService.GetDetailsByIdAsync(id);
            if (details == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Nie masz uprawnień do przeglądania treści tej lekcji lub lekcja nie istnieje." });
            }
            return Ok(details);
        }

        [HttpGet("course/{courseId}")]
        [Authorize]
        public async Task<IActionResult> GetByCourseId(int courseId)
        {
            var lessons = await _lessonService.GetByCourseIdAsync(courseId);
            if (lessons == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Nie masz uprawnień do przeglądania treści tej lekcji lub lekcja nie istnieje." });
            }
            return Ok(lessons);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateLessonDto createLessonDto)
        {
            try
            {
                var success = await _lessonService.CreateAsync(createLessonDto);
                if (!success)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new { message = "Nie masz uprawnień do dodawania lekcji do tego kursu lub kurs nie istnieje." });
                }

                return Ok(new { message = "Lekcja została pomyślnie utworzona." });

            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Author},{AppRoles.Admin}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLessonDto updateLessonDto)
        {
            if (id != updateLessonDto.LessonId)
                return BadRequest(new { message = "Identyfikator w adresie URL nie zgadza się z identyfikatorem w ciele żądania." });

            try
            {
                var success = await _lessonService.UpdateAsync(updateLessonDto); 
                if(!success)
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new { message = "Nie masz uprawnień do dodawania lekcji do tego kursu lub kurs nie istnieje." });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Author},{AppRoles.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _lessonService.DeleteAsync(id);
                if (!success)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                        new { message = $"Lekcja o ID {id} nie została znaleziona" });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }



    }
}

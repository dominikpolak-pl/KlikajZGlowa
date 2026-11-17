using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.Course;
using KlikajZGlowa.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KlikajZGlowa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {   
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null) return NotFound(new { message = $"Kurs o ID {id} nie został odnaleziony." });
            return Ok(course);
        }

        [HttpGet("{id}/details")]
        [Authorize]
        public async Task<IActionResult> GetDetails(int id)
        {
            var exists = await _courseService.GetByIdAsync(id);
            if (exists == null)
                return NotFound(new { message = $"Kurs o ID {id} nie został odnaleziony." });


            var course = await _courseService.GetDetailsByIdAsync(id);
            if (course == null)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Nie posiadasz dostępu do materiałów tego kursu." });
            return Ok(course);
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Author},{AppRoles.Admin}")]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto)
        {
            try {
                var course = await _courseService.CreateAsync(createCourseDto);
                return CreatedAtAction(nameof(GetById), new { id = course.CourseId }, course);
            }catch(UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Author},{AppRoles.Admin}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            if (id != updateCourseDto.CourseId)
            {
                return BadRequest(new { message = "Identyfikator w adresie URL nie zgadza się z identyfikatorem w treści żądania." });
            }

            try
            {
                var course = await _courseService.UpdateAsync(id, updateCourseDto);
                if (!course) 
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "Nie masz uprawnień do edycji tego kursu " +
                        "lub kurs nie istnieje." });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Author},{AppRoles.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _courseService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Kurs o ID {id} nie został odnaleziony." });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }

        [HttpPost("{id}/enroll")]
        [Authorize]
        public async Task<IActionResult> EnrollInCourse(int id)
        {
            var success = await _courseService.EnrollCurrentUserAsync(id);
            if (!success)
                return NotFound(new { message = $"Kurs o ID {id} nie istnieje." });

            return Ok(new { message = "Pomyślnie zapisano na kurs." });
        }

        [HttpPost("{id:int}/authors/{authorId:int}")]
        [Authorize(Roles = $"{AppRoles.Author},{AppRoles.Admin}")]
        public async Task<IActionResult> AddAuthor(int id, int authorId)
        {
            var success = await _courseService.AddAuthorAsync(id, authorId);

            if (!success)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Tylko autor kursu może dodawać współautorów." });
            }

            return Ok(new { message = "Autor został dodany do kursu." });
        }

        [HttpDelete("{id:int}/authors/{authorId:int}")]
        [Authorize(Roles = $"{AppRoles.Author},{AppRoles.Admin}")]
        public async Task<IActionResult> RemoveAuthor(int id, int authorId)
        {
            var success = await _courseService.RemoveAuthorAsync(id, authorId);

            if (!success)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "Tylko autor kursu może usunąć współautorów." });
            }

            return Ok(new { message = "Współautor został usunięty z kursu." });
        }

    }
}

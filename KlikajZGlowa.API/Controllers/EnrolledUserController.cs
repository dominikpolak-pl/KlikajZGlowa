using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.EnrolledUser;
using KlikajZGlowa.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KlikajZGlowa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrolledUserController : ControllerBase
    {
        private readonly IEnrolledUserService _enrolledUserService;

        public EnrolledUserController(IEnrolledUserService enrolledUserService)
        {
            _enrolledUserService = enrolledUserService;
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var enrolledUsers = await _enrolledUserService.GetAllAsync();
            return Ok(enrolledUsers);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var enrolledUser = await _enrolledUserService.GetByIdAsync(id);
            if (enrolledUser == null) return NotFound(new { message = $"Użytkownik zapisany o ID {id} nie został odnaleziony." });
            return Ok(enrolledUser);
        }

        [HttpGet("{id}/details")]
        [Authorize]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var enrolledUser = await _enrolledUserService.GetDetailsByIdAsync(id);
                if (enrolledUser == null) return NotFound(new { message = $"Użytkownik zapisany o ID {id} nie został odnaleziony." });
                return Ok(enrolledUser);

            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var user = await _enrolledUserService.GetMyProfileAsync();
            if (user == null)
            {
                return NotFound(new { message = "Twoje konto nie posiada jeszcze powiązanego profilu kursanta." });
            }

            return Ok(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateEnrolledUserDto createDto)
        {
            try
            {
                var created = await _enrolledUserService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = created.EnrolledUserId }, created);
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

        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.EnrolledUser},{AppRoles.Admin}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEnrolledUserDto updateDto)
        {
            if (id != updateDto.EnrolledUserId)
            {
                return BadRequest(new { message = "Identyfikator w adresie URL nie zgadza się z identyfikatorem w treści żądania." });
            }

            try
            {
                var success = await _enrolledUserService.UpdateAsync(id, updateDto);
                if (!success)
                {
                    return NotFound(new { message = $"Profil kursanta o ID {id} nie został odnaleziony." });
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }

            return NoContent();
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.EnrolledUser},{AppRoles.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _enrolledUserService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = $"Profil kursanta o ID {id} nie został odnaleziony." });
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

using KlikajZGlowa.Service.DTO.ApplicationUser;
using KlikajZGlowa.Service.Interface;
using KlikajZGlowa.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace KlikajZGlowa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IApplicationUserService _applicationUserService;

        public ApplicationUserController(IApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _applicationUserService.GetCurrentUserAsync();
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono profilu użytkownika." });
                }
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.EnrolledUser},{AppRoles.Author}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return BadRequest("Nie można znaleźć użytkownika bez ID.");
                var user = await _applicationUserService.GetUserByIdAsync(id);
                if (user == null) return NotFound(new { message = "Nie znaleziono profilu użytkownika." });
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _applicationUserService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Author},{AppRoles.EnrolledUser}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateApplicationUserDto userUpdateModel)
        {

            if (string.IsNullOrEmpty(id)) return BadRequest("User ID cannot be null or empty.");

            try
            {
                var result = await _applicationUserService.UpdateCurrentUserAsync(id, userUpdateModel);
                if (result == null) return NotFound(new { message = "Użytkownik nie został odnaleziony lub aktualizacja się nie powiodła." });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordApplicationUserDto changePasswordDto)
        {
            try
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

                var ok = await _applicationUserService.ChangePasswordAsync(currentUserId, changePasswordDto);
                if (!ok) return BadRequest(new { message = "Zmiana hasła nie powiodła się. Sprawdź poprawność obecnego hasła." });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginApplicationUserDto loginDto)
        {
            if (loginDto == null) return BadRequest();
            var authResult = await _applicationUserService.LoginAsync(loginDto);

            if (authResult == null)
            {
                return Unauthorized(new { message = "Nieprawidłowy e-mail lub hasło." });
            }

            return Ok(authResult);
        }

        //[HttpPost("logout")]
        //[Authorize]
        //public async Task<IActionResult> Logout()
        //{
        //    await _applicationUserService.LogoutAsync();
        //    return NoContent();
        //}

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateApplicationUserDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _applicationUserService.RegisterAsync(registerDto);

            if (!success)
            {
                return BadRequest(new { message = "Rejestracja nie powiodła się. Sprawdź, czy email nie jest zajęty lub czy hasło spełnia wymagania." });
            }
            return Ok(new { message = "Konto zostało pomyślnie utworzone." });
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest();
            var ok = await _applicationUserService.ConfirmEmailAsync(userId, token);
            if (!ok) return BadRequest();
            return Ok();
        }

        [HttpGet("check-email")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest();
            var exists = await _applicationUserService.CheckEmailExistsAsync(email);
            return Ok(new { exists });
        }

        [HttpGet("{id}/roles")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            try
            {
                var roles = await _applicationUserService.GetUserRolesAsync(id);
                return Ok(roles);
            }
            catch(UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }            
        }

        [HttpPut("{id}/roles")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> UpdateUserRoles(string id, [FromBody] ManageUserRolesDto dto)
        {
            try
            {
                var success = await _applicationUserService.UpdateUserRolesAsync(id, dto.Roles);
                if (!success)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika lub aktualizacja ról nie powiodła się." });
                }
                return Ok(new { message = "Role zostały zaktualizowane." });
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

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var success = await _applicationUserService.DeleteUserAsync(id);
                if(!success)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika lub usunięcie nie powiodło się." });
                }
                return NoContent();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            } 
        }
    }
}


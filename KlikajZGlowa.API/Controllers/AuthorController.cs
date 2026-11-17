using Microsoft.AspNetCore.Mvc;
using KlikajZGlowa.Service.Interface;
using KlikajZGlowa.Service.DTO.Author;
using Microsoft.AspNetCore.Authorization;


namespace KlikajZGlowa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _authorService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound(new { message = $"Autor o ID {id} nie został odnaleziony." });
            return Ok(author);
        }

        [HttpGet("{id}/details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetails(int id)
        {
            var author = await _authorService.GetDetailsByIdAsync(id);
            if (author == null) return NotFound(new { message = $"Autor o ID {id} nie został odnaleziony." });
            return Ok(author);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var author = await _authorService.GetMyProfileAsync();
            if (author == null)
            {
                return NotFound(new { message = "Twoje konto nie posiada jeszcze powiązanego profilu autora." });
            }
            return Ok(author);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateAuthorDto createAuthorDto)
        {
            try
            {
                var author = await _authorService.CreateAsync(createAuthorDto);
                return CreatedAtAction(nameof(GetById), new { id = author.AuthorId }, author);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { message = ex.Message });
            }

        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorDto updateAuthorDto)
        {
            if (id != updateAuthorDto.AuthorId)
            {
                return BadRequest(new { message = "Identyfikator w adresie URL nie zgadza się z identyfikatorem w treści żądania." });
            }
            var author = await _authorService.UpdateAsync(id, updateAuthorDto);
            if (!author) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Nie masz uprawnień do edycji tego profilu lub profil nie istnieje." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var author = await _authorService.DeleteAsync(id);
                if (!author) 
                    return StatusCode(StatusCodes.Status403Forbidden, new { 
                            message = "Nie masz uprawnień do usunięcia tego profilu lub profil nie istnieje." });
                return NoContent();
            }catch(InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}

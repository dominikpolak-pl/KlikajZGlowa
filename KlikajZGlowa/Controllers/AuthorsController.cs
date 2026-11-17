using Humanizer;
using KlikajZGlowa.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using KlikajZGlowa.Service.DTO.Author;

namespace KlikajZGlowa.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;
        public AuthorsController(IAuthorService authorService) => _authorService = authorService;
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var authors = await _authorService.GetAllAsync();
            return View(authors);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();
            return View(author);
        }
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuthorDto createAuthorDto)
        {
            if (!ModelState.IsValid) return View(createAuthorDto);
            await _authorService.CreateAsync(createAuthorDto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();
            var updateAuthorDto = new UpdateAuthorDto { AuthorId = author.AuthorId, FirstName = author.FirstName, LastName = author.LastName };
            return View(updateAuthorDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAuthorDto updateAuthorDto)
        {
            if (!ModelState.IsValid) return View(updateAuthorDto);
            var updated = await _authorService.UpdateAsync(id, updateAuthorDto);
            if(!updated) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _authorService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return RedirectToAction(nameof(Index));
        }
    }
}

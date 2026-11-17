using System.Linq;
using System.Threading.Tasks;
using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KlikajZGlowa.Controllers
{
    public class CoursesController : Controller
    {
         public CoursesController(Db context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id.Value);
            if (course == null) return NotFound();

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create() => View();

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Course course)
        {
            if (!ModelState.IsValid) return View(course);
            _context.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.FindAsync(id.Value);
            if (course == null) return NotFound();
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Title,Description")] Course course)
        {
            if (id != course.CourseId) return NotFound();
            if (!ModelState.IsValid) return View(course);

            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.CourseId)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id.Value);
            if (course == null) return NotFound();
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id) => _context.Courses.Any(e => e.CourseId == id);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using Task = System.Threading.Tasks.Task;

namespace OmintakProduction.Controllers
{
    [Authorize]
    public class BugReportController : Controller
    {
        private readonly AppDbContext _context;

        public BugReportController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bugReports = await _context.BugReports
                .Include(b => b.Project)
                .Include(b => b.Task)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedToUser)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bugReports);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bugReport = await _context.BugReports
                .Include(b => b.Project)
                .Include(b => b.Task)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedToUser)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (bugReport == null) return NotFound();

            return View(bugReport);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BugReport bugReport)
        {
            if (ModelState.IsValid)
            {
                bugReport.CreatedAt = DateTime.Now;
                _context.Add(bugReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(bugReport);
            return View(bugReport);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport == null) return NotFound();

            await PopulateDropdownsAsync(bugReport);
            return View(bugReport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BugReport bugReport)
        {
            if (id != bugReport.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    bugReport.UpdatedAt = DateTime.Now;
                    _context.Update(bugReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BugReportExists(bugReport.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(bugReport);
            return View(bugReport);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bugReport = await _context.BugReports
                .Include(b => b.Project)
                .Include(b => b.Task)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedToUser)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bugReport == null) return NotFound();

            return View(bugReport);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport != null)
            {
                bugReport.IsDeleted = true;
                bugReport.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BugReportExists(int id)
        {
            return _context.BugReports.Any(e => e.Id == id);
        }

        private async Task PopulateDropdownsAsync(BugReport? bugReport = null)
        {
            ViewData["ProjectId"] = new SelectList(await _context.Project.ToListAsync(), "Id", "ProjectName", bugReport?.ProjectId);
            ViewData["TaskId"] = new SelectList(await _context.Tasks.ToListAsync(), "Id", "Title", bugReport?.TaskId);
            ViewData["ReportedByUserId"] = new SelectList(await _context.User.ToListAsync(), "UserId", "UserName", bugReport?.ReportedByUserId);
            ViewData["AssignedToUserId"] = new SelectList(await _context.User.ToListAsync(), "UserId", "UserName", bugReport?.AssignedToUserId);
        }
    }
}

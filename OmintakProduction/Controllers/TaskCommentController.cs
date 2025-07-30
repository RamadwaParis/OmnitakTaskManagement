using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Security.Claims;

namespace OmintakProduction.Controllers
{
    [Authorize]
    public class TaskCommentController : Controller
    {
        private readonly AppDbContext _context;

        public TaskCommentController(AppDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User?.Identity?.IsAuthenticated == true ? 
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        // GET: TaskComment
        public async Task<IActionResult> Index()
        {
            var currentUserId = GetCurrentUserId();
            var comments = await _context.TaskComment
                .Where(tc => tc.UserId == currentUserId)
                .OrderByDescending(tc => tc.CreatedAt)
                .ToListAsync();

            return View(comments);
        }

        // GET: TaskComment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskComment = await _context.TaskComment
                .FirstOrDefaultAsync(m => m.TaskCommentId == id);
            
            if (taskComment == null)
            {
                return NotFound();
            }

            return View(taskComment);
        }

        // GET: TaskComment/Create
        public IActionResult Create(int? taskId)
        {
            ViewBag.TaskId = taskId;
            return View();
        }

        // POST: TaskComment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,Comment")] TaskComment taskComment)
        {
            if (ModelState.IsValid)
            {
                taskComment.UserId = GetCurrentUserId();
                taskComment.CreatedAt = DateTime.UtcNow;
                
                _context.Add(taskComment);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Comment added successfully!";
                return RedirectToAction("Details", "Task", new { id = taskComment.TaskId });
            }
            
            ViewBag.TaskId = taskComment.TaskId;
            return View(taskComment);
        }

        // GET: TaskComment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskComment = await _context.TaskComment.FindAsync(id);
            if (taskComment == null)
            {
                return NotFound();
            }

            // Only allow editing own comments
            if (taskComment.UserId != GetCurrentUserId())
            {
                return Forbid();
            }

            return View(taskComment);
        }

        // POST: TaskComment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskCommentId,TaskId,UserId,Comment,CreatedAt")] TaskComment taskComment)
        {
            if (id != taskComment.TaskCommentId)
            {
                return NotFound();
            }

            // Only allow editing own comments
            if (taskComment.UserId != GetCurrentUserId())
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    taskComment.IsEdited = true;
                    _context.Update(taskComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskCommentExists(taskComment.TaskCommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                TempData["SuccessMessage"] = "Comment updated successfully!";
                return RedirectToAction("Details", "Task", new { id = taskComment.TaskId });
            }
            return View(taskComment);
        }

        // GET: TaskComment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskComment = await _context.TaskComment
                .FirstOrDefaultAsync(m => m.TaskCommentId == id);
            
            if (taskComment == null)
            {
                return NotFound();
            }

            // Only allow deleting own comments
            if (taskComment.UserId != GetCurrentUserId())
            {
                return Forbid();
            }

            return View(taskComment);
        }

        // POST: TaskComment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskComment = await _context.TaskComment.FindAsync(id);
            if (taskComment != null)
            {
                // Only allow deleting own comments
                if (taskComment.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }

                _context.TaskComment.Remove(taskComment);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Comment deleted successfully!";
                return RedirectToAction("Details", "Task", new { id = taskComment.TaskId });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TaskCommentExists(int id)
        {
            return _context.TaskComment.Any(e => e.TaskCommentId == id);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Task History
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TaskHistoryApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskHistoryApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all task history entries
        /// </summary>
        /// <returns>List of all task history entries</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskHistory>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.TaskHistory.OrderByDescending(th => th.CreatedAt).ToListAsync());
        }

        /// <summary>
        /// Get a specific task history entry by ID
        /// </summary>
        /// <param name="id">Task History ID</param>
        /// <returns>Task history details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskHistory), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var history = await _context.TaskHistory.FindAsync(id);
            if (history == null) return NotFound();
            return Ok(history);
        }

        /// <summary>
        /// Get history for a specific task
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <returns>List of history entries for the specified task</returns>
        [HttpGet("by-task/{taskId}")]
        [ProducesResponseType(typeof(IEnumerable<TaskHistory>), 200)]
        public async Task<IActionResult> GetByTask(int taskId)
        {
            var history = await _context.TaskHistory
                .Where(th => th.TaskId == taskId)
                .OrderByDescending(th => th.CreatedAt)
                .ToListAsync();
            return Ok(history);
        }

        /// <summary>
        /// Get history entries by user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of history entries by the specified user</returns>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<TaskHistory>), 200)]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var history = await _context.TaskHistory
                .Where(th => th.UserId == userId)
                .OrderByDescending(th => th.CreatedAt)
                .ToListAsync();
            return Ok(history);
        }

        /// <summary>
        /// Get history entries by action type
        /// </summary>
        /// <param name="action">History action type</param>
        /// <returns>List of history entries with the specified action</returns>
        [HttpGet("by-action/{action}")]
        [ProducesResponseType(typeof(IEnumerable<TaskHistory>), 200)]
        public async Task<IActionResult> GetByAction(TaskHistoryAction action)
        {
            var history = await _context.TaskHistory
                .Where(th => th.Action == action)
                .OrderByDescending(th => th.CreatedAt)
                .ToListAsync();
            return Ok(history);
        }

        /// <summary>
        /// Get recent history entries (last 7 days)
        /// </summary>
        /// <returns>List of recent history entries</returns>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(IEnumerable<TaskHistory>), 200)]
        public async Task<IActionResult> GetRecent()
        {
            var weekAgo = DateTime.Now.AddDays(-7);
            var history = await _context.TaskHistory
                .Where(th => th.CreatedAt >= weekAgo)
                .OrderByDescending(th => th.CreatedAt)
                .ToListAsync();
            return Ok(history);
        }

        /// <summary>
        /// Get history entries within date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of history entries within the specified date range</returns>
        [HttpGet("by-date-range")]
        [ProducesResponseType(typeof(IEnumerable<TaskHistory>), 200)]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var history = await _context.TaskHistory
                .Where(th => th.CreatedAt >= startDate && th.CreatedAt <= endDate)
                .OrderByDescending(th => th.CreatedAt)
                .ToListAsync();
            return Ok(history);
        }

        /// <summary>
        /// Get activity summary for a task
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <returns>Activity summary including action counts</returns>
        [HttpGet("summary/{taskId}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetTaskSummary(int taskId)
        {
            var history = await _context.TaskHistory
                .Where(th => th.TaskId == taskId)
                .ToListAsync();

            var summary = new
            {
                TaskId = taskId,
                TotalActivities = history.Count,
                LastActivity = history.OrderByDescending(h => h.CreatedAt).FirstOrDefault()?.CreatedAt,
                ActionCounts = history.GroupBy(h => h.Action)
                    .Select(g => new { Action = g.Key.ToString(), Count = g.Count() })
                    .ToList(),
                Contributors = history.Select(h => h.UserId).Distinct().Count()
            };

            return Ok(summary);
        }

        /// <summary>
        /// Get user activity summary
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User activity summary</returns>
        [HttpGet("user-summary/{userId}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetUserSummary(int userId)
        {
            var history = await _context.TaskHistory
                .Where(th => th.UserId == userId)
                .ToListAsync();

            var summary = new
            {
                UserId = userId,
                TotalActivities = history.Count,
                LastActivity = history.OrderByDescending(h => h.CreatedAt).FirstOrDefault()?.CreatedAt,
                ActionCounts = history.GroupBy(h => h.Action)
                    .Select(g => new { Action = g.Key.ToString(), Count = g.Count() })
                    .ToList(),
                TasksWorkedOn = history.Select(h => h.TaskId).Distinct().Count(),
                ActivitiesThisWeek = history.Count(h => h.CreatedAt >= DateTime.Now.AddDays(-7))
            };

            return Ok(summary);
        }

        /// <summary>
        /// Create a manual history entry
        /// </summary>
        /// <param name="history">Task history data</param>
        /// <returns>Created task history entry</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskHistory), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(TaskHistory history)
        {
            history.CreatedAt = DateTime.Now;
            _context.TaskHistory.Add(history);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = history.Id }, history);
        }

        /// <summary>
        /// Delete a history entry (admin only)
        /// </summary>
        /// <param name="id">Task History ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var history = await _context.TaskHistory.FindAsync(id);
            if (history == null) return NotFound();
            _context.TaskHistory.Remove(history);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> TaskHistoryExists(int id)
        {
            return await _context.TaskHistory.AnyAsync(e => e.Id == id);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Task Comments
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TaskCommentApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskCommentApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all task comments
        /// </summary>
        /// <returns>List of all task comments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskComment>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.TaskComment.OrderByDescending(tc => tc.CreatedAt).ToListAsync());
        }

        /// <summary>
        /// Get a specific task comment by ID
        /// </summary>
        /// <param name="id">Task Comment ID</param>
        /// <returns>Task comment details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskComment), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var comment = await _context.TaskComment.FindAsync(id);
            if (comment == null) return NotFound();
            return Ok(comment);
        }

        /// <summary>
        /// Create a new task comment
        /// </summary>
        /// <param name="comment">Task comment data</param>
        /// <returns>Created task comment</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskComment), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(TaskComment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            _context.TaskComment.Add(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = comment.TaskCommentId }, comment);
        }

        /// <summary>
        /// Update an existing task comment
        /// </summary>
        /// <param name="id">Task Comment ID</param>
        /// <param name="comment">Updated task comment data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, TaskComment comment)
        {
            if (id != comment.TaskCommentId) return BadRequest();

            comment.IsEdited = true;
            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TaskCommentExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a task comment
        /// </summary>
        /// <param name="id">Task Comment ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.TaskComment.FindAsync(id);
            if (comment == null) return NotFound();
            _context.TaskComment.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get comments for a specific task
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <returns>List of comments for the specified task</returns>
        [HttpGet("by-task/{taskId}")]
        [ProducesResponseType(typeof(IEnumerable<TaskComment>), 200)]
        public async Task<IActionResult> GetByTask(int taskId)
        {
            var comments = await _context.TaskComment
                .Where(tc => tc.TaskId == taskId)
                .OrderBy(tc => tc.CreatedAt)
                .ToListAsync();
            return Ok(comments);
        }

        /// <summary>
        /// Get comments by user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of comments by the specified user</returns>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<TaskComment>), 200)]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var comments = await _context.TaskComment
                .Where(tc => tc.UserId == userId)
                .OrderByDescending(tc => tc.CreatedAt)
                .ToListAsync();
            return Ok(comments);
        }

        /// <summary>
        /// Get recent comments (last 24 hours)
        /// </summary>
        /// <returns>List of recent comments</returns>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(IEnumerable<TaskComment>), 200)]
        public async Task<IActionResult> GetRecent()
        {
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var comments = await _context.TaskComment
                .Where(tc => tc.CreatedAt >= yesterday)
                .OrderByDescending(tc => tc.CreatedAt)
                .ToListAsync();
            return Ok(comments);
        }

        /// <summary>
        /// Get comment count for a task
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <returns>Number of comments for the task</returns>
        [HttpGet("count/{taskId}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetCommentCount(int taskId)
        {
            var count = await _context.TaskComment.CountAsync(tc => tc.TaskId == taskId);
            return Ok(new { TaskId = taskId, CommentCount = count });
        }

        private async Task<bool> TaskCommentExists(int id)
        {
            return await _context.TaskComment.AnyAsync(e => e.TaskCommentId == id);
        }
    }
}

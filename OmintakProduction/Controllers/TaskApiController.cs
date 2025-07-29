using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;
using System.Linq;
using OmintakProduction.Models.TaskModels;

namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Tasks
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TaskApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        /// <returns>List of all tasks</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Models.Task>), 200)]
        public async System.Threading.Tasks.Task<IActionResult> GetAll()
        {
            return Ok(await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync());
        }

        /// <summary>
        /// Get a specific task by ID
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Task details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Models.Task), 200)]
        [ProducesResponseType(404)]
        public async System.Threading.Tasks.Task<IActionResult> Get(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        /// <summary>
        /// Put a task on hold
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="request">Hold request details</param>
        /// <returns>Updated task details</returns>
        [HttpPost("{id}/hold")]
        [Authorize(Roles = "TeamLead")]
        [ProducesResponseType(typeof(Models.Task), 200)]
        [ProducesResponseType(404)]
        public async System.Threading.Tasks.Task<IActionResult> PutTaskOnHold(int id, [FromBody] HoldTaskRequest request)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound("Task not found");

            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            task.PutOnHold(request.Reason, userId);
            
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        /// <summary>
        /// Resume a task that was on hold
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Updated task details</returns>
        [HttpPost("{id}/resume")]
        [Authorize(Roles = "TeamLead")]
        [ProducesResponseType(typeof(Models.Task), 200)]
        [ProducesResponseType(404)]
        public async System.Threading.Tasks.Task<IActionResult> ResumeTask(int id)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound("Task not found");

            if (!task.IsOnHold)
                return BadRequest("Task is not on hold");

            task.ResumeTask();
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        /// <summary>
        /// Get all tasks that are currently on hold
        /// </summary>
        /// <returns>List of on-hold tasks</returns>
        [HttpGet("onhold")]
        [Authorize(Roles = "TeamLead")]
        [ProducesResponseType(typeof(IEnumerable<Models.Task>), 200)]
        public async System.Threading.Tasks.Task<IActionResult> GetOnHoldTasks()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            
            var managedTeamIds = await _context.Team
                .Where(t => t.TeamLeadId == userId)
                .Select(t => t.TeamId)
                .ToListAsync();

            var onHoldTasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.HoldRequestedByUser)
                .Where(t => t.IsOnHold && 
                           t.Project != null && 
                           t.Project.TeamId.HasValue &&
                           managedTeamIds.Contains(t.Project.TeamId.Value))
                .OrderByDescending(t => t.HoldStartDate)
                .ToListAsync();

            return Ok(onHoldTasks);
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        /// <param name="title">Task title</param>
        /// <param name="description">Task description</param>
        /// <param name="projectName">Project name</param>
        /// <param name="roleNames">List of role names to assign</param>
        /// <param name="priority">Task priority</param>
        /// <param name="dueDate">Due date</param>
        [HttpPost]
        [ProducesResponseType(typeof(Models.Task), 201)]
        [ProducesResponseType(400)]
        public async System.Threading.Tasks.Task<IActionResult> Create(string title, string description, string projectName, List<string> roleNames, Models.TaskPriority priority, DateTime dueDate)
        {
            var project = await _context.Project.FirstOrDefaultAsync(p => p.ProjectName == projectName);
            var users = await (from u in _context.User
                               join r in _context.Role on u.RoleId equals r.RoleId
                               where roleNames.Any() && r.RoleName != null && roleNames.Contains(r.RoleName)
                               select u).ToListAsync();
            var task = new Models.Task
            {
                Title = title,
                Description = description,
                ProjectId = project != null ? project.ProjectId : 0,
                Priority = priority,
                DueDate = dueDate,
                CreatedAt = DateTime.Now,
                Status = Models.TaskStatus.Todo,
                CreatedByUserId = 0, // Set appropriately
                AssignedUsers = users
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            await CreateHistoryEntry(task.Id, task.CreatedByUserId, TaskHistoryAction.Created, null, null, "Task created");
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="task">Updated task data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async System.Threading.Tasks.Task<IActionResult> Update(int id, Models.Task task)
        {
            if (id != task.Id) return BadRequest();
            
            var existingTask = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (existingTask == null) return NotFound();

            task.UpdatedAt = DateTime.Now;
            _context.Entry(task).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
                await CreateHistoryEntry(task.Id, task.CreatedByUserId, TaskHistoryAction.Updated, null, null, "Task updated");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TaskExists(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async System.Threading.Tasks.Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            
            task.IsDeleted = true;
            await _context.SaveChangesAsync();
            await CreateHistoryEntry(task.Id, task.CreatedByUserId, TaskHistoryAction.Deleted, null, null, "Task deleted");
            
            return NoContent();
        }

        /// <summary>
        /// Get tasks by project
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>List of tasks for the specified project</returns>
        [HttpGet("by-project/{projectId}")]
        [ProducesResponseType(typeof(IEnumerable<Models.Task>), 200)]
        public async System.Threading.Tasks.Task<IActionResult> GetByProject(int projectId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return Ok(tasks);
        }

        /// <summary>
        /// Get tasks by assigned user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of tasks assigned to the specified user</returns>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Models.Task>), 200)]
        public async System.Threading.Tasks.Task<IActionResult> GetByUser(int userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.AssignedUsers.Any(u => u.UserId == userId) && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return Ok(tasks);
        }

        /// <summary>
        /// Get tasks by status
        /// </summary>
        /// <param name="status">Task status</param>
        /// <returns>List of tasks with the specified status</returns>
        [HttpGet("by-status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<Models.Task>), 200)]
        public async System.Threading.Tasks.Task<IActionResult> GetByStatus(Models.TaskStatus status)
        {
            var tasks = await _context.Tasks
                .Where(t => t.Status == status && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return Ok(tasks);
        }

        /// <summary>
        /// Get tasks by priority
        /// </summary>
        /// <param name="priority">Task priority</param>
        /// <returns>List of tasks with the specified priority</returns>
        [HttpGet("by-priority/{priority}")]
        [ProducesResponseType(typeof(IEnumerable<Models.Task>), 200)]
        public async System.Threading.Tasks.Task<IActionResult> GetByPriority(Models.TaskPriority priority)
        {
            var tasks = await _context.Tasks
                .Where(t => t.Priority == priority && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return Ok(tasks);
        }

        /// <summary>
        /// Update task status
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="status">New status</param>
        /// <returns>No content if successful</returns>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async System.Threading.Tasks.Task<IActionResult> UpdateStatus(int id, [FromBody] Models.TaskStatus status)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            var oldStatus = task.Status;
            task.Status = status;
            task.UpdatedAt = DateTime.Now;
            
            if (status == Models.TaskStatus.Completed)
                task.CompletedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            await CreateHistoryEntry(task.Id, task.CreatedByUserId, TaskHistoryAction.StatusChanged, 
                oldStatus.ToString(), status.ToString(), $"Status changed from {oldStatus} to {status}");
            
            return NoContent();
        }

        /// <summary>
        /// Assign task to user
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="userId">User ID to assign</param>
        /// <returns>No content if successful</returns>
        [HttpPatch("{id}/assign")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async System.Threading.Tasks.Task<IActionResult> AssignTask(int id, [FromBody] int userId)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            var oldUserIds = task.AssignedUsers.Select(u => u.UserId).ToList();
            var user = await _context.User.FindAsync(userId);
            if (user != null && !task.AssignedUsers.Any(u => u.UserId == userId))
            {
                task.AssignedUsers.Add(user);
            }
            task.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            await CreateHistoryEntry(task.Id, task.CreatedByUserId, TaskHistoryAction.AssigneeChanged, 
                string.Join(",", oldUserIds), userId.ToString(), $"Task assigned to user {userId}");
            
            return NoContent();
        }

        private async System.Threading.Tasks.Task<bool> TaskExists(int id)
        {
            return await _context.Tasks.AnyAsync(e => e.Id == id);
        }

        private async System.Threading.Tasks.Task CreateHistoryEntry(int taskId, int userId, TaskHistoryAction action, string? oldValue, string? newValue, string description)
        {
            var history = new TaskHistory
            {
                TaskId = taskId,
                UserId = userId,
                Action = action,
                OldValue = oldValue,
                NewValue = newValue,
                Description = description,
                CreatedAt = DateTime.Now
            };
            
            _context.TaskHistory.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}

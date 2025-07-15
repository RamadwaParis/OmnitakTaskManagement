using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Task Assignments
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TaskAssignmentApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskAssignmentApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all task assignments
        /// </summary>
        /// <returns>List of all task assignments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskAssignment>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.TaskAssignment.ToListAsync());
        }

        /// <summary>
        /// Get a specific task assignment by ID
        /// </summary>
        /// <param name="id">Task Assignment ID</param>
        /// <returns>Task assignment details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskAssignment), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var assignment = await _context.TaskAssignment.FindAsync(id);
            if (assignment == null) return NotFound();
            return Ok(assignment);
        }

        /// <summary>
        /// Create a new task assignment
        /// </summary>
        /// <param name="assignment">Task assignment data</param>
        /// <returns>Created task assignment</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskAssignment), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(TaskAssignment assignment)
        {
            _context.TaskAssignment.Add(assignment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = assignment.Id }, assignment);
        }

        /// <summary>
        /// Update an existing task assignment
        /// </summary>
        /// <param name="id">Task Assignment ID</param>
        /// <param name="assignment">Updated task assignment data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, TaskAssignment assignment)
        {
            if (id != assignment.Id) return BadRequest();
            _context.Entry(assignment).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TaskAssignmentExists(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        /// <summary>
        /// Delete a task assignment
        /// </summary>
        /// <param name="id">Task Assignment ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var assignment = await _context.TaskAssignment.FindAsync(id);
            if (assignment == null) return NotFound();
            _context.TaskAssignment.Remove(assignment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get task assignments by project
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>List of task assignments for the specified project</returns>
        [HttpGet("by-project/{projectId}")]
        [ProducesResponseType(typeof(IEnumerable<TaskAssignment>), 200)]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            var assignments = await _context.TaskAssignment
                .Where(ta => ta.ProjectId == projectId)
                .ToListAsync();
            return Ok(assignments);
        }

        /// <summary>
        /// Get task assignments by user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of task assignments for the specified user</returns>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<TaskAssignment>), 200)]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var assignments = await _context.TaskAssignment
                .Where(ta => ta.UserId == userId)
                .ToListAsync();
            return Ok(assignments);
        }

        /// <summary>
        /// Get task assignments by status
        /// </summary>
        /// <param name="status">Assignment status</param>
        /// <returns>List of task assignments with the specified status</returns>
        [HttpGet("by-status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<TaskAssignment>), 200)]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var assignments = await _context.TaskAssignment
                .Where(ta => ta.Status == status)
                .ToListAsync();
            return Ok(assignments);
        }

        /// <summary>
        /// Get overdue task assignments
        /// </summary>
        /// <returns>List of overdue task assignments</returns>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(IEnumerable<TaskAssignment>), 200)]
        public async Task<IActionResult> GetOverdue()
        {
            var assignments = await _context.TaskAssignment
                .Where(ta => ta.DueDate < DateTime.Now && ta.Status != "Completed")
                .ToListAsync();
            return Ok(assignments);
        }

        private async Task<bool> TaskAssignmentExists(int id)
        {
            return await _context.TaskAssignment.AnyAsync(e => e.Id == id);
        }
    }
}

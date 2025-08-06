using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Tickets
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TicketApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all tickets
        /// </summary>
        /// <returns>List of all tickets</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Ticket>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Ticket.Where(t => !t.IsDeleted).ToListAsync());
        }

        /// <summary>
        /// Get a specific ticket by ID
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>Ticket details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Ticket), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var ticket = await _context.Ticket.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        /// <summary>
        /// Create a new ticket
        /// </summary>
        /// <param name="ticket">Ticket data</param>
        /// <returns>Created ticket</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Ticket), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            _context.Ticket.Add(ticket);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = ticket.Id }, ticket);
        }

        /// <summary>
        /// Update an existing ticket
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <param name="ticket">Updated ticket data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, Ticket ticket)
        {
            if (id != ticket.Id) return BadRequest();
            _context.Entry(ticket).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TicketExists(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        /// <summary>
        /// Delete a ticket
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null || ticket.IsDeleted) return NotFound();
            
            // Implement soft delete
            ticket.IsDeleted = true;
            ticket.DeletedAt = DateTime.UtcNow;
            
            _context.Update(ticket);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get tickets by project
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>List of tickets for the specified project</returns>
        [HttpGet("by-project/{projectId}")]
        [ProducesResponseType(typeof(IEnumerable<Ticket>), 200)]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            var tickets = await _context.Ticket
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync();
            return Ok(tickets);
        }

        /// <summary>
        /// Get tickets by status
        /// </summary>
        /// <param name="status">Ticket status</param>
        /// <returns>List of tickets with the specified status</returns>
        [HttpGet("by-status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<Ticket>), 200)]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var tickets = await _context.Ticket
                .Where(t => t.Status == status && !t.IsDeleted)
                .ToListAsync();
            return Ok(tickets);
        }

        /// <summary>
        /// Get tickets assigned to a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of tickets assigned to the specified user</returns>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Ticket>), 200)]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var tickets = await _context.Ticket
                .Where(t => t.AssignedToUserId == userId && !t.IsDeleted)
                .ToListAsync();
            return Ok(tickets);
        }

        /// <summary>
        /// Assign a ticket to a user (only one user allowed)
        /// </summary>
        [HttpPatch("{id}/assign")]
        public async Task<IActionResult> AssignTicket(int id, int userId)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null) return NotFound();
            ticket.AssignedToUserId = userId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Link a ticket to a task
        /// </summary>
        [HttpPatch("{id}/link-task")]
        public async Task<IActionResult> LinkTicketToTask(int id, int taskId)
        {
            var ticket = await _context.Ticket.Include(t => t.Tasks).FirstOrDefaultAsync(t => t.Id == id);
            var task = await _context.Tasks.FindAsync(taskId);
            if (ticket == null || task == null) return NotFound();
            if (task.TicketId != id)
            {
                task.TicketId = id;
                ticket.Tasks.Add(task);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        private async Task<bool> TicketExists(int id)
        {
            return await _context.Ticket.AnyAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}

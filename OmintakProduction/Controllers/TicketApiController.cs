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
            return Ok(await _context.Ticket.ToListAsync());
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
            var ticket = await _context.Ticket.FirstOrDefaultAsync(t => t.TicketId == id);
           
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
            return CreatedAtAction(nameof(Get), new { id = ticket.TicketId }, ticket);
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
            if (id != ticket.TicketId) return BadRequest();
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
            if (ticket == null) return NotFound();
            _context.Ticket.Remove(ticket);
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
                .Where(t => t.ProjectId == projectId)
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
                .Where(t => t.Status == status)
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
                .Where(t => t.AssignedToUserId == userId)
                .ToListAsync();
            return Ok(tickets);
        }
 
        private async Task<bool> TicketExists(int id)
        {
            return await _context.Ticket.AnyAsync(e => e.TicketId == id);
        }
    }
}

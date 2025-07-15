using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Roles
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoleApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>List of all roles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Role>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Role.ToListAsync());
        }

        /// <summary>
        /// Get a specific role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Role details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Role), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="role">Role data</param>
        /// <returns>Created role</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Role), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(Role role)
        {
            _context.Role.Add(role);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = role.RoleId }, role);
        }

        /// <summary>
        /// Update an existing role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="role">Updated role data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, Role role)
        {
            if (id != role.RoleId) return BadRequest();
            _context.Entry(role).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RoleExists(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null) return NotFound();
            _context.Role.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> RoleExists(int id)
        {
            return await _context.Role.AnyAsync(e => e.RoleId == id);
        }
    }
}

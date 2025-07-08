using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;

namespace OmintakProduction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Role.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null) return NotFound();
            return role;
        }

        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            _context.Role.Add(role);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRole), new { id = role.RoleId }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            if (id != role.RoleId) return BadRequest();
            _context.Entry(role).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Role.Any(e => e.RoleId == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null) return NotFound();
            _context.Role.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}


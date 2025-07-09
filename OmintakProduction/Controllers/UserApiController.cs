using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;
 
namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Users
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserApiController : ControllerBase
    {
        private readonly AppDbContext _context;
 
        public UserApiController(AppDbContext context)
        {
            _context = context;
        }
 
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.User.ToListAsync());
        }
 
        /// <summary>
        /// Get a specific user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();
            return Ok(user);
        }
 
        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
        }
 
        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="user">Updated user data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.UserId) return BadRequest();
            _context.Entry(user).State = EntityState.Modified;
           
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id))
                    return NotFound();
                throw;
            }
           
            return NoContent();
        }
 
        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
 
        /// <summary>
        /// Get users by role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>List of users with the specified role</returns>
        [HttpGet("by-role/{roleId}")]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        public async Task<IActionResult> GetByRole(int roleId)
        {
            var users = await _context.User
                .Where(u => u.RoleId == roleId)
                .ToListAsync();
            return Ok(users);
        }
 
        /// <summary>
        /// Get user with role information
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User with role details</returns>
        [HttpGet("{id}/with-role")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetWithRole(int id)
        {
            var userWithRole = await (from u in _context.User
                                    join r in _context.Role on u.RoleId equals r.RoleId
                                    where u.UserId == id
                                    select new
                                    {
                                        u.UserId,
                                        u.UserName,
                                        u.Email,
                                        u.isActive,
                                        u.NeedsWelcome,
                                        u.CreatedDate,
                                        Role = new
                                        {
                                            r.RoleId,
                                            r.RoleName
                                        }
                                    }).FirstOrDefaultAsync();
 
            if (userWithRole == null) return NotFound();
            return Ok(userWithRole);
        }
 
        private async Task<bool> UserExists(int id)
        {
            return await _context.User.AnyAsync(e => e.UserId == id);
        }
    }
}
 

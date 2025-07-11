using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;
 
namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Projects
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProjectApiController(AppDbContext context)
        {
            _context = context;
        }
 
        /// <summary>
        /// Get all projects
        /// </summary>
        /// <returns>List of all projects</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Project>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Project.ToListAsync());
        }
 
        /// <summary>
        /// Get a specific project by ID
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns>Project details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Project), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }
 
        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="project">Project data</param>
        /// <returns>Created project</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Project), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(Project project)
        {
            _context.Project.Add(project);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = project.ProjectId }, project);
        }
 
        /// <summary>
        /// Update an existing project
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <param name="project">Updated project data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, Project project)
        {
            if (id != project.ProjectId) return BadRequest();
            _context.Entry(project).State = EntityState.Modified;
           
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProjectExists(id))
                    return NotFound();
                throw;
            }
           
            return NoContent();
        }
 
        /// <summary>
        /// Delete a project
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project == null) return NotFound();
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }
 
        private async Task<bool> ProjectExists(int id)
        {
            return await _context.Project.AnyAsync(e => e.ProjectId == id);
        }
    }
}
 
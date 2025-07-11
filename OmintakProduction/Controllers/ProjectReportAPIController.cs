using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;
 
namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Project Reports
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectReportApiController : ControllerBase
    {
        private readonly AppDbContext _context;
 
        public ProjectReportApiController(AppDbContext context)
        {
            _context = context;
        }
 
        /// <summary>
        /// Get all project reports
        /// </summary>
        /// <returns>List of all project reports</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectReport>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.ProjectReport.ToListAsync());
        }
 
        /// <summary>
        /// Get a specific project report by ID
        /// </summary>
        /// <param name="id">Project Report ID</param>
        /// <returns>Project report details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectReport), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var report = await _context.ProjectReport.FindAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }
 
        /// <summary>
        /// Create a new project report
        /// </summary>
        /// <param name="report">Project report data</param>
        /// <returns>Created project report</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectReport), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(ProjectReport report)
        {
            _context.ProjectReport.Add(report);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = report.ProjectReportId }, report);
        }
 
        /// <summary>
        /// Update an existing project report
        /// </summary>
        /// <param name="id">Project Report ID</param>
        /// <param name="report">Updated project report data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, ProjectReport report)
        {
            if (id != report.ProjectReportId) return BadRequest();
            _context.Entry(report).State = EntityState.Modified;
           
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProjectReportExists(id))
                    return NotFound();
                throw;
            }
           
            return NoContent();
        }
 
        /// <summary>
        /// Delete a project report
        /// </summary>
        /// <param name="id">Project Report ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.ProjectReport.FindAsync(id);
            if (report == null) return NotFound();
            _context.ProjectReport.Remove(report);
            await _context.SaveChangesAsync();
            return NoContent();
        }
 
        /// <summary>
        /// Get project reports by project ID
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>List of project reports for the specified project</returns>
        [HttpGet("by-project/{projectId}")]
        [ProducesResponseType(typeof(IEnumerable<ProjectReport>), 200)]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            var reports = await _context.ProjectReport
                .Where(r => r.ProjectId == projectId)
                .ToListAsync();
            return Ok(reports);
        }
 
        private async Task<bool> ProjectReportExists(int id)
        {
            return await _context.ProjectReport.AnyAsync(e => e.ProjectReportId == id);
        }
    }
}
 
 

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;

namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Bug Reports
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BugReportApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BugReportApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all bug reports
        /// </summary>
        /// <returns>List of all bug reports</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BugReport>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var bugReports = await _context.BugReports
                .Include(b => b.Project)
                .Include(b => b.Task)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedToUser)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            
            return Ok(bugReports);
        }

        /// <summary>
        /// Get a specific bug report by ID
        /// </summary>
        /// <param name="id">Bug report ID</param>
        /// <returns>Bug report details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BugReport), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var bugReport = await _context.BugReports
                .Include(b => b.Project)
                .Include(b => b.Task)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedToUser)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            
            if (bugReport == null) return NotFound();
            return Ok(bugReport);
        }

        /// <summary>
        /// Create a new bug report
        /// </summary>
        /// <param name="bugReport">Bug report data</param>
        /// <returns>Created bug report</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BugReport), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(BugReport bugReport)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bugReport.CreatedAt = DateTime.Now;
            _context.BugReports.Add(bugReport);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new { id = bugReport.Id }, bugReport);
        }

        /// <summary>
        /// Update an existing bug report
        /// </summary>
        /// <param name="id">Bug report ID</param>
        /// <param name="bugReport">Updated bug report data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, BugReport bugReport)
        {
            if (id != bugReport.Id) return BadRequest();
            
            var existingBugReport = await _context.BugReports.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            if (existingBugReport == null) return NotFound();

            bugReport.UpdatedAt = DateTime.Now;
            _context.Entry(bugReport).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BugReportExists(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        /// <summary>
        /// Delete a bug report
        /// </summary>
        /// <param name="id">Bug report ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport == null) return NotFound();
            
            bugReport.IsDeleted = true;
            bugReport.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        /// <summary>
        /// Get bug reports by project
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>List of bug reports for the specified project</returns>
        [HttpGet("by-project/{projectId}")]
        [ProducesResponseType(typeof(IEnumerable<BugReport>), 200)]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            var bugReports = await _context.BugReports
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedToUser)
                .Where(b => b.ProjectId == projectId && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            
            return Ok(bugReports);
        }

        /// <summary>
        /// Get bug reports by status
        /// </summary>
        /// <param name="status">Bug status</param>
        /// <returns>List of bug reports with the specified status</returns>
        [HttpGet("by-status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<BugReport>), 200)]
        public async Task<IActionResult> GetByStatus(BugStatus status)
        {
            var bugReports = await _context.BugReports
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedToUser)
                .Where(b => b.Status == status && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            
            return Ok(bugReports);
        }

        /// <summary>
        /// Get bug reports by severity
        /// </summary>
        /// <param name="severity">Bug severity</param>
        /// <returns>List of bug reports with the specified severity</returns>
        [HttpGet("by-severity/{severity}")]
        [ProducesResponseType(typeof(IEnumerable<BugReport>), 200)]
        public async Task<IActionResult> GetBySeverity(BugSeverity severity)
        {
            var bugReports = await _context.BugReports
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedToUser)
                .Where(b => b.Severity == severity && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            
            return Ok(bugReports);
        }

        /// <summary>
        /// Update bug report status
        /// </summary>
        /// <param name="id">Bug report ID</param>
        /// <param name="status">New status</param>
        /// <returns>No content if successful</returns>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] BugStatus status)
        {
            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport == null) return NotFound();

            bugReport.Status = status;
            bugReport.UpdatedAt = DateTime.Now;
            
            if (status == BugStatus.Resolved || status == BugStatus.Closed)
                bugReport.ResolvedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        /// <summary>
        /// Assign bug report to user
        /// </summary>
        /// <param name="id">Bug report ID</param>
        /// <param name="userId">User ID to assign</param>
        /// <returns>No content if successful</returns>
        [HttpPatch("{id}/assign")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AssignBugReport(int id, [FromBody] int userId)
        {
            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport == null) return NotFound();

            bugReport.AssignedToUserId = userId;
            bugReport.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        private async Task<bool> BugReportExists(int id)
        {
            return await _context.BugReports.AnyAsync(e => e.Id == id);
        }
    }
}

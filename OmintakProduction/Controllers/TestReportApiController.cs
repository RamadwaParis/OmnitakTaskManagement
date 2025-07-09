using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;
 
namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Test Reports
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TestReportApiController : ControllerBase
    {
        private readonly AppDbContext _context;
 
        public TestReportApiController(AppDbContext context)
        {
            _context = context;
        }
 
        /// <summary>
        /// Get all test reports
        /// </summary>
        /// <returns>List of all test reports</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TestReport>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.TestReport.ToListAsync());
        }
 
        /// <summary>
        /// Get a specific test report by ID
        /// </summary>
        /// <param name="id">Test Report ID</param>
        /// <returns>Test report details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TestReport), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var report = await _context.TestReport.FindAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }
 
        /// <summary>
        /// Create a new test report
        /// </summary>
        /// <param name="report">Test report data</param>
        /// <returns>Created test report</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TestReport), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(TestReport report)
        {
            _context.TestReport.Add(report);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = report.Id }, report);
        }
 
        /// <summary>
        /// Update an existing test report
        /// </summary>
        /// <param name="id">Test Report ID</param>
        /// <param name="report">Updated test report data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, TestReport report)
        {
            if (id != report.Id) return BadRequest();
            _context.Entry(report).State = EntityState.Modified;
           
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TestReportExists(id))
                    return NotFound();
                throw;
            }
           
            return NoContent();
        }
 
        /// <summary>
        /// Delete a test report
        /// </summary>
        /// <param name="id">Test Report ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.TestReport.FindAsync(id);
            if (report == null) return NotFound();
            _context.TestReport.Remove(report);
            await _context.SaveChangesAsync();
            return NoContent();
        }
 
        /// <summary>
        /// Get test reports by project ID
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>List of test reports for the specified project</returns>
        [HttpGet("by-project/{projectId}")]
        [ProducesResponseType(typeof(IEnumerable<TestReport>), 200)]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            var reports = await _context.TestReport
                .Where(r => r.ProjectId == projectId)
                .ToListAsync();
            return Ok(reports);
        }
 
        private async Task<bool> TestReportExists(int id)
        {
            return await _context.TestReport.AnyAsync(e => e.Id == id);
        }
    }
}
 

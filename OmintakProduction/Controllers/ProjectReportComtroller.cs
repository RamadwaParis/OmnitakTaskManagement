using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;
 
namespace OmintakProduction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectReportController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProjectReportController(AppDbContext context)
        {
            _context = context;
        }
 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.ProjectReport.ToListAsync());
        }
 
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var report = await _context.ProjectReport.FindAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }
 
        [HttpPost]
        public async Task<IActionResult> Create(ProjectReport report)
        {
            _context.ProjectReport.Add(report);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = report.Id }, report);
        }
 
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProjectReport report)
        {
            if (id != report.Id) return BadRequest();
            _context.Entry(report).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.ProjectReport.FindAsync(id);
            if (report == null) return NotFound();
            _context.ProjectReport.Remove(report);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

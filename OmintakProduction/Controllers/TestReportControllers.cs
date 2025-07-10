using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestReportController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TestReportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.TestReports.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var report = await _context.TestReports.FindAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TestReport report)
        {
            _context.TestReports.Add(report);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = report.TestReportId }, report);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TestReport report)
        {
            if (id != report.TestReportId) return BadRequest();
            _context.Entry(report).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.TestReports.FindAsync(id);
            if (report == null) return NotFound();
            _context.TestReports.Remove(report);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectAssignmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectAssignmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("assign")]
        [Authorize(Roles = "TeamLead")]
        public async Task<IActionResult> AssignProjectToTeam([FromBody] ProjectAssignmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Verify the team is managed by the current user
            var team = await _context.Team
                .Include(t => t.TeamLead)
                .FirstOrDefaultAsync(t => t.TeamId == request.TeamId && t.TeamLeadId == userId);

            if (team == null)
                return NotFound("Team not found or you don't have permission to manage it");

            // Get the project
            var project = await _context.Project
                .FirstOrDefaultAsync(p => p.ProjectId == request.ProjectId);

            if (project == null)
                return NotFound("Project not found");

            if (project.TeamId != null)
                return BadRequest("Project is already assigned to a team");

            // Assign the project to the team
            project.TeamId = team.TeamId;
            project.Team = team;

            // Add any notes or assignment details if needed
            if (!string.IsNullOrEmpty(request.Notes))
            {
                // You could store the notes in a ProjectAssignment history table
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Project assigned successfully", project });
        }

        [HttpGet("available-projects")]
        [Authorize(Roles = "TeamLead")]
        public async Task<IActionResult> GetAvailableProjects()
        {
            var projects = await _context.Project
                .Where(p => p.TeamId == null)
                .Select(p => new { p.ProjectId, Name = p.ProjectName, p.Description })
                .ToListAsync();

            return Ok(projects);
        }

        [HttpGet("managed-teams")]
        [Authorize(Roles = "TeamLead")]
        public async Task<IActionResult> GetManagedTeams()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var teams = await _context.Team
                .Where(t => t.TeamLeadId == userId)
                .Select(t => new { t.TeamId, t.TeamName })
                .ToListAsync();

            return Ok(teams);
        }
    }
}

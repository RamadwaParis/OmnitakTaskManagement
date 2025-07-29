using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Security.Claims;


namespace OmintakProduction.Controllers
{
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;
        public ProjectController(AppDbContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            var Project = await _context.Project.ToListAsync();

            return View("~/Views/Project/Index.cshtml", Project);
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Teams = _context.Team.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectName,Description,DueDate,Status,TeamId")] Project project)
        {
            ViewBag.Teams = _context.Team.ToList();
            if (ModelState.IsValid)
            {

                if (project.TeamId == null || !_context.Team.Any(t => t.TeamId == project.TeamId))
                {
                    ModelState.AddModelError("TeamId", "A valid team must be assigned.");
                    return View(project);
                }
                project.Team = await _context.Team.FindAsync(project.TeamId);
                _context.Add(project);
                await _context.SaveChangesAsync();


                // Notify all active team members about new project
                if (project.Team != null && project.Team.TeamMembers != null && project.Team.TeamMembers.Any())
                {
                    foreach (var member in project.Team.TeamMembers.Where(u => u != null && u.isActive))
                    {
                        if (member == null) continue;
                        var notification = new Notification {
                            UserId = member.UserId,
                            Title = "New Project Assigned",
                            Message = $"Your team <b>{project.Team.TeamName}</b> has been assigned a new project: <b>{project.ProjectName}</b>.",
                            Type = NotificationType.Project,
                            IsRead = false,
                            CreatedAt = DateTime.Now,
                            RelatedEntityId = project.ProjectId,
                            RelatedEntityType = "Project"
                        };
                        _context.Notification.Add(notification);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Teams = _context.Team.ToList();
            return View(project);
        }



        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Project.FindAsync(id);

            if (project == null) return NotFound();

            return View("~/Views/Project/Update.cshtml", project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Project project, string? NewDueDate)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            // Get existing project to preserve data
            var existingProject = await _context.Project
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (existingProject == null)
            {
                return NotFound();
            }

            // Handle the DueDate
            if (!string.IsNullOrEmpty(NewDueDate) && DateOnly.TryParse(NewDueDate, out DateOnly newDate))
            {
                project.DueDate = newDate;
            }
            else
            {
                // Keep the original due date if no new date was selected
                project.DueDate = existingProject.DueDate;
            }

            // Preserve other properties that shouldn't change
            project.TeamId = existingProject.TeamId;
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Project.AnyAsync(p => p.ProjectId == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return View(project);
        }




        public IActionResult Delete()
        {

            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Project
                .FirstOrDefaultAsync(t => t.ProjectId == id);

            return project == null ? NotFound() : View("~/Views/Project/Delete.cshtml", project);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Project obj)
        {
            _context.Project.Remove(obj);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }


    }
}

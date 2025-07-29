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
                project.DueDate = DateOnly.FromDateTime(DateTime.Now);
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
        public async Task<IActionResult> Update(int id, [Bind("ProjectId,ProjectName,Description,DueDate,Status")] Project updatedProject)
        {
            // Make sure the ID from the route matches the form
            if (id != updatedProject.ProjectId) return NotFound();

            // Load the existing ticket from DB (includes Created_By)
            var existingProject = await _context.Project.FindAsync(id);
            if (existingProject == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Only update fields that can be changed
                    existingProject.ProjectName = updatedProject.ProjectName;
                    existingProject.Description = updatedProject.Description;
                    existingProject.Status = updatedProject.Status;

                    // Save changes without altering Created_By or Created_At
                    _context.Update(existingProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(updatedProject.ProjectId)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Ticket/Update.cshtml", updatedProject);
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using OmintakProduction.Models;
using OmintakProduction.Data;
using OmintakProduction.Services;
using System.Linq;

namespace OmintakProduction.Controllers
{
    public class TeamController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        
        public TeamController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }
        

        // List all teams
        public IActionResult Index()
        {
            var teams = _context.Team.ToList();
            return View(teams);
        }

        // Create Team (MVC)

        [HttpGet]
        public IActionResult Create()
        {
            // Provide a list of active users for selection
            ViewBag.ActiveUsers = _context.User.Where(u => u.isActive).ToList();
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Create(Team team, int[] SelectedUserIds)
        {
            // Enforce max 5 members
            if (SelectedUserIds == null || SelectedUserIds.Length == 0)
            {
                ModelState.AddModelError("TeamMembers", "Please select at least one active user.");
            }
            else if (SelectedUserIds.Length > 5)
            {
                ModelState.AddModelError("TeamMembers", "A team can have a maximum of 5 members.");
            }
            if (ModelState.IsValid)
            {
                // Assign selected users as team members
                var userIds = SelectedUserIds ?? new int[0];
                var users = _context.User.Where(u => userIds.Contains(u.UserId) && u.isActive).ToList();
                team.TeamMembers = users;
                _context.Team.Add(team);
                _context.SaveChanges();
                
                // Notify all team members about team assignment
                foreach (var user in users)
                {
                    await _notificationService.NotifyUserAssignedToTeam(user.UserId, team.TeamId, team.TeamName);
                }
                
                return RedirectToAction("Index");
            }
            ViewBag.ActiveUsers = _context.User.Where(u => u.isActive).ToList();
            return View(team);
        }

        // Update Team (MVC)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var team = _context.Team.Find(id);
            if (team == null) return NotFound();
            return View(team);
        }

        [HttpPost]
        public IActionResult Edit(Team team)
        {
            if (ModelState.IsValid)
            {
                _context.Team.Update(team);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }

        // Delete Team (MVC)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var team = _context.Team.Find(id);
            if (team == null) return NotFound();

            // Handle orphaned projects
            var projects = _context.Project.Where(p => p.TeamId == team.TeamId).ToList();
            foreach (var project in projects)
            {
                // Option 1: Delete projects (if you want to cascade)
                // _context.Project.Remove(project);
                // Option 2: Unassign team (if you want to keep projects)
                project.TeamId = null;
                project.Team = null;
            }

            // Handle orphaned users
            var users = _context.User.Where(u => u.TeamId == team.TeamId).ToList();
            foreach (var user in users)
            {
                user.TeamId = null;
            }

            // Handle orphaned tasks
            var tasks = _context.Tasks.Where(t => t.Project != null && t.Project.TeamId == team.TeamId).ToList();
            foreach (var task in tasks)
            {
                // Option: Unassign project or mark as orphaned
                task.Project = null;
                task.ProjectId = null;
            }

            _context.Team.Remove(team);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Assign a project to a team
        [HttpPost]
        public IActionResult AssignProject(int teamId, int projectId)
        {
            var team = _context.Team.Find(teamId);
            var project = _context.Project.Find(projectId);
            if (team != null && project != null)
            {
                project.TeamId = teamId;
                project.Team = team;
                _context.SaveChanges();
                return RedirectToAction("Index", "Project");
            }
            return BadRequest();
        }

        // API: Update Team
        [HttpPut]
        [Route("api/team/update/{id}")]
        public IActionResult UpdateApi(int id, [FromBody] Team team)
        {
            var existing = _context.Team.Find(id);
            if (existing == null) return NotFound();
            existing.TeamName = team.TeamName;
            // Add other properties as needed
            _context.SaveChanges();
            return Ok(existing);
        }

        // API: Delete Team
        [HttpDelete]
        [Route("api/team/delete/{id}")]
        public IActionResult DeleteApi(int id)
        {
            var team = _context.Team.Find(id);
            if (team == null) return NotFound();
            _context.Team.Remove(team);
            _context.SaveChanges();
            return Ok();
        }

        // AJAX: Get Active Users for Select2
        [HttpGet]
        public IActionResult GetActiveUsers()
        {
            var users = _context.User
                .Where(u => u.isActive)
                .Select(u => new { userId = u.UserId, userName = u.UserName, email = u.Email })
                .ToList();
            return Json(users);
        }
    }
}
        
    

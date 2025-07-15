using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using TaskStatus = OmintakProduction.Models.TaskStatus;

namespace OmintakProduction.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _context.Team.Include(t => t.TeamMembers).ToListAsync();
            var activeProjects = await _context.Project.CountAsync(p => p.Status == "Active");
            var dashboardData = new DashboardViewModel
            {
                ActiveTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatus.InProgress || t.Status == TaskStatus.Todo),
                CompletedTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatus.Completed),
                OverdueTasks = await _context.Tasks.CountAsync(t => t.DueDate < DateTime.Now && t.Status != TaskStatus.Completed),
                TotalProjects = await _context.Project.CountAsync(),
                ActiveProjects = activeProjects,
                ActiveTickets = await _context.Ticket.CountAsync(),
                ActiveTicketsList = await _context.Ticket.Where(t => t.Status == "Active").OrderByDescending(t => t.CreatedAt).ToListAsync(),
                TeamCount = teams.Count,
                Teams = teams,
                RecentTasks = await _context.Tasks
                    .Include(t => t.AssignedUsers)
                    .Include(t => t.Project)
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                RecentNotifications = await _context.Notification
                    .Include(n => n.User)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                UnreadNotificationCount = await _context.Notification.CountAsync(n => !n.IsRead),
                TasksByStatus = (await _context.Tasks
                    .GroupBy(t => t.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToListAsync())
                    .Cast<object>()
                    .ToList(),
                RecentProjects = await _context.Project
                    .OrderByDescending(p => p.DueDate)
                    .Take(5)
                    .ToListAsync()
            };

            // Get user's name and surname from claims or session
            string welcomeName = string.Empty;
            if (User?.Identity != null && User.Identity.IsAuthenticated)
            {
                var firstName = User.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
                var lastName = User.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value;
                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                {
                    welcomeName = $"{firstName} {lastName}";
                }
                else
                {
                    // fallback to session
                    var sessionName = HttpContext.Session.GetString("WelcomeUserName");
                    if (!string.IsNullOrEmpty(sessionName))
                    {
                        welcomeName = sessionName;
                    }
                }
            }
            if (!string.IsNullOrEmpty(welcomeName))
            {
                TempData["WelcomeMessage"] = $"Welcome, <strong>{welcomeName}</strong>!";
            }
            else
            {
                TempData["WelcomeMessage"] = "Welcome!";
            }

            return View(dashboardData);
        }

        public async Task<IActionResult> UnifiedDashboard()
        {
            var model = new UnifiedDashboardViewModel
            {
                Tasks = await _context.Tasks.Include(t => t.Project).Include(t => t.AssignedUsers).ToListAsync(),
                Tickets = await _context.Ticket.ToListAsync(),
                Users = await _context.User.ToListAsync(),
                UrgentItems = new List<string>(),
                Projects = await _context.Project.ToListAsync()
            };
            return View(model);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using TaskStatus = OmintakProduction.Models.TaskStatus;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OmintakProduction.Services;

namespace OmintakProduction.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PdfGeneratorService _pdfGenerator;

        public DashboardController(AppDbContext context, PdfGeneratorService pdfGenerator)
        {
            _context = context;
            _pdfGenerator = pdfGenerator;
        }

        [HttpGet]
        [Authorize(Roles = "Stakeholder")]
        public async Task<IActionResult> DownloadDashboardReport()
        {
            var projects = await _context.Project
                .Include(p => p.Team)
                .Include(p => p.Tasks)
                .ToListAsync();

            var bugReports = await _context.BugReports
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedToUser)
                .ToListAsync();

            var tasks = await _context.Tasks
                .Include(t => t.AssignedUsers)
                .ToListAsync();

            var pdfBytes = _pdfGenerator.GenerateDashboardReportPdf(projects, bugReports, tasks);
            return File(pdfBytes, "application/pdf", $"DashboardReport_{DateTime.Now:yyyyMMdd}.pdf");
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        private string GetCurrentUserRole()
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            return roleClaim?.Value ?? string.Empty;
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

            // Get user role from claims
            string userRole = string.Empty;
            int currentUserId = GetCurrentUserId();
            if (User?.Identity != null && User.Identity.IsAuthenticated)
            {
                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);
                if (roleClaim != null)
                {
                    userRole = roleClaim.Value;
                }
            }

            // Route to the correct dashboard view based on role
            switch (userRole)
            {
                case "SystemAdmin":
                    var systemAdminData = await GetSystemAdminDashboardData(currentUserId);
                    return View("SystemAdminDashboard", systemAdminData);
                case "TeamLead":
                    var teamLeadData = await GetTeamLeadDashboardData(currentUserId);
                    return View("TeamLeadDashboard", teamLeadData);
                case "Developer":
                    var developerData = await GetDeveloperDashboardData(currentUserId);
                    return View("DeveloperDashboard", developerData);
                case "Tester":
                    var testerData = await GetTesterDashboardData(currentUserId);
                    return View("TesterDashboard", testerData);
                case "Stakeholder":
                    var stakeholderData = await GetStakeholderDashboardData(currentUserId);
                    return View("StakeholderDashboard", stakeholderData);
                default:
                    // For unapproved users or unknown roles, redirect to pending approval page
                    var currentUser = await _context.User.FindAsync(currentUserId);
                    if (currentUser != null && (!currentUser.IsApproved || !currentUser.isActive))
                    {
                        return View("PendingApproval");
                    }
                    var fallbackData = await GetStakeholderDashboardData(currentUserId);
                    return View("StakeholderDashboard", fallbackData); // fallback to view-only
            }
        }

        // COMPREHENSIVE ROLE-SPECIFIC DASHBOARD METHODS
        
        // DEVELOPER DASHBOARD DATA
        private async Task<DeveloperDashboardViewModel> GetDeveloperDashboardData(int userId)
        {
            var assignedTasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                .Where(t => t.AssignedUsers.Any(u => u.UserId == userId))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var recentComments = await _context.TaskComment
                .Where(tc => tc.UserId == userId)
                .OrderByDescending(tc => tc.CreatedAt)
                .Take(10)
                .ToListAsync();

            return new DeveloperDashboardViewModel
            {
                AssignedTasks = assignedTasks,
                MyActiveTasks = assignedTasks.Where(t => t.Status == TaskStatus.InProgress || t.Status == TaskStatus.Todo).ToList(),
                MyCompletedTasks = assignedTasks.Where(t => t.Status == TaskStatus.Completed).ToList(),
                MyOverdueTasks = assignedTasks.Where(t => t.DueDate < DateTime.Now && t.Status != TaskStatus.Completed).ToList(),
                TaskDependencies = GetTaskDependencies(assignedTasks.Select(t => t.Id).ToList()),
                RecentComments = recentComments,
                TasksByPriority = assignedTasks.GroupBy(t => t.Priority).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                RecentNotifications = await _context.Notification
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                UnreadNotificationCount = await _context.Notification
                    .CountAsync(n => n.UserId == userId && !n.IsRead)
            };
        }

        // TESTER DASHBOARD DATA
        private async Task<TesterDashboardViewModel> GetTesterDashboardData(int userId)
        {
            var completedTasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                .Where(t => t.Status == TaskStatus.Completed)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();

            // Check if BugReports table exists, if not use empty list
            var myBugReports = new List<BugReport>();
            try
            {
                myBugReports = await _context.BugReports
                    .Where(br => br.ReportedByUserId == userId)
                    .OrderByDescending(br => br.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                // BugReports table doesn't exist yet, use empty list
            }

            return new TesterDashboardViewModel
            {
                CompletedTasksForTesting = completedTasks,
                PendingTestTasks = completedTasks.Take(10).ToList(),
                MyBugReports = myBugReports,
                ActiveBugReports = myBugReports.Where(br => br.Status != BugStatus.Resolved).ToList(),
                ResolvedBugReports = myBugReports.Where(br => br.Status == BugStatus.Resolved).ToList(),
                BugReportsByPriority = myBugReports.GroupBy(br => br.Severity).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                RecentNotifications = await _context.Notification
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                UnreadNotificationCount = await _context.Notification
                    .CountAsync(n => n.UserId == userId && !n.IsRead)
            };
        }

        // TEAM LEAD DASHBOARD DATA
        private async Task<TeamLeadDashboardViewModel> GetTeamLeadDashboardData(int userId)
        {
            var user = await _context.User.FindAsync(userId);
            var managedProjects = await _context.Project
                .Include(p => p.Team)
                .ThenInclude(t => t != null ? t.TeamMembers : null)
                .Where(p => p.Team != null && p.Team.TeamMembers.Any(tm => tm.UserId == userId))
                .ToListAsync();

            var teamMembers = managedProjects
                .SelectMany(p => p.Team?.TeamMembers ?? new List<User>())
                .Distinct()
                .ToList();

            var allTasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                .Where(t => managedProjects.Select(p => p.ProjectId).Contains(t.ProjectId ?? 0))
                .ToListAsync();

            return new TeamLeadDashboardViewModel
            {
                ManagedProjects = managedProjects,
                TeamMembers = teamMembers,
                AllTasks = allTasks,
                TeamWorkload = GetTeamWorkloadDistribution(teamMembers, allTasks),
                ProjectProgress = GetProjectProgressData(managedProjects, allTasks),
                OverdueTasks = allTasks.Where(t => t.DueDate < DateTime.Now && t.Status != TaskStatus.Completed).ToList(),
                TasksByStatus = allTasks.GroupBy(t => t.Status).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                RecentNotifications = await _context.Notification
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                UnreadNotificationCount = await _context.Notification
                    .CountAsync(n => n.UserId == userId && !n.IsRead)
            };
        }

        // SYSTEM ADMIN DASHBOARD DATA
        private async Task<SystemAdminDashboardViewModel> GetSystemAdminDashboardData(int userId)
        {
            var allUsers = await _context.User
                .Include(u => u.Team)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
            
            var allRoles = await _context.Role.ToListAsync();
            var allProjects = await _context.Project.Include(p => p.Team).ToListAsync();
            var allTasks = await _context.Tasks.Include(t => t.Project).ToListAsync();
            var allTickets = await _context.Ticket.ToListAsync();

            var pendingUsers = allUsers.Where(u => !u.IsApproved).ToList();
            
            // Get recent activity from task history, notifications, and user logins
            var recentTaskHistory = await _context.TaskHistory
                .Include(th => th.User)
                .Include(th => th.Task)
                .OrderByDescending(th => th.CreatedAt)
                .Take(10)
                .ToListAsync();

            var recentNotifications = await _context.Notification
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Calculate real system metrics
            var activeUsers = allUsers.Where(u => u.isActive && u.IsApproved).ToList();
            var totalProjects = allProjects.Count;
            var activeProjects = allProjects.Where(p => p.Status == "Active").Count();
            var completedTasks = allTasks.Where(t => t.Status == Models.TaskStatus.Completed).Count();
            var pendingTasks = allTasks.Where(t => t.Status != Models.TaskStatus.Completed).Count();

            return new SystemAdminDashboardViewModel
            {
                AllUsers = allUsers,
                PendingUsers = pendingUsers,
                AllProjects = allProjects,
                AllTasks = allTasks,
                AllTickets = allTickets,
                SystemLogs = recentTaskHistory,
                UsersByRole = allUsers
                    .Join(allRoles, u => u.RoleId, r => r.RoleId, (u, r) => new { User = u, Role = r })
                    .Where(ur => !string.IsNullOrEmpty(ur.Role.RoleName))
                    .GroupBy(ur => ur.Role.RoleName!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                SystemStatistics = new Dictionary<string, object>
                {
                    ["TotalUsers"] = allUsers.Count,
                    ["ActiveUsers"] = activeUsers.Count,
                    ["PendingUsers"] = pendingUsers.Count,
                    ["TotalProjects"] = totalProjects,
                    ["ActiveProjects"] = activeProjects,
                    ["CompletedTasks"] = completedTasks,
                    ["PendingTasks"] = pendingTasks,
                    ["SystemUptime"] = "99.8%", // This would come from actual monitoring
                    ["DatabaseResponseTime"] = "28ms", // This would come from actual metrics
                    ["DiskUsage"] = "58%", // This would come from actual system monitoring
                    ["MemoryUsage"] = "72%", // This would come from actual system monitoring
                    ["CpuUsage"] = "45%" // This would come from actual system monitoring
                },
                RecentNotifications = recentNotifications,
                UnreadNotificationCount = await _context.Notification
                    .CountAsync(n => n.UserId == userId && !n.IsRead)
            };
        }

        // STAKEHOLDER DASHBOARD DATA
        private async Task<StakeholderDashboardViewModel> GetStakeholderDashboardData(int userId)
        {
            var allProjects = await _context.Project
                .Include(p => p.Team)
                .ThenInclude(t => t != null ? t.TeamMembers : null)
                .ToListAsync();

            var allTasks = await _context.Tasks
                .Include(t => t.Project)
                .ToListAsync();

            var projectReports = await _context.ProjectReport
                .Include(pr => pr.Project)
                .OrderByDescending(pr => pr.CreatedAt)
                .Take(10)
                .ToListAsync();

            return new StakeholderDashboardViewModel
            {
                AllProjects = allProjects,
                ProjectProgress = GetProjectProgressData(allProjects, allTasks),
                ProjectReports = projectReports,
                HighLevelMetrics = GetHighLevelMetrics(allProjects, allTasks),
                RecentNotifications = await _context.Notification
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                UnreadNotificationCount = await _context.Notification
                    .CountAsync(n => n.UserId == userId && !n.IsRead)
            };
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

        // HELPER METHODS
        private List<OmintakProduction.Models.Task> GetTaskDependencies(List<int> taskIds)
        {
            // This would need to be implemented based on how you store task dependencies
            // For now, returning empty list
            return new List<OmintakProduction.Models.Task>();
        }

        private Dictionary<string, object> GetTeamWorkloadDistribution(List<User> teamMembers, List<OmintakProduction.Models.Task> allTasks)
        {
            var workload = new Dictionary<string, object>();
            
            foreach (var member in teamMembers)
            {
                var memberTasks = allTasks.Where(t => t.AssignedUsers.Any(u => u.UserId == member.UserId)).ToList();
                workload[member.UserName ?? member.Email ?? $"User {member.UserId}"] = new
                {
                    TotalTasks = memberTasks.Count,
                    ActiveTasks = memberTasks.Count(t => t.Status == TaskStatus.InProgress || t.Status == TaskStatus.Todo),
                    CompletedTasks = memberTasks.Count(t => t.Status == TaskStatus.Completed),
                    OverdueTasks = memberTasks.Count(t => t.DueDate < DateTime.Now && t.Status != TaskStatus.Completed)
                };
            }
            
            return workload;
        }

        private Dictionary<string, object> GetProjectProgressData(List<Project> projects, List<OmintakProduction.Models.Task> allTasks)
        {
            var progress = new Dictionary<string, object>();
            
            foreach (var project in projects)
            {
                var projectTasks = allTasks.Where(t => t.ProjectId == project.ProjectId).ToList();
                var totalTasks = projectTasks.Count;
                var completedTasks = projectTasks.Count(t => t.Status == TaskStatus.Completed);
                
                progress[project.ProjectName] = new
                {
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    ProgressPercentage = totalTasks > 0 ? (completedTasks * 100.0 / totalTasks) : 0,
                    OverdueTasks = projectTasks.Count(t => t.DueDate < DateTime.Now && t.Status != TaskStatus.Completed),
                    Status = project.Status
                };
            }
            
            return progress;
        }

        private Dictionary<string, object> GetSystemStatistics(List<User> users, List<Project> projects, List<OmintakProduction.Models.Task> tasks, List<Ticket> tickets)
        {
            return new Dictionary<string, object>
            {
                ["TotalUsers"] = users.Count,
                ["ActiveUsers"] = users.Count(u => u.isActive),
                ["PendingApprovals"] = users.Count(u => !u.IsApproved),
                ["TotalProjects"] = projects.Count,
                ["ActiveProjects"] = projects.Count(p => p.Status == "Active"),
                ["TotalTasks"] = tasks.Count,
                ["CompletedTasks"] = tasks.Count(t => t.Status == TaskStatus.Completed),
                ["OverdueTasks"] = tasks.Count(t => t.DueDate < DateTime.Now && t.Status != TaskStatus.Completed),
                ["TotalTickets"] = tickets.Count,
                ["OpenTickets"] = tickets.Count(t => t.Status != "Closed")
            };
        }

        private Dictionary<string, object> GetHighLevelMetrics(List<Project> projects, List<OmintakProduction.Models.Task> tasks)
        {
            var totalTasks = tasks.Count;
            var completedTasks = tasks.Count(t => t.Status == TaskStatus.Completed);
            
            return new Dictionary<string, object>
            {
                ["OverallProgress"] = totalTasks > 0 ? (completedTasks * 100.0 / totalTasks) : 0,
                ["ActiveProjects"] = projects.Count(p => p.Status == "Active"),
                ["ProjectsOnTrack"] = projects.Count(p => p.DueDate >= DateOnly.FromDateTime(DateTime.Now)),
                ["ProjectsAtRisk"] = projects.Count(p => p.DueDate < DateOnly.FromDateTime(DateTime.Now) && p.Status != "Completed"),
                ["TotalDeliverables"] = totalTasks,
                ["CompletedDeliverables"] = completedTasks
            };
        }

        // API ENDPOINTS FOR DASHBOARD DATA
        [HttpGet("api/dashboard/developer/{userId}")]
        public async Task<IActionResult> GetDeveloperDashboardApi(int userId)
        {
            var data = await GetDeveloperDashboardData(userId);
            return Json(data);
        }

        [HttpGet("api/dashboard/tester/{userId}")]
        public async Task<IActionResult> GetTesterDashboardApi(int userId)
        {
            var data = await GetTesterDashboardData(userId);
            return Json(data);
        }

        [HttpGet("api/dashboard/teamlead/{userId}")]
        public async Task<IActionResult> GetTeamLeadDashboardApi(int userId)
        {
            var data = await GetTeamLeadDashboardData(userId);
            return Json(data);
        }

        [HttpGet("api/dashboard/systemadmin/{userId}")]
        public async Task<IActionResult> GetSystemAdminDashboardApi(int userId)
        {
            var data = await GetSystemAdminDashboardData(userId);
            return Json(data);
        }

        [HttpGet("api/dashboard/stakeholder/{userId}")]
        public async Task<IActionResult> GetStakeholderDashboardApi(int userId)
        {
            var data = await GetStakeholderDashboardData(userId);
            return Json(data);
        }
    }
}

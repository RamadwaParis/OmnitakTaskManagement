using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using TaskStatus = OmintakProduction.Models.TaskStatus;

namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for Dashboard data operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DashboardApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        /// <returns>Dashboard statistics</returns>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = new
            {
                ActiveTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatus.InProgress || t.Status == TaskStatus.Todo),
                CompletedTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatus.Completed),
                OverdueTasks = await _context.Tasks.CountAsync(t => t.DueDate < DateTime.Now && t.Status != TaskStatus.Completed),
                TotalProjects = await _context.Project.CountAsync(),
                UnreadNotifications = await _context.Notification.CountAsync(n => !n.IsRead),
                TasksByStatus = await _context.Tasks
                    .GroupBy(t => t.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToListAsync()
            };

            return Ok(stats);
        }

        /// <summary>
        /// Populate database with sample dashboard data
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("populate-sample-data")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> PopulateSampleData()
        {
            try
            {
                // Check if we already have data
                var existingTasksCount = await _context.Tasks.CountAsync();
                var existingProjectsCount = await _context.Project.CountAsync();

                if (existingTasksCount > 0 && existingProjectsCount > 0)
                {
                    return Ok(new { message = "Sample data already exists", tasksCount = existingTasksCount, projectsCount = existingProjectsCount });
                }

                // Add sample projects if they don't exist
                if (existingProjectsCount == 0)
                {
                    var sampleProjects = new List<Project>
                    {
                        new Project { ProjectName = "E-Commerce Platform", Description = "Building a modern e-commerce platform with React and .NET", DueDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3)), Status = "Active" },
                        new Project { ProjectName = "Mobile App Development", Description = "Cross-platform mobile app using React Native", DueDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(4)), Status = "Active" },
                        new Project { ProjectName = "API Documentation", Description = "Comprehensive API documentation and testing suite", DueDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)), Status = "Active" }
                    };

                    _context.Project.AddRange(sampleProjects);
                    await _context.SaveChangesAsync();
                }

                // Get user ID (assuming we have at least one user from seeded data)
                var user = await _context.User.FirstOrDefaultAsync();
                var project = await _context.Project.FirstOrDefaultAsync();

                if (user != null && project != null && existingTasksCount == 0)
                {
                    var sampleTasks = new List<Models.Task>
                    {
                        new Models.Task 
                        { 
                            Title = "Fix authentication bug", 
                            Description = "Resolve login issues reported by users", 
                            Status = TaskStatus.InProgress, 
                            Priority = TaskPriority.High, 
                            AssignedUsers = user != null ? new List<User> { user } : new List<User>(), 
                            CreatedByUserId = user?.UserId ?? 0, 
                            ProjectId = project?.ProjectId ?? 0, 
                            DueDate = DateTime.Now.AddDays(2), 
                            CreatedAt = DateTime.Now.AddHours(-3) 
                        },
                        new Models.Task 
                        { 
                            Title = "Update dashboard design", 
                            Description = "Implement new UI/UX designs for the dashboard", 
                            Status = TaskStatus.InReview, 
                            Priority = TaskPriority.Medium, 
                            AssignedUsers = user != null ? new List<User> { user } : new List<User>(), 
                            CreatedByUserId = user?.UserId ?? 0, 
                            ProjectId = project?.ProjectId ?? 0, 
                            DueDate = DateTime.Now.AddDays(5), 
                            CreatedAt = DateTime.Now.AddHours(-2) 
                        },
                        new Models.Task 
                        { 
                            Title = "Implement API documentation", 
                            Description = "Create comprehensive API documentation with Swagger", 
                            Status = TaskStatus.Todo, 
                            Priority = TaskPriority.Medium, 
                            AssignedUsers = user != null ? new List<User> { user } : new List<User>(), 
                            CreatedByUserId = user?.UserId ?? 0, 
                            ProjectId = project?.ProjectId ?? 0, 
                            DueDate = DateTime.Now.AddDays(7), 
                            CreatedAt = DateTime.Now.AddHours(-1) 
                        },
                        new Models.Task 
                        { 
                            Title = "Database optimization", 
                            Description = "Optimize database queries for better performance", 
                            Status = TaskStatus.Completed, 
                            Priority = TaskPriority.High, 
                            AssignedUsers = user != null ? new List<User> { user } : new List<User>(), 
                            CreatedByUserId = user?.UserId ?? 0, 
                            ProjectId = project?.ProjectId ?? 0, 
                            DueDate = DateTime.Now.AddDays(-2), 
                            CreatedAt = DateTime.Now.AddDays(-3), 
                            CompletedAt = DateTime.Now.AddDays(-1) 
                        },
                        new Models.Task 
                        { 
                            Title = "Setup CI/CD pipeline", 
                            Description = "Configure automated deployment pipeline", 
                            Status = TaskStatus.InProgress, 
                            Priority = TaskPriority.High, 
                            AssignedUsers = user != null ? new List<User> { user } : new List<User>(), 
                            CreatedByUserId = user?.UserId ?? 0, 
                            ProjectId = project?.ProjectId ?? 0, 
                            DueDate = DateTime.Now.AddDays(-1), // Overdue 
                            CreatedAt = DateTime.Now.AddDays(-5) 
                        }
                    };

                    _context.Tasks.AddRange(sampleTasks);

                    // Add sample notifications
                    var sampleNotifications = new List<Notification>
                    {
                        new Notification 
                        { 
                            UserId = user?.UserId ?? 0, 
                            Title = "New Task Assignment", 
                            Message = "You have been assigned to 'Fix authentication bug'", 
                            Type = NotificationType.Task, 
                            IsRead = false, 
                            CreatedAt = DateTime.Now.AddMinutes(-30) 
                        },
                        new Notification 
                        { 
                            UserId = user?.UserId ?? 0, 
                            Title = "Task Review Required", 
                            Message = "Task 'Update dashboard design' is ready for review", 
                            Type = NotificationType.Task, 
                            IsRead = false, 
                            CreatedAt = DateTime.Now.AddHours(-2) 
                        },
                        new Notification 
                        { 
                            UserId = user?.UserId ?? 0, 
                            Title = "Deadline Approaching", 
                            Message = "Task 'Setup CI/CD pipeline' deadline is approaching", 
                            Type = NotificationType.Warning, 
                            IsRead = true, 
                            CreatedAt = DateTime.Now.AddHours(-4), 
                            ReadAt = DateTime.Now.AddHours(-3) 
                        }
                    };

                    _context.Notification.AddRange(sampleNotifications);
                    await _context.SaveChangesAsync();

                    return Ok(new { 
                        message = "Sample data populated successfully", 
                        tasksAdded = sampleTasks.Count, 
                        projectsAdded = 3, 
                        notificationsAdded = sampleNotifications.Count 
                    });
                }

                return Ok(new { message = "Sample data populated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Failed to populate sample data", details = ex.Message });
            }
        }

        /// <summary>
        /// Clear all dashboard data (for testing purposes)
        /// </summary>
        /// <returns>Success message</returns>
        [HttpDelete("clear-data")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> ClearDashboardData()
        {
            try
            {
                _context.TaskHistory.RemoveRange(_context.TaskHistory);
                _context.TaskComment.RemoveRange(_context.TaskComment);
                _context.TaskAssignment.RemoveRange(_context.TaskAssignment);
                _context.Notification.RemoveRange(_context.Notification);
                _context.Tasks.RemoveRange(_context.Tasks);
                _context.Project.RemoveRange(_context.Project.Where(p => p.ProjectId > 1)); // Keep first project if seeded

                await _context.SaveChangesAsync();

                return Ok(new { message = "Dashboard data cleared successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Failed to clear data", details = ex.Message });
            }
        }
    }
}

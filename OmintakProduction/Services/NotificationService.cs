using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;

namespace OmintakProduction.Services
{
    public interface INotificationService
    {
        System.Threading.Tasks.Task NotifyUserAssignedToTeam(int userId, int teamId, string teamName);
        System.Threading.Tasks.Task NotifyUserAssignedToProject(int userId, int projectId, string projectName);
        System.Threading.Tasks.Task NotifyUserAssignedToTicket(int userId, int ticketId, string ticketTitle);
        System.Threading.Tasks.Task NotifyUserAssignedToTask(int userId, int taskId, string taskTitle);
        System.Threading.Tasks.Task NotifyTeamMembersNewProject(int teamId, int projectId, string projectName);
        System.Threading.Tasks.Task NotifyMultipleUsers(List<int> userIds, string title, string message, NotificationType type, int? relatedEntityId = null, string? relatedEntityType = null);
        System.Threading.Tasks.Task<List<Notification>> GetUserNotifications(int userId, bool unreadOnly = false);
        System.Threading.Tasks.Task MarkAsRead(int notificationId, int userId);
        System.Threading.Tasks.Task MarkAllAsRead(int userId);
    }

    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task NotifyUserAssignedToTeam(int userId, int teamId, string teamName)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = "Team Assignment",
                Message = $"You have been assigned to team: <b>{teamName}</b>",
                Type = NotificationType.Info,
                CreatedAt = DateTime.UtcNow,
                RelatedEntityId = teamId,
                RelatedEntityType = "Team"
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task NotifyUserAssignedToProject(int userId, int projectId, string projectName)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = "Project Assignment",
                Message = $"You have been assigned to project: <b>{projectName}</b>",
                Type = NotificationType.Project,
                CreatedAt = DateTime.UtcNow,
                RelatedEntityId = projectId,
                RelatedEntityType = "Project",
                ActionUrl = $"/Project/Details/{projectId}"
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task NotifyUserAssignedToTicket(int userId, int ticketId, string ticketTitle)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = "Ticket Assignment",
                Message = $"A ticket has been assigned to you: <b>{ticketTitle}</b>",
                Type = NotificationType.Info,
                CreatedAt = DateTime.UtcNow,
                RelatedEntityId = ticketId,
                RelatedEntityType = "Ticket",
                ActionUrl = $"/Ticket/Details/{ticketId}"
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task NotifyUserAssignedToTask(int userId, int taskId, string taskTitle)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = "Task Assignment",
                Message = $"A task has been assigned to you: <b>{taskTitle}</b>",
                Type = NotificationType.Task,
                CreatedAt = DateTime.UtcNow,
                RelatedEntityId = taskId,
                RelatedEntityType = "Task",
                ActionUrl = $"/Task/Details/{taskId}"
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task NotifyTeamMembersNewProject(int teamId, int projectId, string projectName)
        {
            var team = await _context.Team
                .Include(t => t.TeamMembers.Where(u => u.isActive))
                .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team?.TeamMembers != null)
            {
                var notifications = team.TeamMembers.Select(member => new Notification
                {
                    UserId = member.UserId,
                    Title = "New Project Assigned",
                    Message = $"Your team <b>{team.TeamName}</b> has been assigned a new project: <b>{projectName}</b>",
                    Type = NotificationType.Project,
                    CreatedAt = DateTime.UtcNow,
                    RelatedEntityId = projectId,
                    RelatedEntityType = "Project",
                    ActionUrl = $"/Project/Details/{projectId}"
                }).ToList();

                _context.Notification.AddRange(notifications);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task NotifyMultipleUsers(List<int> userIds, string title, string message, NotificationType type, int? relatedEntityId = null, string? relatedEntityType = null)
        {
            var notifications = userIds.Select(userId => new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                RelatedEntityId = relatedEntityId,
                RelatedEntityType = relatedEntityType
            }).ToList();

            _context.Notification.AddRange(notifications);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<List<Notification>> GetUserNotifications(int userId, bool unreadOnly = false)
        {
            var query = _context.Notification
                .Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(50) // Limit to recent 50 notifications
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task MarkAsRead(int notificationId, int userId)
        {
            var notification = await _context.Notification
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task MarkAllAsRead(int userId)
        {
            var unreadNotifications = await _context.Notification
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
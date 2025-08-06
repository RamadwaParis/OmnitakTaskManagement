using OmintakProduction.Models;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;

namespace OmintakProduction.Services
{
    public interface IUserApprovalService
    {
        Task<bool> NotifyAdminsOfNewRegistration(User user);
        Task<bool> NotifyUserOfApproval(User user, string roleName, string? teamName = null);
        Task<bool> NotifyUserOfRejection(User user, string reason = "");
        Task<List<User>> GetPendingApprovalUsers();
        Task<bool> ApproveUser(int userId, string newUsername, int roleId, int? teamId, int? projectId, string firstName, string lastName);
        Task<bool> RejectUser(int userId, string reason = "");
    }

    public class UserApprovalService : IUserApprovalService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public UserApprovalService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<bool> NotifyAdminsOfNewRegistration(User user)
        {
            try
            {
                // Get all system admins
                var admins = await _context.User
                    .Include(u => u.Role)
                    .Where(u => u.Role != null && u.Role.RoleName == "SystemAdmin" && u.isActive)
                    .ToListAsync();

                foreach (var admin in admins)
                {
                    var notification = new Notification
                    {
                        UserId = admin.UserId,
                        Title = "New User Registration",
                        Message = $"A new user <b>{user.UserName}</b> ({user.Email}) has registered and is pending approval.",
                        Type = NotificationType.Info,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        RelatedEntityId = user.UserId,
                        RelatedEntityType = "User"
                    };
                    _context.Notification.Add(notification);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> NotifyUserOfApproval(User user, string roleName, string? teamName = null)
        {
            try
            {
                string teamMessage = !string.IsNullOrEmpty(teamName) ? $" and assigned to team <b>{teamName}</b>" : "";
                
                var notification = new Notification
                {
                    UserId = user.UserId,
                    Title = "Account Approved - Welcome to Omnitak!",
                    Message = $"Your account has been approved with role <b>{roleName}</b>{teamMessage}. You can now login and start using the system.",
                    Type = NotificationType.Success,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    RelatedEntityId = user.RoleId,
                    RelatedEntityType = "Role"
                };
                
                _context.Notification.Add(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> NotifyUserOfRejection(User user, string reason = "")
        {
            try
            {
                string reasonMessage = !string.IsNullOrEmpty(reason) ? $" Reason: {reason}" : "";
                
                var notification = new Notification
                {
                    UserId = user.UserId,
                    Title = "Account Registration Rejected",
                    Message = $"Unfortunately, your account registration has been rejected.{reasonMessage} Please contact support if you have questions.",
                    Type = NotificationType.Error,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    RelatedEntityId = user.UserId,
                    RelatedEntityType = "User"
                };
                
                _context.Notification.Add(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<User>> GetPendingApprovalUsers()
        {
            return await _context.User
                .Where(u => !u.IsApproved && !u.IsDeleted)
                .OrderBy(u => u.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> ApproveUser(int userId, string newUsername, int roleId, int? teamId, int? projectId, string firstName, string lastName)
        {
            try
            {
                var user = await _context.User.FindAsync(userId);
                var team = teamId.HasValue ? await _context.Team.FindAsync(teamId.Value) : null;
                var role = await _context.Role.FindAsync(roleId);

                if (user == null || role == null || string.IsNullOrWhiteSpace(newUsername) || 
                    string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                {
                    return false;
                }

                // Update user information
                user.UserName = newUsername;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.RoleId = roleId;
                user.isActive = true;
                user.IsApproved = true;
                user.TeamId = teamId;
                user.ProjectId = projectId;

                if (team != null)
                {
                    user.Team = team;
                }

                _context.User.Update(user);
                await _context.SaveChangesAsync();

                // Send approval notification
                await NotifyUserOfApproval(user, role.RoleName ?? "Unknown", team?.TeamName);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RejectUser(int userId, string reason = "")
        {
            try
            {
                var user = await _context.User.FindAsync(userId);
                if (user == null) return false;

                // Send rejection notification before marking as deleted
                await NotifyUserOfRejection(user, reason);

                // Mark user as deleted (soft delete)
                user.IsDeleted = true;
                user.DeletedAt = DateTime.Now;

                _context.User.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;

namespace OmintakProduction.Data
{
    public class UserManagementService
    {
        private readonly AppDbContext _context;

        public UserManagementService(AppDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task RemoveAllUsersExceptSystemAdmin()
        {
            // Get all users except the System Admin (RoleId = 1)
            var usersToRemove = await _context.User
                .Where(u => u.RoleId != 1) // Keep only System Admin
                .ToListAsync();

            if (usersToRemove.Any())
            {
                // Soft delete instead of hard delete to preserve data integrity
                foreach (var user in usersToRemove)
                {
                    user.IsDeleted = true;
                    user.DeletedAt = DateTime.UtcNow;
                    user.DeletedByUserId = 1; // System Admin UserId
                    user.isActive = false;
                    user.IsApproved = false;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task<bool> ApproveUserAndAssignRole(int userId, int roleId, int? teamId = null, int? projectId = null)
        {
            var user = await _context.User.FindAsync(userId);
            if (user == null || user.IsDeleted) 
                return false;

            // Update user status and role
            user.IsApproved = true;
            user.isActive = true;
            user.RoleId = roleId;
            user.TeamId = teamId;
            user.ProjectId = projectId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async System.Threading.Tasks.Task<List<User>> GetPendingUsers()
        {
            return await _context.User
                .Where(u => !u.IsApproved && !u.IsDeleted && u.RoleId != 1)
                .ToListAsync();
        }
    }
}

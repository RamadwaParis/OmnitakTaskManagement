using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Models;
using System.Threading.Tasks;

namespace OmintakProduction.Data
{
    public static class UserSeeder
    {
        public static async System.Threading.Tasks.Task SeedUsersAsync(AppDbContext context)
        {
            // Create roles if they don't exist
            var roleNames = new[]
            {
                "SystemAdmin",
                "ProjectLead", 
                "Developer",
                "Tester",
                "DepartmentHead",
                "OperationsManager",
                "BusinessAnalyst",
                "ProductOwner",
                "ResourceManager",
                "DepartmentLead",
                "ProcessImprovementSpecialist"
            };

            var existingRoles = await context.Role.Select(r => r.RoleName).ToListAsync();
            foreach (var roleName in roleNames)
            {
                if (!existingRoles.Contains(roleName))
                {
                    context.Role.Add(new Role { RoleName = roleName });
                    await context.SaveChangesAsync();
                    context.ChangeTracker.Clear();
                }
            }

            // Main Users
            var mainUsers = new[]
            {
                new { Email = "uthandocibi@gmail.com", FirstName = "Thando", LastName = "Cibi", Role = "SystemAdmin", IsApproved = true }
            };

            // Remove all notifications before users to avoid FK constraint errors
            var allNotifications = await context.Notification.ToListAsync();
            context.Notification.RemoveRange(allNotifications);
            await context.SaveChangesAsync();

            // Unassign all tickets before users to avoid FK constraint errors
            var allTickets = await context.Ticket.ToListAsync();
            foreach (var ticket in allTickets)
            {
                if (ticket.AssignedToUserId != null)
                {
                    ticket.AssignedToUserId = null;
                }
            }
            context.Ticket.UpdateRange(allTickets);
            await context.SaveChangesAsync();

            // Unassign all tasks' CreatedByUserId before deleting users to avoid FK constraint errors
            // Set to a valid admin user (first seeded user)
            var adminUser = await context.User.FirstOrDefaultAsync(u => u.Email == "uthandocibi@gmail.com");
            List<Models.Task> allTasks = new List<Models.Task>();
            if (adminUser != null)
            {
                allTasks = await context.Tasks.ToListAsync();
                foreach (var task in allTasks)
                {
                    task.CreatedByUserId = adminUser.UserId;
                }
                context.Tasks.UpdateRange(allTasks);
                await context.SaveChangesAsync();
            }

            // Remove all other seeded users and stakeholders
            // Detach all navigation properties to avoid EF severed relationship errors
            if (allTasks.Count == 0) allTasks = await context.Tasks.ToListAsync();
            foreach (var task in allTasks) {
                // Only detach if the CreatedByUserId is not the admin (still valid)
                if (task.CreatedByUserId != adminUser?.UserId)
                    task.CreatedByUser = null;
            }
            context.Tasks.UpdateRange(allTasks);
            await context.SaveChangesAsync();
            var allUsers = await context.User.ToListAsync();
            context.User.RemoveRange(allUsers.Where(u => u.UserId != adminUser?.UserId));
            await context.SaveChangesAsync();

            // Create main users
            foreach (var userData in mainUsers)
            {
                var existingUser = await context.User.FirstOrDefaultAsync(u => u.Email == userData.Email);
                if (existingUser == null)
                {
                    var role = await context.Role.FirstOrDefaultAsync(r => r.RoleName == userData.Role); // Always fetch fresh
                    if (role != null)
                    {
                        var user = new User
                        {
                            UserName = userData.Email,
                            Email = userData.Email,
                            FirstName = userData.FirstName,
                            LastName = userData.LastName,
                            Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                            IsApproved = userData.IsApproved,
                            isActive = userData.IsApproved,
                            RoleId = role.RoleId,
                            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                            NeedsWelcome = true
                        };
                        context.User.Add(user);
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}

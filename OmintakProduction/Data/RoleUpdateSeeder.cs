using Microsoft.EntityFrameworkCore;
using OmintakProduction.Models;

namespace OmintakProduction.Data
{
    public static class RoleUpdateSeeder
    {
        public static async System.Threading.Tasks.Task UpdateUserRolesAsync(AppDbContext context)
        {
            // First, ensure all required roles exist
            var requiredRoles = new[]
            {
                "SystemAdmin",
                "ProjectLead", 
                "Developer",
                "Tester",
                "Stakeholder",
                "TeamLead"
            };

            foreach (var roleName in requiredRoles)
            {
                var existingRole = await context.Role.FirstOrDefaultAsync(r => r.RoleName == roleName);
                if (existingRole == null)
                {
                    context.Role.Add(new Role { RoleName = roleName });
                    await context.SaveChangesAsync();
                }
            }

            // Get role IDs
            var teamLeadRole = await context.Role.FirstOrDefaultAsync(r => r.RoleName == "TeamLead");
            var stakeholderRole = await context.Role.FirstOrDefaultAsync(r => r.RoleName == "Stakeholder");
            var testerRole = await context.Role.FirstOrDefaultAsync(r => r.RoleName == "Tester");
            var developerRole = await context.Role.FirstOrDefaultAsync(r => r.RoleName == "Developer");

            if (teamLeadRole == null || stakeholderRole == null || testerRole == null || developerRole == null)
            {
                throw new InvalidOperationException("Required roles not found in database");
            }

            // Update user roles as requested
            var userUpdates = new[]
            {
                new { Email = "dumisaninxumalo5gmail.com", FirstName = "Dumisani", LastName = "Nxumalo", NewRoleId = teamLeadRole.RoleId },
                new { Email = "tledwaba@dynamicdna.co.za", FirstName = "Thabang", LastName = "Ledwaba", NewRoleId = stakeholderRole.RoleId },
                new { Email = "ramadwaparis@gmail.com", FirstName = "Paris", LastName = "Ramadwa", NewRoleId = testerRole.RoleId },
                new { Email = "ayakazilungile20@gmail.com", FirstName = "Zilungile", LastName = "Nquku", NewRoleId = developerRole.RoleId }
            };

            foreach (var update in userUpdates)
            {
                var user = await context.User.FirstOrDefaultAsync(u => 
                    u.Email == update.Email || 
                    (u.FirstName == update.FirstName && u.LastName == update.LastName));
                
                if (user != null)
                {
                    user.RoleId = update.NewRoleId;
                    context.User.Update(user);
                    Console.WriteLine($"Updated {user.FirstName} {user.LastName} role to {context.Role.Find(update.NewRoleId)?.RoleName}");
                }
                else
                {
                    Console.WriteLine($"User {update.FirstName} {update.LastName} not found");
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine("User role updates completed successfully");
        }
    }
}

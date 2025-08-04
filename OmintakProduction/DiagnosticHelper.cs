using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using BCrypt.Net;

namespace OmintakProduction
{
    public class DiagnosticHelper
    {
        public static async System.Threading.Tasks.Task CheckSystemAdminUser(AppDbContext context)
        {
            Console.WriteLine("=== SYSTEM ADMIN USER DIAGNOSTIC ===");
            
            var systemAdmin = await context.User
                .Where(u => u.Email == "uthandocibi@gmail.com")
                .FirstOrDefaultAsync();
            
            if (systemAdmin == null)
            {
                Console.WriteLine("❌ System Admin user NOT FOUND in database!");
                return;
            }
            
            Console.WriteLine($"✅ System Admin user found:");
            Console.WriteLine($"   UserId: {systemAdmin.UserId}");
            Console.WriteLine($"   Email: {systemAdmin.Email}");
            Console.WriteLine($"   UserName: {systemAdmin.UserName}");
            Console.WriteLine($"   FirstName: {systemAdmin.FirstName}");
            Console.WriteLine($"   LastName: {systemAdmin.LastName}");
            Console.WriteLine($"   RoleId: {systemAdmin.RoleId}");
            Console.WriteLine($"   isActive: {systemAdmin.isActive}");
            Console.WriteLine($"   IsApproved: {systemAdmin.IsApproved}");
            Console.WriteLine($"   IsDeleted: {systemAdmin.IsDeleted}");
            Console.WriteLine($"   Password Hash: {systemAdmin.Password}");
            
            // Test password verification
            bool passwordValid = BCrypt.Net.BCrypt.Verify("password", systemAdmin.Password);
            Console.WriteLine($"   Password 'password' verification: {passwordValid}");
            
            // Check role
            if (systemAdmin.RoleId > 0)
            {
                var role = new Role().getRole(systemAdmin.RoleId);
                Console.WriteLine($"   Role: {role}");
            }
            
            // Check if user can login
            bool canLogin = systemAdmin.isActive && systemAdmin.IsApproved && !systemAdmin.IsDeleted;
            Console.WriteLine($"   Can Login: {canLogin}");
            
            if (!canLogin)
            {
                Console.WriteLine("❌ LOGIN ISSUES DETECTED:");
                if (!systemAdmin.isActive) Console.WriteLine("   - User is not active");
                if (!systemAdmin.IsApproved) Console.WriteLine("   - User is not approved");
                if (systemAdmin.IsDeleted) Console.WriteLine("   - User is deleted");
            }
        }
    }
}

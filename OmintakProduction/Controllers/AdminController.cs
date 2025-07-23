using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> PendingUsers()
        {
            var pendingUsers = await _context.User
                .Where(u => !u.IsApproved)
                .ToListAsync();

            var roles = await _context.Role.ToListAsync();
            ViewData["Roles"] = roles;

            return View(pendingUsers);
        }

        [HttpPost]
        [Route("Admin/ApproveUser")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> ApproveUser(int id, int roleId)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();

            user.IsApproved = true;
            user.isActive = true;
            user.RoleId = roleId;
            user.NeedsWelcome = true; // Set to show welcome screen on first login
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User {user.FirstName} {user.LastName} has been approved and assigned the selected role.";
            return RedirectToAction("PendingUsers");
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> RejectUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "User has been rejected and removed from the system.";
            return RedirectToAction("PendingUsers");
        }

        [HttpPost]
        [Route("Admin/ApproveUserJson")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> ApproveUser([FromBody] ApprovalRequest request)
        {
            try
            {
                var user = await _context.User.FindAsync(request.UserId);
                if (user == null) 
                    return Json(new { success = false, message = "User not found" });

                // Get role by name
                var role = await _context.Role.FirstOrDefaultAsync(r => r.RoleName == request.Role);
                if (role == null)
                    return Json(new { success = false, message = "Role not found" });

                user.IsApproved = true;
                user.isActive = true;
                user.RoleId = role.RoleId;
                user.NeedsWelcome = true;
                
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"User approved as {request.Role}" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error approving user" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> RejectUser([FromBody] RejectRequest request)
        {
            try
            {
                var user = await _context.User.FindAsync(request.UserId);
                if (user == null)
                    return Json(new { success = false, message = "User not found" });

                _context.User.Remove(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "User rejected and removed" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error rejecting user" });
            }
        }

        public class ApprovalRequest
        {
            public int UserId { get; set; }
            public required string Role { get; set; }
        }

        public class RejectRequest
        {
            public int UserId { get; set; }
        }
    }
}

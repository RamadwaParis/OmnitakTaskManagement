using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;

namespace OmintakProduction.Controllers
{
    
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.User.Where(u => u.isActive == true && !u.IsDeleted).ToList();

            return View(users);
        }
        public IActionResult GetIndividualUser(int id)
        {
            var user = _context.User.Where(u => u.UserId == id).ToList();

            return View(user);
        }

        public IActionResult GetAllUsers()
        {
            var users = _context.User.Where(u => !u.IsDeleted).ToList();

            return View(users);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.User.FindAsync(id);

            if (user == null) return NotFound();

            return View("~/Views/User/Update.cshtml", user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("UserId,UserName,Email,Password,CreatedDate,isActive")] User updatedUser) // Make sure UserId is bound
        {
            if (id != updatedUser.UserId) return NotFound();

            var existingUser = await _context.User.FindAsync(id);
            if (existingUser == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Correctly assign values from updatedUser to existingUser
                    existingUser.UserName = updatedUser.UserName;
                    //existingUser.Email = updatedUser.Email;
                    existingUser.Password = updatedUser.Password; // Be careful with password handling in real apps (hashing!)
                    existingUser.CreatedDate = updatedUser.CreatedDate;
                    existingUser.isActive = updatedUser.isActive; // Assuming you want to update isActive as well

                    _context.Update(existingUser); // Or _context.Entry(existingUser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsertExists(updatedUser.UserId)) // Check if the user still exists
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Re-throw if it's another concurrency issue
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // If ModelState is not valid, return the view with the invalid model
            return View("~/Views/User/Update.cshtml", updatedUser);
        }



        public IActionResult Delete()
        {

            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.User
                .FirstOrDefaultAsync(t => t.UserId == id);

            return user == null ? NotFound() : View("~/Views/User/Delete.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Implement soft delete
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.isActive = false;
            
            // Get current user ID from claims for audit trail
            var currentUserIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (currentUserIdClaim != null && int.TryParse(currentUserIdClaim.Value, out int currentUserId))
            {
                user.DeletedByUserId = currentUserId;
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User {user.FirstName} {user.LastName} has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete([FromBody] SoftDeleteRequest request)
        {
            try
            {
                var user = await _context.User.FindAsync(request.UserId);
                if (user == null)
                    return Json(new { success = false, message = "User not found" });

                // Implement soft delete
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                user.isActive = false;

                // Get current user ID from claims for audit trail
                var currentUserIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (currentUserIdClaim != null && int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    user.DeletedByUserId = currentUserId;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "User deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error deleting user" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusRequest request)
        {
            try
            {
                var user = await _context.User.FindAsync(request.UserId);
                if (user == null)
                    return Json(new { success = false, message = "User not found" });

                user.isActive = request.Enable;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"User {(request.Enable ? "enabled" : "disabled")} successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error updating user status" });
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.User
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            // Get the role name
            var role = await _context.Role.FindAsync(user.RoleId);
            
            ViewBag.RoleName = role?.RoleName ?? "Unknown Role";
            ViewBag.TeamName = user.Team?.TeamName ?? "No Team Assigned";

            return View(user);
        }

        private bool UsertExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }

        public class ToggleStatusRequest
        {
            public int UserId { get; set; }
            public bool Enable { get; set; }
        }

        public class SoftDeleteRequest
        {
            public int UserId { get; set; }
        }
    }
}

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
            var pendingUsers = await _context.Users
                .Where(u => !u.isActive)
                .ToListAsync();

            return View(pendingUsers);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> ApproveUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.isActive = true;
            await _context.SaveChangesAsync();

            // To do: Send notification to user
            return RedirectToAction("PendingUsers");
        }
    }
}

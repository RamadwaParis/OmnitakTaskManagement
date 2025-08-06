using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Security.Claims;

namespace OmintakProduction.ViewComponents
{
    public class AdminPendingApprovalsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public AdminPendingApprovalsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Check if user is a SystemAdmin
            var userRole = UserClaimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "SystemAdmin")
            {
                return View(new List<User>());
            }

            // Get pending user approvals with full user details
            var pendingUsers = await _context.User
                .Where(u => !u.IsApproved && !u.IsDeleted)
                .ToListAsync();

            return View(pendingUsers);
        }
    }
}

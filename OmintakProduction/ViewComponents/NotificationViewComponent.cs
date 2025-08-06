using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;

namespace OmintakProduction.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public NotificationViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdClaim = UserClaimsPrincipal.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return View(0);
            }

            var unreadCount = await _context.Notification
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            return View(unreadCount); // This will look for Default.cshtml in the correct location
        }
    }
}
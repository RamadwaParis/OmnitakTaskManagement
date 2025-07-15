using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    /// <summary>
    /// API Controller for managing Notifications
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class NotificationApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all notifications
        /// </summary>
        /// <returns>List of all notifications</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Notification>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Notification.OrderByDescending(n => n.CreatedAt).ToListAsync());
        }

        /// <summary>
        /// Get a specific notification by ID
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>Notification details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Notification), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification == null) return NotFound();
            return Ok(notification);
        }

        /// <summary>
        /// Create a new notification
        /// </summary>
        /// <param name="notification">Notification data</param>
        /// <returns>Created notification</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Notification), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(Notification notification)
        {
            notification.CreatedAt = DateTime.Now;
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = notification.Id }, notification);
        }

        /// <summary>
        /// Update an existing notification
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <param name="notification">Updated notification data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, Notification notification)
        {
            if (id != notification.Id) return BadRequest();
            _context.Entry(notification).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await NotificationExists(id))
                    return NotFound();
                throw;
            }
            
            return NoContent();
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification == null) return NotFound();
            _context.Notification.Remove(notification);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Get notifications for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of notifications for the specified user</returns>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), 200)]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var notifications = await _context.Notification
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return Ok(notifications);
        }

        /// <summary>
        /// Get unread notifications for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of unread notifications for the specified user</returns>
        [HttpGet("unread/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), 200)]
        public async Task<IActionResult> GetUnreadByUser(int userId)
        {
            var notifications = await _context.Notification
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return Ok(notifications);
        }

        /// <summary>
        /// Get notifications by type
        /// </summary>
        /// <param name="type">Notification type (Info, Warning, Success, Error, Task, Project, User)</param>
        /// <returns>List of notifications of the specified type</returns>
        [HttpGet("by-type/{type}")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), 200)]
        public async Task<IActionResult> GetByType(NotificationType type)
        {
            var notifications = await _context.Notification
                .Where(n => n.Type == type)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return Ok(notifications);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>No content if successful</returns>
        [HttpPatch("{id}/mark-read")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Mark all notifications as read for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Number of notifications marked as read</returns>
        [HttpPatch("mark-all-read/{userId}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            var unreadNotifications = await _context.Notification
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok(new { MarkedCount = unreadNotifications.Count });
        }

        /// <summary>
        /// Get notification count for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Notification counts (total and unread)</returns>
        [HttpGet("count/{userId}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetNotificationCount(int userId)
        {
            var totalCount = await _context.Notification.CountAsync(n => n.UserId == userId);
            var unreadCount = await _context.Notification.CountAsync(n => n.UserId == userId && !n.IsRead);

            return Ok(new { 
                TotalCount = totalCount, 
                UnreadCount = unreadCount 
            });
        }

        /// <summary>
        /// Create a bulk notification for multiple users
        /// </summary>
        /// <param name="request">Bulk notification request</param>
        /// <returns>Number of notifications created</returns>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateBulkNotification([FromBody] BulkNotificationRequest request)
        {
            if (request.UserIds == null || !request.UserIds.Any())
                return BadRequest("UserIds cannot be empty");

            var notifications = new List<Notification>();
            foreach (var userId in request.UserIds)
            {
                notifications.Add(new Notification
                {
                    UserId = userId,
                    Title = request.Title,
                    Message = request.Message,
                    Type = request.Type,
                    RelatedEntityId = request.RelatedEntityId,
                    RelatedEntityType = request.RelatedEntityType,
                    ActionUrl = request.ActionUrl,
                    CreatedAt = DateTime.Now
                });
            }

            _context.Notification.AddRange(notifications);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { CreatedCount = notifications.Count });
        }

        /// <summary>
        /// Delete all read notifications for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Number of notifications deleted</returns>
        [HttpDelete("clear-read/{userId}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> ClearReadNotifications(int userId)
        {
            var readNotifications = await _context.Notification
                .Where(n => n.UserId == userId && n.IsRead)
                .ToListAsync();

            _context.Notification.RemoveRange(readNotifications);
            await _context.SaveChangesAsync();

            return Ok(new { DeletedCount = readNotifications.Count });
        }

        private async Task<bool> NotificationExists(int id)
        {
            return await _context.Notification.AnyAsync(e => e.Id == id);
        }
    }

    /// <summary>
    /// Request model for bulk notification creation
    /// </summary>
    public class BulkNotificationRequest
    {
        public List<int> UserIds { get; set; } = new();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.Info;
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
        public string? ActionUrl { get; set; }
    }
}

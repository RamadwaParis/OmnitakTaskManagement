
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using OmintakProduction.Services;
using System.Security.Claims;

namespace OmintakProduction.Controllers
{
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        
        public TicketController(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // POST: Ticket/AssignTicket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTicket(int id, int? assignedToUserId)
        {
            // Check if the current user is authorized to assign tickets
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";
            if (!RolePermissions.HasPermission(userRole, "AssignTickets"))
            {
                return Forbid();
            }

            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // If assignedToUserId is provided, validate the assigned user's role
            if (assignedToUserId.HasValue)
            {
                var assignedUser = await _context.User
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == assignedToUserId.Value);

                if (assignedUser == null)
                {
                    return NotFound("Assigned user not found.");
                }

                // Check if the assigned user has an appropriate role (not SystemAdmin or Stakeholder)
                var assignedUserRole = assignedUser.Role?.RoleName;
                if (assignedUserRole == RoleNames.SystemAdmin.ToString() || 
                    assignedUserRole == RoleNames.Stakeholder.ToString())
                {
                    ModelState.AddModelError("", "Cannot assign tickets to System Administrators or Stakeholders.");
                    return RedirectToAction("Edit", new { id = ticket.Id });
                }
            }

            ticket.AssignedToUserId = assignedToUserId;
            _context.Update(ticket);
            await _context.SaveChangesAsync();
            
            // Notify the assigned user if a user was assigned
            if (assignedToUserId.HasValue)
            {
                await _notificationService.NotifyUserAssignedToTicket(assignedToUserId.Value, ticket.Id, ticket.Title);
            }
            
            return RedirectToAction("Edit", new { id = ticket.Id });
        }

        // Ticket Management Landing Page (Kanban)
        public async Task<IActionResult> Dashboard()
        {
            var tickets = await _context.Ticket
                .Include(t => t.AssignedToUser)
                .Include(t => t.Project)
                .ToListAsync();
            return View("~/Views/Ticket/Dashboard.cshtml", tickets);
        }

        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Ticket
                .Where(t => !t.IsDeleted)
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .ToListAsync();
            return View("~/Views/Ticket/Index.cshtml", tickets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Ticket
                .Include(t => t.Tasks.Where(task => !task.IsDeleted))
                .Include(t => t.AssignedToUser)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (ticket == null) return NotFound();
            return View("~/Views/Ticket/Details.cshtml", ticket);
        }

        public IActionResult Create(int? projectId)
        {
            ViewBag.ProjectList = new SelectList(_context.Project.Include(p => p.Team).ToList(), "ProjectId", "ProjectName");
            if (projectId.HasValue)
            {
                var project = _context.Project.Include(p => p.Team).FirstOrDefault(p => p.ProjectId == projectId.Value);
                var activeTeamUsers = project?.Team?.TeamMembers?.Where(u => u != null && u.isActive).ToList() ?? new List<User>();
                ViewBag.AssignedToList = new SelectList(activeTeamUsers, "UserId", "UserName");
            }
            else
            {
                ViewBag.AssignedToList = new SelectList(_context.User.Where(u => u.isActive).ToList(), "UserId", "UserName");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate,Status,ProjectId,AssignedToUserId")] Ticket ticket)
        {
            // Validate due date is not in the past
            var dateValidation = DateValidationService.ValidateTicketDueDate(ticket.DueDate);
            if (!dateValidation.isValid)
            {
                ModelState.AddModelError("DueDate", dateValidation.errorMessage);
            }
            
            if (ModelState.IsValid)
            {
                ticket.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
                _context.Add(ticket);
                await _context.SaveChangesAsync();

                // Notify assigned user about new ticket
                if (ticket.AssignedToUserId != null)
                {
                    var notification = new Notification {
                        UserId = ticket.AssignedToUserId,
                        Title = "New Ticket Issued",
                        Message = $"A new ticket has been issued and assigned to you: <b>{ticket.Title}</b>.",
                        Type = NotificationType.Task,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        RelatedEntityId = ticket.Id,
                        RelatedEntityType = "Ticket"
                    };
                    _context.Notification.Add(notification);
                    await _context.SaveChangesAsync();
                }

                // After creating a ticket, redirect to the ticket dashboard (kanban/landing page)
                return RedirectToAction("Dashboard");
            }
            var project = _context.Project.Include(p => p.Team).FirstOrDefault(p => p.ProjectId == ticket.ProjectId);
            var activeTeamUsers = (project != null && project.Team != null && project.Team.TeamMembers != null)
                ? project.Team.TeamMembers.Where(u => u != null && u.isActive).ToList()
                : new List<User>();
            ViewBag.ProjectList = new SelectList(_context.Project.Include(p => p.Team).ToList(), "ProjectId", "ProjectName");
            ViewBag.AssignedToList = new SelectList(activeTeamUsers, "UserId", "UserName");
            return View(ticket);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Ticket.FindAsync(id);

            if (ticket == null) return NotFound();

            return View("~/Views/Ticket/Update.cshtml", ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,Title,Description,DueDate,Status")] Ticket updatedTicket)
        {
            if (id != updatedTicket.Id) return NotFound();

            // Validate due date is not in the past
            var dateValidation = DateValidationService.ValidateTicketDueDate(updatedTicket.DueDate);
            if (!dateValidation.isValid)
            {
                ModelState.AddModelError("DueDate", dateValidation.errorMessage);
            }

            var existingTicket = await _context.Ticket.FindAsync(id);
            if (existingTicket == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    existingTicket.Title = updatedTicket.Title;
                    existingTicket.Description = updatedTicket.Description;
                    existingTicket.Status = updatedTicket.Status;

                    _context.Update(existingTicket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(updatedTicket.Id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Ticket/Update.cshtml", updatedTicket);
        }

        public IActionResult Delete()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "TeamLead,SystemAdmin,ProjectLead")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Ticket
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            return ticket == null ? NotFound() : View("~/Views/Ticket/Delete.cshtml", ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TeamLead,SystemAdmin,ProjectLead")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null && !ticket.IsDeleted)
            {
                // Implement soft delete
                ticket.IsDeleted = true;
                ticket.DeletedAt = DateTime.UtcNow;
                
                // Get current user ID from claims for audit trail
                var currentUserIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (currentUserIdClaim != null && int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    ticket.DeletedByUserId = currentUserId;
                }

                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _context.Ticket
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.ProjectList = new SelectList(await _context.Project.ToListAsync(), "ProjectId", "ProjectName");
            List<User> assignedUsers = new List<User>();

            if (ticket.ProjectId.HasValue)
            {
                var project = await _context.Project
                    .Include(p => p.Team)
                    .ThenInclude(t => t != null ? t.TeamMembers.Where(m => m.isActive) : null)
                    .FirstOrDefaultAsync(p => p.ProjectId == ticket.ProjectId.Value);

                if (project?.Team?.TeamMembers != null)
                {
                    assignedUsers = project.Team.TeamMembers.ToList();
                }
            }
            ViewBag.AssignedToList = new SelectList(assignedUsers, "UserId", "UserName");
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Title,Description,DueDate,Status,ProjectId,AssignedToUserId")] Ticket ticket)
        {
            if (ticket == null)
            {
                return NotFound();
            }

            // Check if the current user is authorized to edit tickets
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (!RolePermissions.HasPermission(userRole ?? "", "ManageTickets"))
            {
                return Forbid();
            }

            // If the ticket is being assigned to a user, validate the assigned user's role
            if (ticket.AssignedToUserId.HasValue)
            {
                var assignedUser = await _context.User
                    .FirstOrDefaultAsync(u => u.UserId == ticket.AssignedToUserId.Value);

                if (assignedUser == null)
                {
                    ModelState.AddModelError("AssignedToUserId", "Selected user not found.");
                    return View(ticket);
                }

                var assignedUserRoleDetails = await _context.Role
                    .FirstOrDefaultAsync(r => r.RoleId == assignedUser.RoleId);

                if (assignedUserRoleDetails == null)
                {
                    ModelState.AddModelError("AssignedToUserId", "User role not found.");
                    return View(ticket);
                }

                // Check if the assigned user has an appropriate role (not SystemAdmin or Stakeholder)
                if (assignedUserRoleDetails.RoleName == RoleNames.SystemAdmin.ToString() || 
                    assignedUserRoleDetails.RoleName == RoleNames.Stakeholder.ToString())
                {
                    ModelState.AddModelError("AssignedToUserId", "Cannot assign tickets to System Administrators or Stakeholders.");
                    return View(ticket);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();

                    // Create notification for assigned user
                    if (ticket.AssignedToUserId.HasValue)
                    {
                        var notification = new Notification
                        {
                            UserId = ticket.AssignedToUserId.Value,
                            Title = "Ticket Assignment",
                            Message = $"Ticket has been assigned/updated: {ticket.Title}",
                            Type = NotificationType.Task,
                            IsRead = false,
                            CreatedAt = DateTime.Now,
                            RelatedEntityId = ticket.Id,
                            RelatedEntityType = "Ticket"
                        };
                        _context.Notification.Add(notification);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            // If we got here, something failed, redisplay form
            ViewBag.ProjectList = new SelectList(await _context.Project.ToListAsync(), "ProjectId", "ProjectName", ticket.ProjectId);
            
            // Get eligible users for assignment (excluding SystemAdmin and Stakeholder)
            var eligibleUsers = await _context.User
                .Include(u => u.Role)
                .Where(u => u.isActive && 
                           u.Role != null &&
                           u.Role.RoleName != RoleNames.SystemAdmin.ToString() && 
                           u.Role.RoleName != RoleNames.Stakeholder.ToString())
                .ToListAsync();

            ViewBag.AssignedToList = new SelectList(eligibleUsers, "UserId", "UserName", ticket.AssignedToUserId);
            return View(ticket);
        }
    }
}

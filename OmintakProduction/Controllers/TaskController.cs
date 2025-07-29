
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Security.Claims;
using System.Linq;
using System;
using System.Collections.Generic;


namespace OmintakProduction.Controllers
{
    // Role-based permissions for Task actions
    [Authorize]
    public class TaskController : Controller
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        // --- AUDIT TRAIL & NOTIFICATION HELPERS ---
        private void LogTaskHistory(int taskId, int userId, TaskHistoryAction action, string? oldValue, string? newValue, string? description)
        {
            var history = new TaskHistory
            {
                TaskId = taskId,
                UserId = userId,
                Action = action,
                OldValue = oldValue,
                NewValue = newValue,
                Description = description,
                CreatedAt = DateTime.Now
            };
            _context.TaskHistory.Add(history);
            _context.SaveChanges();
        }

        private async System.Threading.Tasks.Task NotifyUser(int userId, string title, string message, NotificationType type, int? relatedEntityId, string? relatedEntityType)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.Now,
                RelatedEntityId = relatedEntityId,
                RelatedEntityType = relatedEntityType
            };
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }

        // GET: Task/Manage/{ticketId}
        [Authorize(Roles = "TeamLead,SystemAdmin")]
        public async Task<IActionResult> Manage(int ticketId)
        {
            var ticket = await _context.Ticket.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                return NotFound();
            }
            var tasks = await _context.Tasks
                .Include(t => t.AssignedUsers)
                .Where(t => t.TicketId == ticketId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            ViewBag.Ticket = ticket;
            return View("Manage", tasks);
        }

        // GET: Task
        [Authorize(Roles = "Developer,Tester,TeamLead,SystemAdmin,Stakeholder")]
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                .Include(t => t.CreatedByUser)
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tasks);
        }

        // GET: Task/Details/5
        [Authorize(Roles = "Developer,Tester,TeamLead,SystemAdmin,Stakeholder")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Task/Create
        [Authorize(Roles = "TeamLead,SystemAdmin")]
        public IActionResult Create(int? ticketId)
        {
            // Permission enforcement: Only allow if user has ManageTasks
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole == null || !RolePermissions.HasPermission(userRole, "ManageTasks"))
            {
                return Forbid();
            }
            if (!ticketId.HasValue)
            {
                // Block direct access to /Task/Create without a ticket context
                return RedirectToAction("Index");
            }
            ViewBag.Projects = _context.Project.Include(p => p.Team).ToList();
            ViewBag.Teams = _context.Team.Include(t => t.TeamMembers).ToList();
            ViewBag.Users = _context.User.Where(u => u.isActive).ToList();
            ViewBag.TicketId = ticketId;
            // Get the ticket owner name for display
            var ticket = _context.Ticket.Include(t => t.AssignedToUser).FirstOrDefault(t => t.Id == ticketId.Value);
            if (ticket != null && ticket.AssignedToUser != null)
            {
                ViewBag.TicketOwner = ticket.AssignedToUser.UserName ?? ticket.AssignedToUser.Email ?? $"User #{ticket.AssignedToUser.UserId}";
            }
            else
            {
                ViewBag.TicketOwner = "Unassigned";
            }
            var model = new OmintakProduction.Models.Task { TicketId = ticketId.Value };
            return View(model);
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Developer,Engineer,TeamLead,Tester,SoftwareTester,SystemAdmin")]
        public async Task<IActionResult> Create(OmintakProduction.Models.Task task, int[] AssignedUserIds, int? ticketId = null, int ProjectId = 0)
        {
            // Permission enforcement: Only allow if user has ManageTasks
            var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole == null || !RolePermissions.HasPermission(userRole, "ManageTasks"))
            {
                return Forbid();
            }
            int currentUserId = GetCurrentUserId();
            if (ticketId.HasValue)
            {
                // Creating from ticket context: auto-assign project, skip user assignment
                var ticket = await _context.Ticket.Include(t => t.Project).Include(t => t.AssignedToUser).FirstOrDefaultAsync(t => t.Id == ticketId.Value);
                if (ticket == null)
                {
                    ModelState.AddModelError("TicketId", "Invalid ticket.");
                }
                else
                {
                    task.TicketId = ticket.Id;
                    task.ProjectId = ticket.ProjectId;
                    task.Project = ticket.Project;
                    // ENFORCE: Always assign to ticket owner
                    if (ticket.AssignedToUser == null)
                    {
                        ModelState.AddModelError("AssignedUsers", "Cannot create a task for a ticket with no assigned user. Assign the ticket first.");
                    }
                    else
                    {
                        task.AssignedUsers = new List<User> { ticket.AssignedToUser };
                    }
                }
                if (ModelState.IsValid)
                {
                    // Set CreatedByUserId to the current user
                    task.CreatedByUserId = currentUserId;
                    _context.Tasks.Add(task);
                    await _context.SaveChangesAsync();
                    // Audit log
                    LogTaskHistory(task.Id, currentUserId, TaskHistoryAction.Created, null, null, "Task created");
                    // Notify assignee
                    if (ticket?.AssignedToUser != null)
                    {
                        await NotifyUser(ticket.AssignedToUser.UserId, $"New Task Assigned: {task.Title}", $"You have been assigned a new task.", NotificationType.Task, task.Id, "Task");
                    }
                    TempData["SuccessMessage"] = "Task created successfully.";
                    return RedirectToAction("Manage", new { ticketId = ticketId.Value });
                }
                ViewBag.TicketId = ticketId;
                ViewBag.Projects = _context.Project.Include(p => p.Team).ToList();
                ViewBag.Teams = _context.Team.Include(t => t.TeamMembers).ToList();
                ViewBag.Users = _context.User.Where(u => u.isActive).ToList();
                // Also pass ticket owner for display
                ViewBag.TicketOwner = ticket?.AssignedToUser?.UserName ?? ticket?.AssignedToUser?.Email ?? "Unassigned";
                return View(task);
            }
            else
            {
                // Not from ticket: require project and user assignment as before
                var project = await _context.Project.Include(p => p.Team).FirstOrDefaultAsync(p => p.ProjectId == ProjectId);
                if (ProjectId == 0 || project == null)
                {
                    ModelState.AddModelError("ProjectId", "A valid project must be selected.");
                }
                if (project?.Team == null)
                {
                    ModelState.AddModelError("ProjectId", "Selected project is not assigned to a team.");
                }
                var validUserIds = project?.Team?.TeamMembers?.Where(u => u.isActive).Select(u => u.UserId).ToList() ?? new List<int>();
                if (AssignedUserIds != null && AssignedUserIds.Length > 0)
                {
                    task.AssignedUsers = _context.User.Where(u => AssignedUserIds.Contains(u.UserId) && validUserIds.Contains(u.UserId)).ToList();
                }
                if (ModelState.IsValid)
                {
                    task.ProjectId = ProjectId;
                    task.Project = project;
                    task.CreatedByUserId = currentUserId;
                    _context.Tasks.Add(task);
                    await _context.SaveChangesAsync();
                    // Audit log
                    LogTaskHistory(task.Id, currentUserId, TaskHistoryAction.Created, null, null, "Task created");
                    // Notify all assignees
                    foreach (var user in task.AssignedUsers)
                    {
                        await NotifyUser(user.UserId, $"New Task Assigned: {task.Title}", $"You have been assigned a new task.", NotificationType.Task, task.Id, "Task");
                    }
                    TempData["SuccessMessage"] = "Task created successfully.";
                    return RedirectToAction("Index");
                }
                ViewBag.Projects = _context.Project.Include(p => p.Team).ToList();
                ViewBag.Teams = _context.Team.Include(t => t.TeamMembers).ToList();
                ViewBag.Users = _context.User.Where(u => u.isActive).ToList();
                return View(task);
            }
        }

        // GET: Task/Edit/5
        [Authorize(Roles = "Developer,Tester,TeamLead,SystemAdmin,Stakeholder")]
        public async Task<IActionResult> Edit(int? id, int? ticketId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null || task.IsDeleted)
            {
                return NotFound();
            }
            ViewBag.Projects = _context.Project.ToList();
            ViewBag.Users = _context.User.ToList();
            ViewBag.TicketId = ticketId;
            return View(task);
        }

        // POST: Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Developer,Tester,TeamLead,SystemAdmin,Stakeholder")]
        public async System.Threading.Tasks.Task<IActionResult> Edit(OmintakProduction.Models.Task model, int[] AssignedUserIds)
        {
            // ENFORCE: Task must always have an assignee
            if ((AssignedUserIds == null || AssignedUserIds.Length == 0) && model.TicketId.HasValue)
            {
                // Try to assign to ticket owner if possible
                var ticket = await _context.Ticket.Include(t => t.AssignedToUser).FirstOrDefaultAsync(t => t.Id == model.TicketId.Value);
                if (ticket?.AssignedToUser != null)
                {
                    AssignedUserIds = new int[] { ticket.AssignedToUser.UserId };
                }
                else
                {
                    ModelState.AddModelError("AssignedUsers", "A task must have at least one assignee. Assign the ticket first.");
                }
            }
            if (ModelState.IsValid)
            {
            var task = await _context.Tasks.Include(t => t.AssignedUsers).FirstOrDefaultAsync(t => t.Id == model.Id);
                if (task == null) return NotFound();
                int currentUserId = GetCurrentUserId();
                // Audit: Track changes
                if (task.Status != model.Status)
                    LogTaskHistory(task.Id, currentUserId, TaskHistoryAction.StatusChanged, task.Status.ToString(), model.Status.ToString(), "Status changed");
                if (task.DueDate != model.DueDate)
                    LogTaskHistory(task.Id, currentUserId, TaskHistoryAction.DueDateChanged, task.DueDate?.ToString(), model.DueDate?.ToString(), "Due date changed");
                if (task.Priority != model.Priority)
                    LogTaskHistory(task.Id, currentUserId, TaskHistoryAction.PriorityChanged, task.Priority.ToString(), model.Priority.ToString(), "Priority changed");
                if (task.ProjectId != model.ProjectId)
                    LogTaskHistory(task.Id, currentUserId, TaskHistoryAction.Updated, task.ProjectId?.ToString(), model.ProjectId?.ToString(), "Project changed");
                // Assignment changes
                var oldAssignees = task.AssignedUsers.Select(u => u.UserId).OrderBy(x => x).ToList();
                var newAssignees = (AssignedUserIds ?? new int[0]).OrderBy(x => x).ToList();
                if (!oldAssignees.SequenceEqual(newAssignees))
                {
                    LogTaskHistory(task.Id, currentUserId, TaskHistoryAction.AssigneeChanged, string.Join(",", oldAssignees), string.Join(",", newAssignees), "Assignees changed");
                    // Notify new assignees
                    foreach (var userId in newAssignees.Except(oldAssignees))
                        await NotifyUser(userId, $"New Task Assigned: {model.Title}", $"You have been assigned a new task.", NotificationType.Task, task.Id, "Task");
                }
                // Update properties
                task.Title = model.Title;
                task.Description = model.Description;
                task.Status = model.Status;
                task.DueDate = model.DueDate;
                task.ProjectId = model.ProjectId;
                // Update assigned users
                task.AssignedUsers.Clear();
                var safeAssignedUserIds = AssignedUserIds ?? new int[0];
                var users = _context.User.Where(u => safeAssignedUserIds.Contains(u.UserId)).ToList();
                foreach (var user in users)
                {
                    task.AssignedUsers.Add(user);
                }
                await _context.SaveChangesAsync();
                if (model.TicketId.HasValue)
                {
                    return RedirectToAction("Manage", new { ticketId = model.TicketId.Value });
                }
                return RedirectToAction("Index");
            }
            ViewBag.Projects = _context.Project.ToList();
            if (model.Project != null && model.Project.Team != null)
            {
                ViewBag.AssignedUsers = model.Project.Team.TeamMembers;
            }
            else
            {
                ViewBag.AssignedUsers = new List<User>();
            }
            ViewBag.TicketId = model.TicketId;
            return View(model);
        }

        // GET: Task/Delete/5
        [Authorize(Roles = "TeamLead,SystemAdmin")]
        public async Task<IActionResult> Delete(int? id, int? ticketId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (task == null)
            {
                return NotFound();
            }
            ViewBag.TicketId = ticketId;
            return View(task);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TeamLead,SystemAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            int? ticketId = null;
            if (task != null)
            {
                ticketId = task.TicketId;
                task.IsDeleted = true;
                _context.Update(task);
                await _context.SaveChangesAsync();
                // Audit log
                LogTaskHistory(task.Id, GetCurrentUserId(), TaskHistoryAction.Deleted, null, null, "Task deleted");
            }
            if (ticketId.HasValue)
            {
                return RedirectToAction("Manage", new { ticketId = ticketId.Value });
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id && !e.IsDeleted);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}

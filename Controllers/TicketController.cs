
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Security.Claims;

namespace OmintakProduction.Controllers
{
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;
        public TicketController(AppDbContext context)
        {
            _context = context;
        }

        // POST: Ticket/AssignTicket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTicket(int id, int? assignedToUserId)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ticket.AssignedToUserId = assignedToUserId;
            _context.Update(ticket);
            await _context.SaveChangesAsync();
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
            var tickets = await _context.Ticket.ToListAsync();
            return View("~/Views/Ticket/Index.cshtml", tickets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Ticket
                .Include(t => t.Tasks)
                .Include(t => t.AssignedToUser)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Ticket
                .FirstOrDefaultAsync(t => t.Id == id);

            return ticket == null ? NotFound() : View("~/Views/Ticket/Delete.cshtml", ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Ticket obj)
        {
            _context.Ticket.Remove(obj);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }

        public IActionResult Edit(int id)
        {
            var ticket = _context.Ticket.Include(t => t.Project).Include(t => t.AssignedToUser).FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewBag.ProjectList = new SelectList(_context.Project.ToList(), "ProjectId", "ProjectName");
            List<User> assignedUsers = new List<User>();
            if (ticket.ProjectId.HasValue)
            {
                var project = _context.Project.Include(p => p.Team).ThenInclude(team => team.TeamMembers)
                    .FirstOrDefault(p => p.ProjectId == ticket.ProjectId.Value);
                if (project != null && project.Team != null && project.Team.TeamMembers != null)
                {
                    assignedUsers = project.Team.TeamMembers.Where(u => u != null && u.isActive).ToList();
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
            if (ModelState.IsValid)
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ProjectList = new SelectList(_context.Project.ToList(), "ProjectId", "ProjectName");
            List<User> assignedUsers = new List<User>();
            if (ticket.ProjectId.HasValue)
            {
                var project = _context.Project.Include(p => p.Team).ThenInclude(team => team.TeamMembers)
                    .FirstOrDefault(p => p.ProjectId == ticket.ProjectId.Value);
                if (project != null && project.Team != null && project.Team.TeamMembers != null)
                {
                    assignedUsers = project.Team.TeamMembers.Where(u => u != null && u.isActive).ToList();
                }
            }
            ViewBag.AssignedToList = new SelectList(assignedUsers, "UserId", "UserName");
            return View(ticket);
        }
    }
}

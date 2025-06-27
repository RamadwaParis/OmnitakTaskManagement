using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Ticket.ToListAsync();

            return View("~/Views/Ticket/Index.cshtml", tickets);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate,Status")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

                _context.Add(ticket);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Ticket/Create.cshtml", ticket);
        }


        public IActionResult Create()
        {
            return View();
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


    }
}
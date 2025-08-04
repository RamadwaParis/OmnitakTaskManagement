using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace OmintakProduction.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class DragDropApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DragDropApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("updateItemStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateItemStatus([FromBody] UpdateItemRequest request)
        {
            try
            {
                switch (request.Type.ToLower())
                {
                    case "ticket":
                        return await UpdateTicketStatus(request);
                    case "task":
                        return await UpdateTaskStatus(request);
                    case "project":
                        return await UpdateProjectStatus(request);
                    default:
                        return BadRequest(new { success = false, message = "Invalid item type" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private async Task<IActionResult> UpdateTicketStatus(UpdateItemRequest request)
        {
            var ticket = await _context.Ticket.FindAsync(request.Id);
            if (ticket == null)
            {
                return NotFound(new { success = false, message = "Ticket not found" });
            }

            // Update status if provided
            if (!string.IsNullOrEmpty(request.NewStatus))
            {
                ticket.Status = request.NewStatus;
            }

            // Update priority if provided
            if (!string.IsNullOrEmpty(request.NewPriority) && 
                Enum.TryParse<TaskPriority>(request.NewPriority, out var priority))
            {
                ticket.Priority = priority;
            }

            _context.Update(ticket);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Ticket updated successfully" });
        }

        private async Task<IActionResult> UpdateTaskStatus(UpdateItemRequest request)
        {
            var task = await _context.Tasks.FindAsync(request.Id);
            if (task == null)
            {
                return NotFound(new { success = false, message = "Task not found" });
            }

            // Update status if provided
            if (!string.IsNullOrEmpty(request.NewStatus) && 
                Enum.TryParse<Models.TaskStatus>(request.NewStatus.Replace(" ", ""), out var status))
            {
                task.Status = status;
            }

            // Update priority if provided
            if (!string.IsNullOrEmpty(request.NewPriority) && 
                Enum.TryParse<TaskPriority>(request.NewPriority, out var priority))
            {
                task.Priority = priority;
            }

            _context.Update(task);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Task updated successfully" });
        }

        private async Task<IActionResult> UpdateProjectStatus(UpdateItemRequest request)
        {
            var project = await _context.Project.FindAsync(request.Id);
            if (project == null)
            {
                return NotFound(new { success = false, message = "Project not found" });
            }

            // Update status if provided
            if (!string.IsNullOrEmpty(request.NewStatus))
            {
                project.Status = request.NewStatus;
            }

            _context.Update(project);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Project updated successfully" });
        }
    }

    public class UpdateItemRequest
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? NewStatus { get; set; }
        public string? NewPriority { get; set; }
    }
}

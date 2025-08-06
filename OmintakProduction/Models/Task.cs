using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OmintakProduction.Validations;

namespace OmintakProduction.Models
{
    public enum TaskStatus
    {
        Todo,
        InProgress,
        InReview,
        Completed,
        Cancelled,
        OnHold
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical,
        Urgent
    }

    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public int CreatedByUserId { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [NotPastDate(AllowToday = true)]
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Tags { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }
        public User? DeletedByUser { get; set; }

        // Hold feature properties
        public bool IsOnHold { get; set; } = false;
        public DateTime? HoldStartDate { get; set; }
        public string? HoldReason { get; set; }
        public int? HoldRequestedByUserId { get; set; }
        public User? HoldRequestedByUser { get; set; }

        // Navigation properties
        public Project? Project { get; set; }
        public List<User> AssignedUsers { get; set; } = new List<User>();
        public User? CreatedByUser { get; set; }
        public int? TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        // Methods for task hold management
        public void PutOnHold(string reason, int requestedByUserId)
        {
            IsOnHold = true;
            HoldStartDate = DateTime.UtcNow;
            HoldReason = reason;
            HoldRequestedByUserId = requestedByUserId;
            Status = TaskStatus.OnHold;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ResumeTask()
        {
            IsOnHold = false;
            HoldStartDate = null;
            HoldReason = null;
            HoldRequestedByUserId = null;
            Status = TaskStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

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
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Tags { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Project? Project { get; set; }
        public List<User> AssignedUsers { get; set; } = new List<User>();
        public User? CreatedByUser { get; set; }
        public int? TicketId { get; set; }
        public Ticket? Ticket { get; set; }
    }
}

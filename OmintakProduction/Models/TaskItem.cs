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

    public class TaskItem
    {
        public int TaskItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public int? AssignedToUserId { get; set; }
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
        public User? AssignedToUser { get; set; }
        public User? CreatedByUser { get; set; }
    }
}

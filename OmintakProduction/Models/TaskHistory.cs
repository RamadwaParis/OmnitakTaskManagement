namespace OmintakProduction.Models
{
    public enum TaskHistoryAction
    {
        Created,
        Updated,
        StatusChanged,
        AssigneeChanged,
        PriorityChanged,
        DueDateChanged,
        CommentAdded,
        Deleted,
        Restored
    }

    public class TaskHistory
    {
        public int TaskHistoryId { get; set; }
        public int TaskItemId { get; set; }
        public int UserId { get; set; }
        public TaskHistoryAction Action { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public TaskItem? TaskItem { get; set; }
        public User? User { get; set; }
    }
}

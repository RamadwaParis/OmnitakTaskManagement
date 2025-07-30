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
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public TaskHistoryAction Action { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Task Task { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}

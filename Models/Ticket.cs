namespace OmintakProduction.Models
{
    enum TaskStatuses
    {
        ToDo,
        In_Progress,
        In_Review,
        Done
    }

    public class Ticket
    {


        public int Id { get; set; }
        public int? ProjectId { get; set; } = null;
        public int? AssignedToUserId { get; set; } = null;
        public User? AssignedToUser { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? CompletedAt { get; set; }
        public string Status { get; set; } = string.Empty; 
        public List<Task> Tasks { get; set; } = new List<Task>();
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public Project? Project { get; set; }
    }
}

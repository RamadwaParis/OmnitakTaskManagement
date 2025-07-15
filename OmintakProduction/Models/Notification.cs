namespace OmintakProduction.Models
{
    public enum NotificationType
    {
        Info,
        Warning,
        Success,
        Error,
        Task,
        Project,
        User
    }

    public class Notification
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.Info;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ReadAt { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; } // "Project", "Ticket", "User", etc.
        public string? ActionUrl { get; set; }
        
        // Navigation property
        public User? User { get; set; }
    }
}

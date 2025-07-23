namespace OmintakProduction.Models
{
    public class TaskComment
    {
        public int TaskCommentId { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public required string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsEdited { get; set; } = false;


    }
}

namespace OmintakProduction.Models
{
    public class TaskAssignment
    {
        public int Id { get; set; } // Primary Key (for views)
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string? Status { get; set; }
        public DateTime DueDate { get; set; }

        // Navigation properties
        public Project? Project { get; set; }
        public User? User { get; set; }
    }
}

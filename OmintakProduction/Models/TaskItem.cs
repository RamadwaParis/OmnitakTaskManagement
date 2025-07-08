namespace OmintakProduction.Models
{
    public class TaskItem
    {
        public int TaskItemId { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedToUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } //= "Open"; // Default status
        public DateOnly CreatedDate { get; set; } //= DateTime.Now;
    }
}

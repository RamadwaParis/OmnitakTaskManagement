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
        public string Title { get; set; } 
        public string Description { get; set; }
        public DateOnly DueDate { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? CompletedAt { get; set; }
        public string Status { get; set; } 
    }
}
 
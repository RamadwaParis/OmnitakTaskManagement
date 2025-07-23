namespace OmintakProduction.Models
{
    enum ProjectStatuses
    {
        Active = 1,
        OnHold,
        Done,
    }
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Status { get; set; } = "Active";
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}

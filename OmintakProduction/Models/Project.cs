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
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateOnly DueDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}

namespace OmintakProduction.Models
{
    public class ProjectReport
    {
        public int ProjectReportId { get; set; }
        public int ProjectId { get; set; }
        public string? ReportDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        // Navigation property
        public Project? Project { get; set; }
    }
}

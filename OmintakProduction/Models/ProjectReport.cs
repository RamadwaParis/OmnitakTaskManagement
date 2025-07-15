using System;

namespace OmintakProduction.Models
{
    public class ProjectReport
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? ReportDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        // Navigation property
        public Project? Project { get; set; }
    }
}

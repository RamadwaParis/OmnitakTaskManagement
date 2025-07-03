namespace OmintakProduction.Models
{
    public class ProjectReport
    {
        public int ProjectReportId { get; set; }
        public int ProjectId { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime ReportDate { get; set; }
        public string Details { get; set; }
    }
}

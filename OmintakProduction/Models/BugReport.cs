namespace OmintakProduction.Models
{
    public class BugReport
    {
        public int BugReportId { get; set; }
        public int TaskId { get; set; }
        public string ReportedBy { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateOnly ReportDate { get; set; }
    }
}

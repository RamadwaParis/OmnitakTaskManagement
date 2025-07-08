namespace OmintakProduction.Models
{
    public class TestReport
    {
        public int TestReportId { get; set; }
        public int ReportId { get; set; }
        public int BugId { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime ReportDate { get; set; }
        public string Details { get; set; }
    }
}

namespace OmintakProduction.Models
{
    public class TestReport
    {
        public int TestReportId { get; set; }
        public int ProjectId { get; set; }
        public string? TestSummary { get; set; }
        public DateTime CreatedAt{ get; set; }
        // Navigation property
        public Project? Project { get; set; }
    }
}

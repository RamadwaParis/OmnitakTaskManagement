namespace OmintakProduction.Models
{
    public class TaskAssignment
    {
        public int ProjectReportId { get; set; }
        public int ProjectId { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public DateTime ReportDate { get; set; }
        public required string Status { get; set; }
    }
}

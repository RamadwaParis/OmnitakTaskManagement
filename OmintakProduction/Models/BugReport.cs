namespace OmintakProduction.Models
{
    public enum BugSeverity
    {
        Low,
        Medium,
        High,
        Critical,
        Blocker
    }

    public enum BugStatus
    {
        Open,
        InProgress,
        Testing,
        Resolved,
        Closed,
        Reopened
    }

    public class BugReport
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StepsToReproduce { get; set; } = string.Empty;
        public string ExpectedBehavior { get; set; } = string.Empty;
        public string ActualBehavior { get; set; } = string.Empty;
        public BugSeverity Severity { get; set; } = BugSeverity.Medium;
        public BugStatus Status { get; set; } = BugStatus.Open;
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public int ReportedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? ScreenshotPath { get; set; }
        public string? BrowserInfo { get; set; }
        public string? OperatingSystem { get; set; }
        public string? Version { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Project? Project { get; set; }
        public Task? Task { get; set; }
        public User? ReportedByUser { get; set; }
        public User? AssignedToUser { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmintakProduction.ViewModels
{
    using TaskModel = OmintakProduction.Models.Task;
    using UserModel = OmintakProduction.Models.User;
    using ProjectModel = OmintakProduction.Models.Project;
    using TeamModel = OmintakProduction.Models.Team;
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTeams { get; set; }
        public required SystemMetrics SystemMetrics { get; set; }
        public List<ActivityLog> RecentActivities { get; set; } = new();
    }

    public class TeamLeadDashboardViewModel
    {
        public List<TeamModel> ManagedTeams { get; set; } = new();
        public List<ProjectModel> TeamProjects { get; set; } = new();
        public List<UserModel> TeamMembers { get; set; } = new();
        public List<TaskModel> OnHoldTasks { get; set; } = new();
        public required ProjectMetrics ProjectMetrics { get; set; }

        public TeamLeadDashboardViewModel()
        {
            ProjectMetrics = new ProjectMetrics();
        }
    }

    public class DeveloperDashboardViewModel
    {
        public List<TaskModel> AssignedTasks { get; set; } = new();
        public ProjectModel? CurrentProject { get; set; }
        public List<TaskModel> TeamTasks { get; set; } = new();
        public required DeveloperMetrics PersonalMetrics { get; set; }

        public DeveloperDashboardViewModel()
        {
            PersonalMetrics = new DeveloperMetrics();
        }
    }

    public class TesterDashboardViewModel
    {
        public List<TaskModel> TestTasks { get; set; } = new();
        public List<Models.BugReport> BugReports { get; set; } = new();
        public required TestMetrics TestMetrics { get; set; }
        public List<QualityMetric> ProjectQualityMetrics { get; set; } = new();

        public TesterDashboardViewModel()
        {
            TestMetrics = new TestMetrics();
        }
    }

    public class StakeholderDashboardViewModel
    {
        public List<ProjectProgress> ProjectProgress { get; set; } = new();
        public List<Milestone> Milestones { get; set; } = new();
        public required HighLevelMetrics HighLevelMetrics { get; set; }
        public List<ProjectTimeline> ProjectTimelines { get; set; } = new();

        public StakeholderDashboardViewModel()
        {
            HighLevelMetrics = new HighLevelMetrics();
        }
    }

    public class SystemMetrics
    {
        public double SystemUptime { get; set; }
        public int ActiveUsers { get; set; }
        public int PendingTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double SystemLoad { get; set; }
    }

    public class ProjectMetrics
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OnHoldTasks { get; set; }
        public double Progress { get; set; }
        public int TeamMembers { get; set; }
        public List<TaskMetric> TaskBreakdown { get; set; } = new();
    }

    public class DeveloperMetrics
    {
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public double AverageCompletionTime { get; set; }
        public List<TaskEfficiency> TaskEfficiencyMetrics { get; set; } = new();
    }

    public class TestMetrics
    {
        public int TotalTestCases { get; set; }
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
        public int PendingTests { get; set; }
        public double TestCoverage { get; set; }
    }

    public class QualityMetric
    {
        public string MetricName { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class ProjectProgress
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public double CompletionPercentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime EstimatedCompletion { get; set; }
    }

    public class Milestone
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<TaskModel> AssociatedTasks { get; set; } = new();
    }

    public class HighLevelMetrics
    {
        public int TotalProjects { get; set; }
        public double OverallProgress { get; set; }
        public int DelayedProjects { get; set; }
        public double BudgetUtilization { get; set; }
        public List<RiskMetric> ProjectRisks { get; set; } = new();
    }

    public class ProjectTimeline
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<TimelineEvent> Events { get; set; } = new();
    }

    public class TaskMetric
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public double PercentageOfTotal { get; set; }
    }

    public class TaskEfficiency
    {
        public string TaskType { get; set; } = string.Empty;
        public double AverageCompletionTime { get; set; }
        public int TaskCount { get; set; }
    }

    public class RiskMetric
    {
        public string RiskType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string MitigationStatus { get; set; } = string.Empty;
    }

    public class TimelineEvent
    {
        public string EventName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class ActivityLog
    {
        public int Id { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}

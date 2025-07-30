using System.Collections.Generic;
using System.Linq;
using OmintakProduction.Models;

namespace OmintakProduction.Models
{
    // Developer Dashboard ViewModel
    public class DeveloperDashboardViewModel
    {
        public List<Task> AssignedTasks { get; set; } = new List<Task>();
        public List<Task> MyActiveTasks { get; set; } = new List<Task>();
        public List<Task> MyCompletedTasks { get; set; } = new List<Task>();
        public List<Task> MyOverdueTasks { get; set; } = new List<Task>();
        public List<Task> TaskDependencies { get; set; } = new List<Task>();
        public List<TaskComment> RecentComments { get; set; } = new List<TaskComment>();
        public Dictionary<string, int> TasksByPriority { get; set; } = new Dictionary<string, int>();
        public List<Notification> RecentNotifications { get; set; } = new List<Notification>();
        public int UnreadNotificationCount { get; set; }
        
        // Computed properties for dashboard statistics
        public List<Task> RecentTasks => AssignedTasks;
        public int ActiveTasks => MyActiveTasks.Count;
        public int CompletedTasks => MyCompletedTasks.Count;
        public int OverdueTasks => MyOverdueTasks.Count;
        public int TotalAssignedTasks => AssignedTasks.Count;
    }

    // Tester Dashboard ViewModel
    public class TesterDashboardViewModel
    {
        public List<Task> CompletedTasksForTesting { get; set; } = new List<Task>();
        public List<Task> PendingTestTasks { get; set; } = new List<Task>();
        public List<BugReport> MyBugReports { get; set; } = new List<BugReport>();
        public List<BugReport> ActiveBugReports { get; set; } = new List<BugReport>();
        public List<BugReport> ResolvedBugReports { get; set; } = new List<BugReport>();
        public Dictionary<string, int> BugReportsByPriority { get; set; } = new Dictionary<string, int>();
        public List<Notification> RecentNotifications { get; set; } = new List<Notification>();
        public int UnreadNotificationCount { get; set; }
        
        // Computed properties for dashboard
        public List<Task> RecentTasks => CompletedTasksForTesting.Concat(PendingTestTasks).OrderByDescending(t => t.UpdatedAt).ToList();
        public int CompletedTasks => CompletedTasksForTesting.Count;
        public int ActiveBugReportsCount => ActiveBugReports.Count;
        public int ResolvedBugReportsCount => ResolvedBugReports.Count;
    }

    // Team Lead Dashboard ViewModel
    public class TeamLeadDashboardViewModel
    {
        public List<Project> ManagedProjects { get; set; } = new List<Project>();
        public List<User> TeamMembers { get; set; } = new List<User>();
        public List<Task> AllTasks { get; set; } = new List<Task>();
        public Dictionary<string, object> TeamWorkload { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> ProjectProgress { get; set; } = new Dictionary<string, object>();
        public List<Task> OverdueTasks { get; set; } = new List<Task>();
        public Dictionary<string, int> TasksByStatus { get; set; } = new Dictionary<string, int>();
        public List<Notification> RecentNotifications { get; set; } = new List<Notification>();
        public int UnreadNotificationCount { get; set; }
        
        // Computed properties for dashboard
        public int TotalProjects => ManagedProjects.Count;
        public int TeamCount => TeamMembers.Count;
        public int CompletedTasks => AllTasks.Count(t => t.Status == TaskStatus.Completed);
        public int ActiveTasks => AllTasks.Count(t => t.Status == TaskStatus.InProgress || t.Status == TaskStatus.Todo);
        public List<Project> RecentProjects => ManagedProjects.OrderByDescending(p => p.StartDate ?? DateOnly.MinValue).ToList();
        public List<Team> Teams => ManagedProjects.Where(p => p.Team != null).Select(p => p.Team!).Distinct().ToList();
    }

    // System Admin Dashboard ViewModel
    public class SystemAdminDashboardViewModel
    {
        public List<User> AllUsers { get; set; } = new List<User>();
        public List<User> PendingUsers { get; set; } = new List<User>();
        public List<Project> AllProjects { get; set; } = new List<Project>();
        public List<Task> AllTasks { get; set; } = new List<Task>();
        public List<Ticket> AllTickets { get; set; } = new List<Ticket>();
        public List<TaskHistory> SystemLogs { get; set; } = new List<TaskHistory>();
        public Dictionary<string, int> UsersByRole { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, object> SystemStatistics { get; set; } = new Dictionary<string, object>();
        public List<Notification> RecentNotifications { get; set; } = new List<Notification>();
        public int UnreadNotificationCount { get; set; }
    }

    // Stakeholder Dashboard ViewModel
    public class StakeholderDashboardViewModel
    {
        public List<Project> AllProjects { get; set; } = new List<Project>();
        public List<Task> AllTasks { get; set; } = new List<Task>();
        public List<Task> CompletedTasks { get; set; } = new List<Task>();
        public List<Task> HighPriorityTasks { get; set; } = new List<Task>();
        public Dictionary<string, object> ProjectProgress { get; set; } = new Dictionary<string, object>();
        public List<ProjectReport> ProjectReports { get; set; } = new List<ProjectReport>();
        public Dictionary<string, object> HighLevelMetrics { get; set; } = new Dictionary<string, object>();
        public List<Notification> RecentNotifications { get; set; } = new List<Notification>();
        public int UnreadNotificationCount { get; set; }
        public decimal TotalBudget { get; set; }
        public int TeamCount { get; set; }
    }
}

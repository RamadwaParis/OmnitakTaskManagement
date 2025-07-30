using System.Collections.Generic;

namespace OmintakProduction.Models
{
    public static class RolePermissions
    {
        public static readonly Dictionary<string, List<string>> Permissions = new()
        {
            // System Admin: full access with system-wide dashboard
            ["SystemAdmin"] = new List<string>
            {
                "ViewAdminDashboard", "ManageUsers", "ManageTasks", "ManageProjects", "ManageReports", 
                "ViewLogs", "ManageSettings", "ViewAllData", "ManageRoles", "ViewSystemMetrics",
                "ConfigureSystem", "ManageAllTeams", "ViewSystemStats"
            },
            // Team Lead: project and team management focused dashboard
            ["TeamLead"] = new List<string>
            {
                "ViewTeamLeadDashboard", "ManageTeamProjects", "AssignProjectToTeam", "ViewTeamWorkload", 
                "TrackTeamProgress", "GenerateTeamReports", "ManageTeamTasks", "ViewTeamData", 
                "AssignTickets", "ManageTickets", "PutTasksOnHold", "ManageProjectTimelines",
                "AssignTasksToTeam", "ViewTeamPerformance", "ManageTeamMembers", "SetTaskPriorities",
                "ViewProjectMetrics"
            },
            // Developer: task-focused dashboard with project visibility
            ["Developer"] = new List<string>
            {
                "ViewDeveloperDashboard", "ViewAssignedTasks", "UpdateTaskStatus", "AddTaskComment", 
                "ViewTaskDependencies", "ReceiveNotifications", "ViewTeamProjects", "ReportTaskBlocks",
                "ViewProjectTimeline", "UpdateWorkProgress", "ViewTeamTasks", "CollaborateOnTasks",
                "ViewPersonalMetrics"
            },
            // Tester: testing and quality focused dashboard
            ["Tester"] = new List<string>
            {
                "ViewTesterDashboard", "ViewTestTasks", "CreateBugReport", "LinkBugToTask", "VerifyBugFix",
                "ViewTestCases", "UpdateTestStatus", "ViewProjectQuality", "TrackBugStatus",
                "ViewTestMetrics", "AssignBugPriority", "ViewTestProgress"
            },
            // Stakeholder: high-level project overview dashboard
            ["Stakeholder"] = new List<string>
            {
                "ViewStakeholderDashboard", "ViewProjectReports", "ViewProjectProgress", "ViewMilestones",
                "ViewProjectMetrics", "AccessProjectTimeline", "ViewHighLevelStatus", "ViewProjectSummary"
            }
        };

        public static bool HasPermission(string role, string permission)
        {
            return Permissions.ContainsKey(role) && Permissions[role].Contains(permission);
        }
    }
}

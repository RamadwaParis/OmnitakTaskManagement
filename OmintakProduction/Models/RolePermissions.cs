using System.Collections.Generic;

namespace OmintakProduction.Models
{
    public static class RolePermissions
    {
        public static readonly Dictionary<string, List<string>> Permissions = new()
        {
            // System Admin: full access
            ["SystemAdmin"] = new List<string>
            {
                "ViewDashboard", "ManageUsers", "ManageTasks", "ManageProjects", "ManageReports", "ViewLogs", "ManageSettings", "ViewAllData"
            },
            // Project Lead: manage team, tasks, and reports
            ["ProjectLead"] = new List<string>
            {
                "ViewDashboard", "AssignTasks", "ViewTeamWorkload", "TrackProgress", "GenerateReports", "ManageTasks", "ViewAllData"
            },
            // Developer: work on assigned tasks
            ["Developer"] = new List<string>
            {
                "ViewDashboard", "ViewAssignedTasks", "UpdateTaskStatus", "AddTaskComment", "ViewTaskDependencies", "ReceiveNotifications"
            },
            ["Engineer"] = new List<string>
            {
                "ViewDashboard", "ViewAssignedTasks", "UpdateTaskStatus", "AddTaskComment", "ViewTaskDependencies", "ReceiveNotifications"
            },
            // Tester: test tasks and report bugs
            ["Tester"] = new List<string>
            {
                "ViewDashboard", "ViewCompletedTasks", "CreateBugReport", "LinkBugToTask", "VerifyBugFix"
            },
            ["SoftwareTester"] = new List<string>
            {
                "ViewDashboard", "ViewCompletedTasks", "CreateBugReport", "LinkBugToTask", "VerifyBugFix"
            },
            // Stakeholder: view only
            ["Stakeholder"] = new List<string>
            {
                "ViewDashboard", "ViewReports", "ViewProjectProgress"
            }
        };

        public static bool HasPermission(string role, string permission)
        {
            return Permissions.ContainsKey(role) && Permissions[role].Contains(permission);
        }
    }
}

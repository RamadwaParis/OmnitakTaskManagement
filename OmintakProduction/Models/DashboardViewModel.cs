using System.Collections.Generic;
using OmintakProduction.Models;

namespace OmintakProduction.Models
{
    public class DashboardViewModel
    {
        public int ActiveTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int ActiveTickets { get; set; } // <--- Added for dashboard
        public int TeamCount { get; set; }
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Task> RecentTasks { get; set; } = new List<Task>();
        public List<Project> RecentProjects { get; set; } = new List<Project>();
        public List<Notification> RecentNotifications { get; set; } = new List<Notification>();
        public int UnreadNotificationCount { get; set; }
        public List<object> TasksByStatus { get; set; } = new List<object>();
        public List<Ticket> ActiveTicketsList { get; set; } = new List<Ticket>();
    }
}

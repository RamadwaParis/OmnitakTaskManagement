using System.Collections.Generic;

namespace OmintakProduction.Models
{
    public class UnifiedDashboardViewModel
    {
        public List<Task> Tasks { get; set; } = new List<Task>();
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<User> Users { get; set; } = new List<User>();
        public List<string> UrgentItems { get; set; } = new List<string>();
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Team> Teams { get; set; } = new List<Team>();
    }
}

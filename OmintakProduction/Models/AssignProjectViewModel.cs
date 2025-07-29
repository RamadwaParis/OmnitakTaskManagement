using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OmintakProduction.Models
{
    public class AssignProjectViewModel
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int TeamId { get; set; }

        public string ProjectName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public List<Project> AvailableProjects { get; set; } = new List<Project>();
        public List<Team> ManagedTeams { get; set; } = new List<Team>();
    }

    public class ProjectAssignmentRequest
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int TeamId { get; set; }

        public string? Notes { get; set; }
    }
}

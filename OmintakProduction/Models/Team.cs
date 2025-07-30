using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmintakProduction.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        public string TeamName { get; set; } = string.Empty;

        [ForeignKey("TeamLead")]
        public int? TeamLeadId { get; set; }
        public virtual User? TeamLead { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual List<User> TeamMembers { get; set; } = new List<User>();

        public void AssignTeamLead(User user)
        {
            if (user.Role?.RoleName == "TeamLead")
            {
                TeamLeadId = user.UserId;
                TeamLead = user;
            }
        }
    }
}

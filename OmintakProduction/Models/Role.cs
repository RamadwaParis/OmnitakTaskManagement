namespace OmintakProduction.Models
{

    public enum RoleNames
    {
        SystemAdmin = 1,
        Developer = 2,
        Tester = 3,
        Stakeholder = 4,
        TeamLead = 5
    }

    public class Role
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public string getRole(int id)
        {
            return ((RoleNames)id).ToString();
        }
    }


}

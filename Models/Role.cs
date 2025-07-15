namespace OmintakProduction.Models
{
    enum RoleNames
    {
        SystemAdmin=1,
        Engineer,
        SoftwareTester
    }

    public class Role
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public string? RoleName { get; set; }

        public string getRole(int id)
        {
            return ((RoleNames)id).ToString();
        }
    }


}

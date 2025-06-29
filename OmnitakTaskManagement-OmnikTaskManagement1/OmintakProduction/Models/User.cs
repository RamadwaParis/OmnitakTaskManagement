namespace OmintakProduction.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;
        public DateOnly CreatedDate { get; set; }
    }
}

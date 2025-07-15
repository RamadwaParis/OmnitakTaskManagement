namespace OmintakProduction.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public required string Password { get; set; } = string.Empty;
        public bool isActive { get; set; } = false; // default to false on registration, will set to true when user approves account
        public bool IsApproved { get; set; } = false; // for admin approval
        public bool NeedsWelcome { get; set; } = true;
        public DateOnly CreatedDate { get; set; }
        public int? ProjectId { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}

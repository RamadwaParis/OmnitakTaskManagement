namespace OmintakProduction.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateOnly DateSent { get; set; }
        public bool IsRead { get; set; }
    }
}

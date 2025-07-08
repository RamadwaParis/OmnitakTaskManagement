namespace OmintakProduction.Models
{
    public class TaskHistory
    {
        public int TaskHistoryId { get; set; }
        public int TaskId { get; set; }
        public int ChangedByUserId { get; set; }
        public string FieldChanged { get; set; }
        public string OldValue { get; set; }
        public int NewValue { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.Now;
        public string ChangeType { get; set; }
    }
}

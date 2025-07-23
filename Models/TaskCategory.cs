using System.Collections.Generic;

namespace OmintakProduction.Models
{
    public class TaskCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}

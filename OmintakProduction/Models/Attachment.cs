namespace OmintakProduction.Models
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public int TaskId { get; set; }
        public int UploadedByUserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; } // e.g., "image/png", "application/pdf"
        public int FileSize { get; set; }
        public DateTime UploadedDate { get; set; } = DateTime.Now;
    }
}

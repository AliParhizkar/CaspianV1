namespace Caspian.UI
{
    public class FileUploadData
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string FileId { get; set; }

        public FileUploadStatus Status { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class ProgressData
    {
        public string FileName { get; set; }

        public int Value { get; set; }

        public int Max { get; set; }

        public bool IsFailed { get; set; }
    }

    public enum FileUploadStatus
    {
        InProgress,

        Completed,

        Failed
    }
}

namespace Malama.Models
{
    public class UploadLabFilesResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> UploadedFiles { get; set; } = new();
    }
}

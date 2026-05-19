using Microsoft.AspNetCore.Http;

namespace Malama.Models
{
    public class LabFileUpdatePlan
    {
        public List<(IFormFile File, string Prefix)> FilesToUpload { get; set; } = new();
        public List<(string Prefix, string ExistingFileName)> FilesToDelete { get; set; } = new();
        public List<(string Prefix, string ExistingFileName)> FilesToKeep { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}

using Microsoft.AspNetCore.Http;

namespace Malama.Models
{
    public sealed class DentalXRayFileUpdatePlan
    {
        public List<DentalXRayStagedFileUpload> FilesToUpload { get; set; } = new();
        public List<DentalXRayFileReference> FilesToDelete { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public sealed class DentalXRayStagedFileUpload
    {
        public required IFormFile File { get; init; }
        public required string Prefix { get; init; }
        public required string FinalFileName { get; init; }
        public string? StagedFullPath { get; set; }
    }

    public sealed class DentalXRayFileReference
    {
        public required string Prefix { get; init; }
        public required string FileName { get; init; }
    }

    public sealed class DentalXRayFileUploadSession
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> StagedFullPaths { get; set; } = new();
    }

    public sealed class DentalXRayFileProcessResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DentalXRayFileUpdatePlan Plan { get; set; } = new();
        public DentalXRayFileUploadSession Session { get; set; } = new();
    }
}

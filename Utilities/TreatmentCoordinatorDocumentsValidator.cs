using Microsoft.AspNetCore.Http;

namespace Malama.Utilities
{
    public static class TreatmentCoordinatorDocumentsValidator
    {
        public const int MaxDocumentCount = 3;
        public const long MaxDocumentBytes = 3L * 1024L * 1024L;

        /// <summary>
        /// Validates optional Treatment Coordinator document uploads.
        /// Returns an error message, or null when valid.
        /// </summary>
        public static string? Validate(IEnumerable<IFormFile>? documents)
        {
            var files = (documents ?? Enumerable.Empty<IFormFile>())
                .Where(f => f != null && f.Length > 0)
                .ToList();

            if (files.Count == 0)
            {
                return null;
            }

            if (files.Count > MaxDocumentCount)
            {
                return $"A maximum of {MaxDocumentCount} documents can be uploaded.";
            }

            foreach (var file in files)
            {
                if (file.Length > MaxDocumentBytes)
                {
                    var name = string.IsNullOrWhiteSpace(file.FileName) ? "A document" : file.FileName;
                    return $"{name} exceeds the maximum size of 3 MB.";
                }

                if (!IsPdfFile(file))
                {
                    var name = string.IsNullOrWhiteSpace(file.FileName) ? "A document" : file.FileName;
                    return $"{name} must be a PDF file.";
                }
            }

            return null;
        }

        private static bool IsPdfFile(IFormFile file)
        {
            var fileName = (file.FileName ?? string.Empty).Trim();
            var hasPdfExtension = fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
            var contentType = (file.ContentType ?? string.Empty).Trim();
            var hasPdfContentType = contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase)
                || contentType.Equals("application/x-pdf", StringComparison.OrdinalIgnoreCase);

            if (!hasPdfExtension && !hasPdfContentType)
            {
                return false;
            }

            return HasPdfMagicBytes(file);
        }

        private static bool HasPdfMagicBytes(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                Span<byte> header = stackalloc byte[5];
                var read = stream.Read(header);
                if (read < 4)
                {
                    return false;
                }

                // PDF files start with "%PDF"
                return header[0] == (byte)'%'
                    && header[1] == (byte)'P'
                    && header[2] == (byte)'D'
                    && header[3] == (byte)'F';
            }
            catch
            {
                return false;
            }
        }
    }
}

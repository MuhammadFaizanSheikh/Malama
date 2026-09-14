using Microsoft.AspNetCore.Http;

namespace Malama.Utilities
{
    public static class TreatmentConsentSignatureFileHelper
    {
        public static IFormFile? CreatePngFormFileFromDataUrl(string? dataUrl, string fileName)
        {
            if (string.IsNullOrWhiteSpace(dataUrl))
            {
                return null;
            }

            var commaIndex = dataUrl.IndexOf(',');
            var base64 = commaIndex >= 0 ? dataUrl[(commaIndex + 1)..] : dataUrl;
            if (string.IsNullOrWhiteSpace(base64))
            {
                return null;
            }

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64);
            }
            catch
            {
                return null;
            }

            if (bytes.Length == 0)
            {
                return null;
            }

            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "signature", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
        }

        public static bool HasInkDataUrl(string? dataUrl)
        {
            if (string.IsNullOrWhiteSpace(dataUrl))
            {
                return false;
            }

            var commaIndex = dataUrl.IndexOf(',');
            var base64 = commaIndex >= 0 ? dataUrl[(commaIndex + 1)..] : dataUrl;
            return !string.IsNullOrWhiteSpace(base64) && base64.Length > 64;
        }
    }
}

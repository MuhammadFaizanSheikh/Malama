namespace Malama.Models
{
    public class TreatmentConsentStartSmModeRequest
    {
        public long ServiceMembersChildId { get; set; }
    }

    public class TreatmentConsentUnlockSmModeRequest
    {
        public string Password { get; set; } = string.Empty;
    }

    public class TreatmentConsentSmModeResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public bool IsActive { get; set; }
        public long? ServiceMembersChildId { get; set; }
        public string? RedirectUrl { get; set; }

        public static TreatmentConsentSmModeResponse Ok(
            string? message = null,
            bool isActive = false,
            long? serviceMembersChildId = null,
            string? redirectUrl = null)
        {
            return new TreatmentConsentSmModeResponse
            {
                Success = true,
                Message = message,
                IsActive = isActive,
                ServiceMembersChildId = serviceMembersChildId,
                RedirectUrl = redirectUrl
            };
        }

        public static TreatmentConsentSmModeResponse Fail(string message)
        {
            return new TreatmentConsentSmModeResponse
            {
                Success = false,
                Message = message
            };
        }
    }
}

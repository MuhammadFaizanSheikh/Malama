using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    [Table("TreatmentConsent")]
    public class TreatmentConsent : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild? ServiceMembersChild { get; set; }

        /// <summary>Questionnaire is always included; this flag is informational only.</summary>
        public bool IncludeQuestionnaire { get; set; } = true;

        public bool IncludeOralSurgeryForm { get; set; }

        public bool IncludeDentalTreatmentConsent { get; set; }

        /// <summary>JSON array of EventStaff Ids selected for Oral Surgery Form.</summary>
        public string? OralSurgeryDentistEventStaffIdsJson { get; set; }

        /// <summary>JSON array of EventStaff Ids selected for Dental Treatment Consent.</summary>
        public string? DentalTreatmentDentistEventStaffIdsJson { get; set; }
    }

    public class TreatmentConsentFormSelectionDto
    {
        public long ServiceMembersChildId { get; set; }
        public bool IncludeQuestionnaire { get; set; } = true;
        public bool IncludeOralSurgeryForm { get; set; }
        public bool IncludeDentalTreatmentConsent { get; set; }
        public List<long> OralSurgeryDentistEventStaffIds { get; set; } = new();
        public List<long> DentalTreatmentDentistEventStaffIds { get; set; } = new();
    }

    public class TreatmentConsentSaveFormSelectionRequest
    {
        public long ServiceMembersChildId { get; set; }
        public bool IncludeOralSurgeryForm { get; set; }
        public bool IncludeDentalTreatmentConsent { get; set; }
        public List<long> OralSurgeryDentistEventStaffIds { get; set; } = new();
        public List<long> DentalTreatmentDentistEventStaffIds { get; set; } = new();
    }

    public class TreatmentConsentSaveFormSelectionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public TreatmentConsentFormSelectionDto? Selection { get; set; }

        public static TreatmentConsentSaveFormSelectionResponse Ok(string message, TreatmentConsentFormSelectionDto selection)
        {
            return new TreatmentConsentSaveFormSelectionResponse
            {
                Success = true,
                Message = message,
                Selection = selection
            };
        }

        public static TreatmentConsentSaveFormSelectionResponse Fail(string message)
        {
            return new TreatmentConsentSaveFormSelectionResponse
            {
                Success = false,
                Message = message
            };
        }
    }
}

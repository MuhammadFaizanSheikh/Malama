using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    [Table("DentalTreatment")]
    public class DentalTreatment : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        public long DentalExamId { get; set; }

        public string? SmFinalClassification { get; set; }

        public string Status { get; set; } = "Pending";

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild ServiceMembersChild { get; set; } = null!;

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalExam DentalExam { get; set; } = null!;

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<DentalTreatmentFinding> Findings { get; set; } = new List<DentalTreatmentFinding>();

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<DentalTreatmentSelectedTooth> SelectedTeeth { get; set; } = new List<DentalTreatmentSelectedTooth>();

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<DentalTreatmentAnesthesia> AnesthesiaRecords { get; set; } = new List<DentalTreatmentAnesthesia>();

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<DentalTreatmentPrescription> Prescriptions { get; set; } = new List<DentalTreatmentPrescription>();

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<DentalTreatmentOverallNote> OverallNotes { get; set; } = new List<DentalTreatmentOverallNote>();
    }

    [Table("DentalTreatmentFinding")]
    public class DentalTreatmentFinding
    {
        public long Id { get; set; }

        public long DentalTreatmentId { get; set; }

        public long? DentalExamFindingId { get; set; }

        public bool IsPrimaryTooth { get; set; }

        public string? AffectedTooth { get; set; }

        public string? DiseaseConditionType { get; set; }

        public string? AffectedSurfacesJson { get; set; }

        public string? CdtCodesJson { get; set; }

        public string? CdtCodesNotes { get; set; }

        public string? DescriptionDetails { get; set; }

        public string? Classification { get; set; }

        public string? TreatmentCompleted { get; set; }

        public string? PostServiceTreatmentJson { get; set; }

        public string? TreatmentCdtCodesJson { get; set; }

        public string? Reason { get; set; }

        public string? Notes { get; set; }

        public string? ProceduredDrc { get; set; }

        public string? DentistProfessional { get; set; }

        public string? TreatmentStatus { get; set; }

        public string? TreatmentDateTime { get; set; }

        public int SortOrder { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalTreatment DentalTreatment { get; set; } = null!;

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalExamFinding? DentalExamFinding { get; set; }
    }

    [Table("DentalTreatmentSelectedTooth")]
    public class DentalTreatmentSelectedTooth
    {
        public long Id { get; set; }

        public long DentalTreatmentId { get; set; }

        public int ToothNumber { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalTreatment DentalTreatment { get; set; } = null!;
    }

    [Table("DentalTreatmentAnesthesia")]
    public class DentalTreatmentAnesthesia
    {
        public long Id { get; set; }

        public long DentalTreatmentId { get; set; }

        public string? Date { get; set; }

        public string? CarpulesByTypeJson { get; set; }

        public int SortOrder { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalTreatment DentalTreatment { get; set; } = null!;
    }

    [Table("DentalTreatmentPrescription")]
    public class DentalTreatmentPrescription
    {
        public long Id { get; set; }

        public long DentalTreatmentId { get; set; }

        public string? Type { get; set; }

        public string? Product { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? Dosage { get; set; }

        public string? Duration { get; set; }

        public string? Frequency { get; set; }

        public string? PrescribedAmount { get; set; }

        public string? Notes { get; set; }

        public string? PrescribedBy { get; set; }

        public string? PrescribedOn { get; set; }

        public int SortOrder { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalTreatment DentalTreatment { get; set; } = null!;
    }

    [Table("DentalTreatmentOverallNote")]
    public class DentalTreatmentOverallNote
    {
        public long Id { get; set; }

        public long DentalTreatmentId { get; set; }

        public string Notes { get; set; } = string.Empty;

        public string? Dentist { get; set; }

        public string? NoteDateTime { get; set; }

        public int SortOrder { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalTreatment DentalTreatment { get; set; } = null!;
    }
}

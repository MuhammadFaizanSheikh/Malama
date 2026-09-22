using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    [Table("TreatmentCoordinatorAppointment")]
    public class TreatmentCoordinatorAppointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long DentalTreatmentId { get; set; }

        public long EventStaffId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string AppointmentStartTime { get; set; } = string.Empty;

        public string AppointmentDuration { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalTreatment DentalTreatment { get; set; } = null!;

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<TreatmentCoordinatorAppointmentFinding> Findings { get; set; }
            = new List<TreatmentCoordinatorAppointmentFinding>();
    }

    [Table("TreatmentCoordinatorAppointmentFinding")]
    public class TreatmentCoordinatorAppointmentFinding
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long AppointmentId { get; set; }

        public long DentalFindingId { get; set; }

        public string? FindingClientKey { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual TreatmentCoordinatorAppointment Appointment { get; set; } = null!;

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalFinding DentalFinding { get; set; } = null!;
    }

    public class TreatmentCoordinatorDocumentMetaDto
    {
        public string FileName { get; set; } = string.Empty;
        public string? OriginalFileName { get; set; }
        public DateTime? UploadedOn { get; set; }
    }
}

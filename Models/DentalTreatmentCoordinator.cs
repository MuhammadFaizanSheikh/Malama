using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    [Table("DentalTreatmentCoordinator")]
    public class DentalTreatmentCoordinator : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        public bool IsTreatmentRequired { get; set; } = true;

        /// <summary>JSON array of uploaded coordinator document metadata.</summary>
        public string? DocumentsJson { get; set; }

        public string? TreatmentCoordinatorUserId { get; set; }

        /// <summary>EventStaff Id of the Treatment Coordinator (source of truth for display).</summary>
        public long? TreatmentCoordinatorEventStaffId { get; set; }

        public DateTime? TreatmentCoordinatorDateTime { get; set; }

        public string? TreatmentCoordinatorComments { get; set; }

        public string Status { get; set; } = "Pending";

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild ServiceMembersChild { get; set; } = null!;

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<DentalAppointment> Appointments { get; set; } = new List<DentalAppointment>();
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    [Table("DentalDenClass")]
    public class DentalDenClassRecord : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild ServiceMembersChild { get; set; } = null!;

        public string? DenClass { get; set; }
        public string? DenClassReasonComments { get; set; }

        public string? Source { get; set; }
    }
}

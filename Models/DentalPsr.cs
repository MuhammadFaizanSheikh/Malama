using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    [Table("DentalPsr")]
    public class DentalPsr : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild ServiceMembersChild { get; set; } = null!;

        public string? PsrUpperRight { get; set; }
        public string? PsrUpperAnterior { get; set; }
        public string? PsrUpperLeft { get; set; }
        public string? PsrLowerRight { get; set; }
        public string? PsrLowerAnterior { get; set; }
        public string? PsrLowerLeft { get; set; }
        public string? PsrCarrierRisk { get; set; }
        public string? SoftTissuesWnl { get; set; }
        public string? SoftTissuesConditionDetail { get; set; }

        public string? Source { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<DentalPsrSelectedTooth> SelectedTeeth { get; set; } = new List<DentalPsrSelectedTooth>();
    }

    [Table("DentalPsrSelectedTooth")]
    public class DentalPsrSelectedTooth
    {
        public long Id { get; set; }

        public long DentalPsrId { get; set; }

        public int ToothNumber { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalPsr DentalPsr { get; set; } = null!;
    }
}

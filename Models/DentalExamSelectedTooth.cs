using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Malama.Models
{
    [Table("DentalExamSelectedTooth")]
    public class DentalExamSelectedTooth
    {
        public long Id { get; set; }

        public long DentalExamId { get; set; }

        public int ToothNumber { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalExam DentalExam { get; set; } = null!;
    }
}

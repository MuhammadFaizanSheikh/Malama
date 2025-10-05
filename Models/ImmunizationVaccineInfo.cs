using ExcelFilesCompiler.Controllers;
using ExcelFilesCompiler;
using Malama.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Malama.Models
{
    public class ImmunizationVaccineViewModel
    {
        public List<ImmunizationVaccineInfo>? ListOfImmunizationVaccineInfo { get; set; }
        public ImmunizationVaccineInfo SingleImmunizationVaccineInfo { get; set; }
    }

    [Table("ImmunizationVaccineInfo")]
    public class ImmunizationVaccineInfo : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Client { get; set; }

        [Required]
        public string EventId { get; set; }  // readonly in UI

        public string EventLocation { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string ImmunizationType { get; set; }

        [Required]
        public string Vaccine { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public int StartingDoses { get; set; }

        public int? FinalDoses { get; set; }  // optional

        [Required]
        public List<ImmunizationVaccineLotEntry> Lots { get; set; } = new List<ImmunizationVaccineLotEntry>();
    }

    [Table("ImmunizationVaccineLotEntry")]
    public class ImmunizationVaccineLotEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string LotNumber { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        public int ImmunizationVaccineInfoId { get; set; }

        // ✅ Navigation property (optional in DTO, but useful for EF/entity mapping)
        public ImmunizationVaccineInfo ImmunizationVaccineInfo { get; set; }
    }

}

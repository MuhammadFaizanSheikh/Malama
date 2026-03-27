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
        [ValidateNever]
        public long EventId { get; set; }
        [ValidateNever]
        public string EventIdAndVersion { get; set; }
        public List<ImmunizationVaccineInfoForPreview>? ListOfImmunizationVaccineInfo { get; set; }
        public ImmunizationVaccineInfo SingleImmunizationVaccineInfo { get; set; }
    }

    public class ImmunizationVaccineInfoForPreview
    {
        public long Id { get; set; }

        public string LotNumber { get; set; }
        public string Expiration { get; set; }
        public string ImmunizationType { get; set; }
        public string Vaccine { get; set; }
        public string ContainerName { get; set; }
        public string Dose { get; set; }
        public string Unit { get; set; }

        public string Manufacturer { get; set; }

        public int? StartingDoses { get; set; }

        public int? FinalDoses { get; set; }  // optional
        public string? AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
    }

    [Table("ImmunizationVaccineInfo")]
    public class ImmunizationVaccineInfo : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long EventManagementId { get; set; }

        [ForeignKey("EventManagementId")]
        [JsonIgnore]
        [ValidateNever]
        public virtual EventManagement EventManagement { get; set; }

        [Required(ErrorMessage = "Event Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Immunization type is required")]
        [StringLength(100, ErrorMessage = "Immunization type cannot exceed 100 characters")]
        public string ImmunizationType { get; set; }

        [Required(ErrorMessage = "Vaccine name is required")]
        [StringLength(100, ErrorMessage = "Vaccine name cannot exceed 100 characters")]
        public string Vaccine { get; set; }

        [Required(ErrorMessage = "Manufacturer name is required")]
        [StringLength(100, ErrorMessage = "Manufacturer name cannot exceed 100 characters")]
        public string Manufacturer { get; set; }

        [Required(ErrorMessage = "Dose is required")]
        [Range(0.1, 100, ErrorMessage = "Dose must be greater than 0")]
        public decimal Dose { get; set; }


        [Required(ErrorMessage = "Unit is required")]
        [StringLength(2, ErrorMessage = "Unit cannot exceed 2 characters")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Starting doses are required")]
        [Range(1, int.MaxValue, ErrorMessage = "Starting doses must be greater than 0")]
        public int? StartingDoses { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Final doses cannot be negative")]
        public int? FinalDoses { get; set; }  // optional

        [Required(ErrorMessage = "At least one lot entry is required")]
        [MinLength(1, ErrorMessage = "At least one lot entry must be provided")]
        public List<ImmunizationVaccineLotEntry> Lots { get; set; } = new List<ImmunizationVaccineLotEntry>();

        
    }

    [Table("ImmunizationVaccineLotEntry")]
    public class ImmunizationVaccineLotEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Lot number is required")]
        [StringLength(50, ErrorMessage = "Lot number cannot exceed 50 characters")]
        public string LotNumber { get; set; }

        [Required(ErrorMessage = "Expiration date is required")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Expiration { get; set; }


        [Required(ErrorMessage = "Container selection is required")]
        [ForeignKey("Container")]
        public long ContainerId { get; set; }   // FK column in ImmunizationVaccineInfo table

        public Container? Container { get; set; }

        public long ImmunizationVaccineInfoId { get; set; }

        // ✅ Navigation property (useful for EF/entity mapping, optional in DTOs)
        [ValidateNever]
        public ImmunizationVaccineInfo ImmunizationVaccineInfo { get; set; }
    }
}

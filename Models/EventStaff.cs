using ExcelFilesCompiler.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExcelToCsv.Models
{
    public class EventStaffViewModel
    {
        public List<EventStaff>? EventStaff { get; set; }
        public EventStaff SingleEventStaff { get; set; }
    }

    public class CombinedEventStaffSubContractorAndContractDto
    {
        public EventStaff EventStaff { get; set; }
        public List<StaffSubContractorAffiliationDto> StaffSubContractorAffiliation { get; set; }
        public List<TravelHonor> TravelHonor { get; set; }
    }

    public class StaffSubContractorAffiliationDto
    {
        public long SubContractorId { get; set; }
        public string SubContractorName { get; set; }
        public List<StaffContractAffiliationDto> StaffContractAffiliation { get; set; }
    }
    public class StaffContractAffiliationDto
    {
        public long ContractId { get; set; }
        public string ContractName { get; set; }
    }

    [Table("EventStaff")]
    public class EventStaff : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // Primary key, auto-incremented

        [StringLength(6, ErrorMessage = "StaffID cannot exceed 6 characters.")]
        [Required(ErrorMessage = "StaffID is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffID cannot be only white spaces.")]
        public string StaffID { get; set; }
        public DateTime StartDate { get; set; }

        [StringLength(20, ErrorMessage = "StaffStatus cannot exceed 20 characters.")]
        [Required(ErrorMessage = "StaffStatus is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffStatus cannot be only white spaces.")]
        public string StaffStatus { get; set; }

        [StringLength(50, ErrorMessage = "StaffLastName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "StaffLastName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffLastName cannot be only white spaces.")]
        public string StaffLastName { get; set; }

        [StringLength(50, ErrorMessage = "StaffFirstName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "StaffFirstName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffFirstName cannot be only white spaces.")]
        public string StaffFirstName { get; set; }

        [StringLength(50, ErrorMessage = "StaffMiddleInitial cannot exceed 50 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffMiddleInitial cannot be only white spaces.")]
        public string? StaffMiddleInitial { get; set; }

        [StringLength(11, ErrorMessage = "StaffSSN cannot exceed 11 characters.")]
        [Required(ErrorMessage = "StaffSSN is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffSSN cannot be only white spaces.")]
        public string StaffSSN { get; set; }

        [Required(ErrorMessage = "StaffDOB is required.")]
        public DateTime StaffDOB { get; set; }

        //public ICollection<StaffRoleDto> StaffRoles { get; set; } = new List<StaffRoleDto>();

        [Required(ErrorMessage = "EventOnCallStaff is required.")]
        [RegularExpression("^(true|false)$", ErrorMessage = "EventOnCallStaff : Invalid selection. Choose 'Yes' or 'No'.")]
        public string EventOnCallStaff { get; set; }

        [StringLength(50, ErrorMessage = "EventOnCallStaffEvent cannot exceed 50 characters.")]
        //[Required(ErrorMessage = "EventOnCallStaffEvent is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventOnCallStaffEvent cannot be only white spaces.")]
        public string? EventOnCallStaffEvent { get; set; }

        [StringLength(10, ErrorMessage = "NPI must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "NPI must be a numeric value with exactly 10 digits.")]
        public string? NPI { get; set; }

        [StringLength(8, ErrorMessage = "DAE cannot exceed 8 characters.")]
        [Required(ErrorMessage = "DAE is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "DAE cannot be only white spaces.")]
        public string DAE { get; set; }
        public DateTime CredentialingProcessDate { get; set; }
        public DateTime? HistoricalCredentialingDate { get; set; }
        public DateTime DAWSONInternalCredentialingCompleteDate { get; set; }

        [Required(ErrorMessage = "OnboardingTrainingComplete is required.")]
        [RegularExpression("^(true|false)$", ErrorMessage = "OnboardingTrainingComplete : Invalid selection. Choose 'Yes' or 'No'.")]
        public string OnboardingTrainingComplete { get; set; }

        [StringLength(100, ErrorMessage = "OutstandingTrainings cannot exceed 100 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "OutstandingTrainings cannot be only white spaces.")]
        public string? OutstandingTrainings { get; set; }

        [StringLength(100, ErrorMessage = "BackgroundCheckConcerns cannot exceed 100 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "BackgroundCheckConcerns cannot be only white spaces.")]
        public string? BackgroundCheckConcerns { get; set; }
        public DateTime? BLSCertDate { get; set; }
        [StringLength(100, ErrorMessage = "BLSCertNumber cannot exceed 100 characters.")]
        public string? BLSCertNumber { get; set; }
        public DateTime? ACLSCertDate { get; set; }

        [StringLength(100, ErrorMessage = "ACLSCertNumber cannot exceed 100 characters.")]
        public string? ACLSCertNumber { get; set; }

        [Required(ErrorMessage = "CACApplicationProcessStatus is required.")]
        public string CACApplicationProcessStatus { get; set; }

        [Required(ErrorMessage = "StaffCAC is required.")]
        [RegularExpression("^(true|false)$", ErrorMessage = "StaffCAC : Invalid selection. Choose 'Yes' or 'No'.")]
        public string StaffCAC { get; set; }
        

        [StringLength(10, ErrorMessage = "StaffDoDID cannot exceed 10 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffDoDID cannot be only white spaces.")]
        public string? StaffDoDID { get; set; }

        public DateTime? CacExpiryDate { get; set; }

        //[Required(ErrorMessage = "SubContractorId is required.")]
        //public long SubContractorId { get; set; }

        [Required(ErrorMessage = "StaffCellNumber is required.")]
        [StringLength(12, ErrorMessage = "StaffCellNumber cannot exceed 12 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffCellNumber cannot be only white spaces.")]
        public string StaffCellNumber { get; set; }
        public string? StaffPhone2 { get; set; }

        [Required(ErrorMessage = "StaffEmail is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid StaffEmail address.")]
        public string StaffEmail { get; set; }

        // Primary Residence fields
        [Required(ErrorMessage = "PrimaryAddress1 is required.")]
        [StringLength(200, ErrorMessage = "PrimaryAddress1 cannot exceed 200 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffCPrimaryAddress1ellNumber cannot be only white spaces.")]
        public string PrimaryAddress1 { get; set; }

        [StringLength(200, ErrorMessage = "PrimaryAddress2 cannot exceed 200 characters.")]
        public string? PrimaryAddress2 { get; set; }

        [Required(ErrorMessage = "PrimaryCity is required.")]
        [StringLength(50, ErrorMessage = "PrimaryCity cannot exceed 50 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "PrimaryCity cannot be only white spaces.")]
        public string PrimaryCity { get; set; }

        [Required(ErrorMessage = "PrimaryState is required.")]
        [StringLength(50, ErrorMessage = "PrimaryState cannot exceed 50 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "PrimaryState cannot be only white spaces.")]
        public string PrimaryState { get; set; }

        [Required(ErrorMessage = "PrimaryZip is required.")]
        [StringLength(50, ErrorMessage = "PrimaryZip cannot exceed 50 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "PrimaryZip cannot be only white spaces.")]
        public string PrimaryZip { get; set; }

        // Secondary Residence fields

        [Required(ErrorMessage = "SecondaryAddress1 is required.")]
        [StringLength(200, ErrorMessage = "SecondaryAddress1 cannot exceed 200 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "SecondaryAddress1 cannot be only white spaces.")]
        public string SecondaryAddress1 { get; set; }

        [StringLength(200, ErrorMessage = "SecondaryAddress2 cannot exceed 200 characters.")]
        public string? SecondaryAddress2 { get; set; }

        [Required(ErrorMessage = "SecondaryCity is required.")]
        [StringLength(50, ErrorMessage = "SecondaryCity cannot exceed 50 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "SecondaryCity cannot be only white spaces.")]
        public string SecondaryCity { get; set; }

        [Required(ErrorMessage = "SecondaryState is required.")]
        [StringLength(50, ErrorMessage = "SecondaryState cannot exceed 50 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "SecondaryState cannot be only white spaces.")]
        public string SecondaryState { get; set; }

        [Required(ErrorMessage = "SecondaryZip is required.")]
        [StringLength(50, ErrorMessage = "SecondaryZip cannot exceed 50 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "SecondaryZip cannot be only white spaces.")]
        public string SecondaryZip { get; set; }

        [Required(ErrorMessage = "StaffInfoEnteredBy is required.")]
        [StringLength(50, ErrorMessage = "StaffInfoEnteredBy cannot exceed 50 characters.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "StaffInfoEnteredBy cannot be only white spaces.")]
        public string StaffInfoEnteredBy { get; set; }

        public List<LicenseInfoDTO> Licenses { get; set; } = new List<LicenseInfoDTO>();

        //[Required(ErrorMessage = "StaffContractAffiliation is required.")]
        //[NotMapped]
        //public List<long> StaffContractAffiliation { get; set; } = new List<long>();

        public List<StaffContractAffiliation> StaffContractAffiliation { get; set; } = new List<StaffContractAffiliation>();
        public bool TravelHonorAir { get; set; }

        public bool TravelHonorCar { get; set; }
        public bool TravelHonorHotel { get; set; }
        //public List<TravelHonor> TravelHonorCarList { get; set; } = new List<TravelHonor>();  // List of Airlines
        public List<TravelHonor>? TravelHonorList { get; set; } = new List<TravelHonor>();  // List of Airlines

    }

    [Table("StaffContractAffiliation")]
    public class StaffContractAffiliation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // Primary key

        [ForeignKey("EventStaff")]
        public long EventStaffId { get; set; }
        public long SubContractorId { get; set; }
        
        [NotMapped]
        public string? SubContractorName { get; set; }
        public long ContractId { get; set; }//public List<SubContractorContractAffiliation> SubContractorContractAffiliation { get; set; } = new List<SubContractorContractAffiliation>();


        [NotMapped]
        public List<long> StaffContractAffiliationTemp { get; set; } = new List<long>();
    }

    //[Table("SubContractorContractAffiliation")]
    //public class SubContractorContractAffiliation
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public long Id { get; set; } // Primary key

    //    [ForeignKey("StaffSubContractorAffiliation")]
    //    public long StaffSubContractorAffiliationId { get; set; }
    //    public long ContractId { get; set; }
    //}

    //[Table("StaffRoles")]
    //public class StaffRoleDto
    //{
    //    [Key]
    //    public long Id { get; set; }

    //    [Required]
    //    public long EventStaffId { get; set; }

    //    [Required]
    //    public string RoleName { get; set; }

    //    [ForeignKey("EventStaffId")]
    //    public EventStaffDto EventStaff { get; set; }

    //}

    [Table("StaffLicense")]
    public class LicenseInfoDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // Primary key, auto-incremented

        [ForeignKey("EventStaff")]

        public long EventStaffId { get; set; }


        [Required]
        [StringLength(50)]
        public string RoleId { get; set; }

        public List<StaffLicenseDetails> LicenseDetails { get; set; } = new List<StaffLicenseDetails>();
    }

    [Table("StaffLicenseDetails")]
    public class StaffLicenseDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // Primary key, auto-incremented

        [ForeignKey("StaffLicense")]

        public long StaffLicenseId { get; set; }


        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseState { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseType { get; set; }

        [Required]
        public DateTime LicenseActiveDate { get; set; }
        [Required]
        public DateTime LicenseExpiryDate { get; set; }
    }

    [Table("TravelHonor")]
    public class TravelHonor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("EventStaff")]
        public long EventStaffId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(6,0)")]
        public decimal Rewards { get; set; }
    }
}

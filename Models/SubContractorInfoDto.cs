 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExcelFilesCompiler.Models
{
    public class CombinedSubContractorAndContractDto
    {
        public SubContractorInfoDto SubContractor { get; set; }
        public ContractDetails ContractDetails { get; set; }
    }

    public class SubContractorViewModel
    {
        //public List<SubContractorInfoDto>? SubContractor { get; set; }
        public List<SubContractorAndContractViewModel>? SubContractor { get; set; }
        public SubContractorInfoDto SingleSubContractor { get; set; }
    }

    public class SubContractorAndContractViewModel
    {
        public long Id { get; set; }
        public string ContractId { get; set; }
        public string ContractName { get; set; }
        public string CompanyId { get; set; }
        public string CompanyMainName { get; set; }
        public string ContractClient { get; set; }
        public string ContractType { get; set; }
        public string SmallBusinessType { get; set; }
        public string ContractAffiliation { get; set; }
        public string ContractServiceBranch { get; set; }
        public string ContractComponent { get; set; }
        public string SolicitationNumber { get; set; }
    }

    [Table("SubContractor")]
    public class SubContractorInfoDto : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // Primary key, auto-incremented

        [Required(ErrorMessage = "Contract ID is required.")]
        [Range(1, 9999999999999, ErrorMessage = "Contract ID must be a valid numeric value.")]
        public long ContractId { get; set; }

        [StringLength(20, ErrorMessage = "CompanyId cannot exceed 20 characters.")]
        [Required(ErrorMessage = "CompanyId is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "CompanyId cannot be only white spaces.")]
        public string CompanyId { get; set; }

        //[StringLength(20, ErrorMessage = "ContractClient cannot exceed 20 characters.")]
        //[Required(ErrorMessage = "ContractClient is required.")]
        //[RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "ContractClient cannot be only white spaces.")]
        //public string ContractClient { get; set; }

        //[StringLength(20, ErrorMessage = "ContractType cannot exceed 20 characters.")]
        //[Required(ErrorMessage = "ContractType is required.")]
        //[RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "ContractType cannot be only white spaces.")]
        //public string ContractType { get; set; }

        [StringLength(300, ErrorMessage = "SmallBusinessType cannot exceed 300 characters.")]
        [Required(ErrorMessage = "SmallBusinessType is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "SmallBusinessType cannot be only white spaces.")]
        public string SmallBusinessType { get; set; }

        [StringLength(50, ErrorMessage = "ContractAffiliation cannot exceed 50 characters.")]
        [Required(ErrorMessage = "ContractAffiliation is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "ContractAffiliation cannot be only white spaces.")]
        public string ContractAffiliation { get; set; }

        //[StringLength(20, ErrorMessage = "ContractServiceBranch cannot exceed 20 characters.")]
        //[Required(ErrorMessage = "ContractServiceBranch is required.")]
        //[RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "ContractServiceBranch cannot be only white spaces.")]
        //public string ContractServiceBranch { get; set; }

        //[StringLength(20, ErrorMessage = "ContractComponent cannot exceed 20 characters.")]
        //[Required(ErrorMessage = "ContractComponent is required.")]
        //[RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "ContractComponent cannot be only white spaces.")]
        //public string ContractComponent { get; set; }

        [StringLength(20, ErrorMessage = "SolicitationNumber cannot exceed 20 characters.")]
        [Required(ErrorMessage = "SolicitationNumber is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "SolicitationNumber cannot be only white spaces.")]
        public string SolicitationNumber { get; set; }

        [StringLength(50, ErrorMessage = "CompanyName cannot exceed 50 characters.")]
        [MinLength(3, ErrorMessage = "CompanyName must be at least 3 characters.")]
        [Required(ErrorMessage = "CompanyName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "CompanyName cannot be only white spaces.")]
        public string CompanyMainName { get; set; }

        [StringLength(200, ErrorMessage = "Address1 cannot exceed 200 characters.")]
        [Required(ErrorMessage = "Address1 is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Address1 cannot be only white spaces.")]
        public string CompanyMainAddress1 { get; set; }

        [StringLength(200, ErrorMessage = "Address2 cannot exceed 200 characters.")]
        public string? CompanyMainAddress2 { get; set; }

        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        [Required(ErrorMessage = "City is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "City cannot be only white spaces.")]
        public string CompanyMainCity { get; set; }

        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters.")]
        [Required(ErrorMessage = "State is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "State cannot be only white spaces.")]
        public string CompanyMainState { get; set; }

        [StringLength(50, ErrorMessage = "Zip cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Zip is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Zip cannot be only white spaces.")]
        public string CompanyMainZip { get; set; }

        [StringLength(50, ErrorMessage = "LastName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "LastName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "LastName cannot be only white spaces.")]
        public string CompanyMainLastName { get; set; }

        [StringLength(50, ErrorMessage = "FirstName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "FirstName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "FirstName cannot be only white spaces.")]
        public string CompanyMainFirstName { get; set; }

        [StringLength(12, ErrorMessage = "Phone cannot exceed 12 characters.")]
        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Phone cannot be only white spaces.")]
        public string CompanyMainPhone { get; set; }

        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Email cannot be only white spaces.")]
        public string CompanyMainEmail { get; set; }

        [StringLength(50, ErrorMessage = "FinanceLastName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "FinanceLastName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "FinanceLastName cannot be only white spaces.")]
        public string FinanceLastName { get; set; }

        [StringLength(50, ErrorMessage = "FinanceFirstName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "FinanceFirstName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "FinanceFirstName cannot be only white spaces.")]
        public string FinanceFirstName { get; set; }

        [StringLength(200, ErrorMessage = "FinanceAddress1 cannot exceed 200 characters.")]
        [Required(ErrorMessage = "FinanceAddress1 is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "FinanceAddress1 cannot be only white spaces.")]
        public string FinanceAddress1 { get; set; }

        [StringLength(200, ErrorMessage = "FinanceAddress2 cannot exceed 200 characters.")]
        public string? FinanceAddress2 { get; set; }

        [StringLength(50, ErrorMessage = "FinanceCity cannot exceed 50 characters.")]
        [Required(ErrorMessage = "FinanceCity is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "FinanceCity cannot be only white spaces.")]
        public string FinanceCity { get; set; }

        [StringLength(50, ErrorMessage = "FinanceState cannot exceed 50 characters.")]
        [Required(ErrorMessage = "FinanceState is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "FinanceState cannot be only white spaces.")]
        public string FinanceState { get; set; }

        [StringLength(50, ErrorMessage = "FinanceZip cannot exceed 50 characters.")]
        [Required(ErrorMessage = "FinanceZip is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "FinanceZip cannot be only white spaces.")]
        public string FinanceZip { get; set; }

        [StringLength(12, ErrorMessage = "FinancePhone cannot exceed 12 characters.")]
        [Required(ErrorMessage = "FinancePhone is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "FinancePhone cannot be only white spaces.")]
        public string FinancePhone { get; set; }

        [StringLength(50, ErrorMessage = "FinanceEmail cannot exceed 50 characters.")]
        [Required(ErrorMessage = "FinanceEmail is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "FinanceEmail cannot be only white spaces.")]
        public string FinanceEmail { get; set; }

        [StringLength(50, ErrorMessage = "EventLastName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "EventLastName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventLastName cannot be only white spaces.")]
        public string EventLastName { get; set; }

        [StringLength(50, ErrorMessage = "EventFirstName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "EventFirstName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventFirstName cannot be only white spaces.")]
        public string EventFirstName { get; set; }

        [StringLength(12, ErrorMessage = "EventPhone cannot exceed 12 characters.")]
        [Required(ErrorMessage = "EventPhone is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventPhone cannot be only white spaces.")]
        public string EventPhone { get; set; }

        [StringLength(50, ErrorMessage = "EventEmail cannot exceed 50 characters.")]
        [Required(ErrorMessage = "EventEmail is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventEmail cannot be only white spaces.")]
        public string EventEmail { get; set; }

        [StringLength(50, ErrorMessage = "TrainingLastName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "TrainingLastName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "TrainingLastName cannot be only white spaces.")]
        public string TrainingLastName { get; set; }

        [StringLength(50, ErrorMessage = "TrainingFirstName cannot exceed 50 characters.")]
        [Required(ErrorMessage = "TrainingFirstName is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "TrainingFirstName cannot be only white spaces.")]
        public string TrainingFirstName { get; set; }

        [StringLength(12, ErrorMessage = "TrainingPhone cannot exceed 12 characters.")]
        [Required(ErrorMessage = "TrainingPhone is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "TrainingPhone cannot be only white spaces.")]
        public string TrainingPhone { get; set; }

        [StringLength(50, ErrorMessage = "TrainingEmail cannot exceed 50 characters.")]
        [Required(ErrorMessage = "TrainingEmail is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "TrainingEmail cannot be only white spaces.")]
        public string TrainingEmail { get; set; }

        //[StringLength(50, ErrorMessage = "EventId cannot exceed 50 characters.")]
        //[Required(ErrorMessage = "EventId is required.")]
        //[RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventId cannot be only white spaces.")]
        //public string EventId { get; set; }

        //[StringLength(50, ErrorMessage = "EventStatus cannot exceed 50 characters.")]
        //[Required(ErrorMessage = "EventStatus is required.")]
        //[RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventStatus cannot be only white spaces.")]
        //public string EventStatus { get; set; }

        //[StringLength(50, ErrorMessage = "EventRole cannot exceed 50 characters.")]
        //[Required(ErrorMessage = "EventRole is required.")]
        //[RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventRole cannot be only white spaces.")]
        //public string EventRole { get; set; }

        //[StringLength(50, ErrorMessage = "EventCity cannot exceed 50 characters.")]
        //[Required(ErrorMessage = "EventCity is required.")]
        //[RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventCity cannot be only white spaces.")]
        //public string EventCity { get; set; }

        //[StringLength(50, ErrorMessage = "EventState cannot exceed 50 characters.")]
        //[Required(ErrorMessage = "EventState is required.")]
        //[RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "EventState cannot be only white spaces.")]
        //public string EventState { get; set; }

        //[Required(ErrorMessage = "EventDate is required.")]
        //public string EventDate { get; set; }
    }
}

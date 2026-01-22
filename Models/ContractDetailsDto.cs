namespace Malama.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ContractViewModel
    {
        public List<ContractDetails>? Contracts { get; set; }
        public ContractDetails SingleContract { get; set; }
    }

    [Table("ContractDetails")]
    public class ContractDetails : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // Primary key, auto-incremented

        // Contract Details Properties
        [StringLength(13, ErrorMessage = "Contract ID cannot exceed 13 characters.")]
        [Required(ErrorMessage = "Contract ID is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Contract ID cannot be only white spaces.")]
        public string ContractID { get; set; }

        [StringLength(32, ErrorMessage = "Contract Name cannot exceed 32 characters.")]
        [Required(ErrorMessage = "Contract Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Contract Name cannot be only white spaces.")]
        public string ContractName { get; set; }


        [StringLength(50, ErrorMessage = "Contract Agency cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Contract Agency is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Contract Agency cannot be only white spaces.")]
        public string ContractAgency { get; set; }

        [StringLength(20, ErrorMessage = "Contract Service Branch cannot exceed 20 characters.")]
        [Required(ErrorMessage = "Contract Service Branch is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Contract Service Branch cannot be only white spaces.")]
        public string ContractServiceBranch { get; set; }

        [StringLength(20, ErrorMessage = "Contract Component cannot exceed 20 characters.")]
        [Required(ErrorMessage = "Contract Component is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Contract Component Branch cannot be only white spaces.")]
        public string ContractComponent { get; set; }

        [StringLength(20, ErrorMessage = "Contract client cannot exceed 20 characters.")]
        [Required(ErrorMessage = "Contract Client is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Contract Client cannot be only white spaces.")]
        public string ContractClient { get; set; }

        [StringLength(20, ErrorMessage = "Contract type cannot exceed 20 characters.")]
        [Required(ErrorMessage = "Contract Type is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Contract Type cannot be only white spaces.")]
        public string ContractType { get; set; }

        [StringLength(20, ErrorMessage = "Contract role cannot exceed 20 characters.")]
        [Required(ErrorMessage = "Dawson Role On Contract is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Dawson Role On Contract cannot be only white spaces.")]
        public string DawsonRoleOnContract { get; set; }

        [StringLength(20, ErrorMessage = "Contract status cannot exceed 20 characters.")]
        [Required(ErrorMessage = "Contract Status is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Contract Status cannot be only white spaces.")]
        public string ContractStatus { get; set; }

        [StringLength(20, ErrorMessage = "Client Name cannot exceed 20 characters.")]
        [Required(ErrorMessage = "Client Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Client Name cannot be only white spaces.")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "Contract Start Date is required.")]
        public DateTime ContractStartDate { get; set; } // DateTime cannot be null, so we remove the nullable (?)

        [Required(ErrorMessage = "Contract End Date is required.")]
        public DateTime ContractEndDate { get; set; } // DateTime cannot be null, so we remove the nullable (?)

        [StringLength(20, ErrorMessage = "Site Id cannot exceed 20 characters.")]
        [Required(ErrorMessage = "Site Id is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Site Id cannot be only white spaces.")]
        public string SiteId { get; set; }

        // Contract Officer (KO) Properties
        [StringLength(50, ErrorMessage = "KO Last name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "KO Last Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "KO Last Name cannot be only white spaces.")]
        public string KoLastName { get; set; }

        [StringLength(50, ErrorMessage = "KO First name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "KO First Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "KO First Name cannot be only white spaces.")]
        public string KoFirstName { get; set; }

        [StringLength(12, ErrorMessage = "KO Phone cannot exceed 12 characters.")]
        [Required(ErrorMessage = "KO Phone is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "KO Phone cannot be only white spaces.")]
        public string KOPhone { get; set; }

        [StringLength(12, ErrorMessage = "KO Phone2 cannot exceed 12 characters.")]
        [Required(ErrorMessage = "KO Phone 2 is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "KO Phone 2 cannot be only white spaces.")]
        public string KOPhone2 { get; set; }

        [StringLength(50, ErrorMessage = "KO email cannot exceed 50 characters.")]
        [Required(ErrorMessage = "KO Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "KO Email cannot be only white spaces.")]
        public string KOEmail { get; set; }

        [StringLength(300, ErrorMessage = "KO notes cannot exceed 300 characters.")]
        [Required(ErrorMessage = "KO Notes are required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "KO Notes cannot be only white spaces.")]
        public string KONotes { get; set; }

        [StringLength(50, ErrorMessage = "COR Last name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "COR Last Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "COR Last Name cannot be only white spaces.")]
        public string CORLastName { get; set; }

        [StringLength(5, ErrorMessage = "COR First name prefix cannot exceed 5 characters.")]
        [Required(ErrorMessage = "COR First Name prefix is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "COR First Name prefix cannot be only white spaces.")]
        public string CORPrefix { get; set; }

        [StringLength(50, ErrorMessage = "COR First name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "COR First Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "COR First Name cannot be only white spaces.")]
        public string CORFirstName { get; set; }

        [StringLength(50, ErrorMessage = "COR Rank cannot exceed 50 characters.")]
        [Required(ErrorMessage = "COR Rank is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "COR Rank cannot be only white spaces.")]
        public string CORKORank { get; set; }

        [StringLength(12, ErrorMessage = "COR Phone cannot exceed 12 characters.")]
        [Required(ErrorMessage = "COR Phone is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "COR Phone cannot be only white spaces.")]
        public string CORPhone { get; set; }

        [StringLength(12, ErrorMessage = "COR Phone2 cannot exceed 12 characters.")]
        [Required(ErrorMessage = "COR Phone 2 is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "COR Phone 2 cannot be only white spaces.")]
        public string CORPhone2 { get; set; }

        [StringLength(50, ErrorMessage = "COR email cannot exceed 50 characters.")]
        [Required(ErrorMessage = "COR Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "COR Email cannot be only white spaces.")]
        public string COREmail { get; set; }

        [StringLength(300, ErrorMessage = "COR Notes cannot exceed 300 characters.")]
        [Required(ErrorMessage = "COR Notes are required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "COR Notes cannot be only white spaces.")]
        public string CORNotes { get; set; }

        [StringLength(50, ErrorMessage = "Dawson Program Manager cannot Last name exceed 50 characters.")]
        [Required(ErrorMessage = "Dawson Program Manager Last Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Dawson Program Manager Last Name cannot be only white spaces.")]
        public string DawsonProgramManagerLastName { get; set; }

        [StringLength(50, ErrorMessage = "Dawson Program Manager cannot First Name exceed 50 characters.")]
        [Required(ErrorMessage = "Dawson Program Manager First Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Dawson Program Manager First Name cannot be only white spaces.")]
        public string DawsonProgramManagerFirstName { get; set; }

        [StringLength(50, ErrorMessage = "Dawson Program Manager Last Name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Dawson Deputy Program Manager Last Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Dawson Deputy Program Manager Last Name cannot be only white spaces.")]
        public string DawsonDeputyProgramManagerLastName { get; set; }

        [StringLength(50, ErrorMessage = "Dawson Deputy Program Manager First Name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Dawson Deputy Program Manager First Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Dawson Deputy Program Manager First Name cannot be only white spaces.")]
        public string DawsonDeputyProgramManagerFirstName { get; set; }

        [StringLength(50, ErrorMessage = "Dawson Program Manager Last Name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Dawson Project Manager Last Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Dawson Project Manager Last Name cannot be only white spaces.")]
        public string DawsonProjectManagerLastName { get; set; }

        [StringLength(50, ErrorMessage = "Dawson Project Manager First Name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Dawson Project Manager First Name is required.")]
        [RegularExpression(@"^\s*[\S]+.*$", ErrorMessage = "Dawson Project Manager First Name cannot be only white spaces.")]
        public string DawsonProjectManagerFirstName { get; set; }
    }

}

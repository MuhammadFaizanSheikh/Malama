namespace Malama.Models
{
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EventManagementViewModel
    {
        public List<EventManagementPreview>? EventManagements { get; set; }
        public EventManagement SingleEventManagement { get; set; }
        public string SubmissionToken { get; set; }
    }

    public class CombinedEventManagementAndContractDetails
    {
        public EventManagement EventManagement { get; set; }
        public ContractDetails ContractDetails { get; set; }
        public EventStaff EventStaffForHIVDropOff { get; set; }
        public List<EventStaffDetailAndAdditionalRoles> EventStaffDetailAndAdditionalRoleslist { get; set; }
    }

    public class EventStaffDetailAndAdditionalRoles
    {
        public CombinedEventStaffRolesNameAndLicense EventStaffRolesNameAndLicense { get; set; }
        public EventStaffDetail EventStaffDetail { get; set; }

    }

    public class EventManagementPreview
    {
        public long Id { get; set; }
        public string? EventID { get; set; }
        public int EventVersion { get; set; }
        public string? SubEventID { get; set; }
        public string? EventStatus { get; set; }
        public string? TaskForce { get; set; }
        public string? EventState { get; set; }
        public string? EventCity { get; set; }
        public string? EventZipCode { get; set; }
        public string? StatusDescription { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public Boolean CanEdit { get; set; }
    }

    [Table("EventManagement")]
    public class EventManagement : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // Event Specific Details
        [Required]
        public string EventID { get; set; } // Read-only field
        [MaxLength(50)]
        public string? SubEventID { get; set; }

        [Required]
        public int EventVersion { get; set; }

        [Required]
        [MaxLength(50)]
        public string EventStatus { get; set; }

        [Required]
        public long ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        [JsonIgnore]
        [ValidateNever]
        public ContractDetails ContractDetails { get; set; }

        [Required]
        [MaxLength(200)]
        public string EventAddress1 { get; set; }
        [MaxLength(200)]
        public string? EventAddress2 { get; set; }
        [MaxLength(50)]
        public string EventState { get; set; }
        [Required]
        [MaxLength(50)]
        public string EventCity { get; set; }
        [Required]
        [MaxLength(50)]
        public string EventZipCode { get; set; }

        [Required]
        public int TotalRequestedServiceMembers { get; set; }

        [Required]
        public DateTime EventStartDate { get; set; }
        [Required]
        public DateTime EventEndDate { get; set; }
        
        [Required]
        public string Deploy { get; set; }
        public DateTime? MOBDate { get; set; }

        [MaxLength(300)]
        public string? RegardingSites { get; set; }

        [Required]
        [MaxLength(12)]
        public string EventHelpLine { get; set; }

        // Event Main POC
        [MaxLength(50)]
        public string? MainPocLastName { get; set; }
        [MaxLength(50)]
        public string? MainPocFirstName { get; set; }
        [MaxLength(50)]
        public string? MainPocRank { get; set; }
        [MaxLength(12)]
        public string? MainPocPhonePrimary { get; set; }
        [MaxLength(12)]
        public string? MainPocPhoneSecondary { get; set; }
        [MaxLength(50)]
        public string? MainPocEmailPrimary { get; set; }
        [MaxLength(50)]
        public string? MainPocEmailSecondary { get; set; }

        // Event Secondary POC
        [MaxLength(50)]
        public string? SecondaryPocLastName { get; set; }
        [MaxLength(50)]
        public string? SecondaryPocFirstName { get; set; }
        [MaxLength(50)]
        public string? SecondaryPocRank { get; set; }
        [MaxLength(12)]
        public string? SecondaryPocPhonePrimary { get; set; }
        [MaxLength(12)]
        public string? SecondaryPocPhoneSecondary { get; set; }
        [MaxLength(50)]
        public string? SecondaryPocEmailPrimary { get; set; }
        [MaxLength(50)]
        public string? SecondaryPocEmailSecondary { get; set; }

        // AddtionalAlternate POC
        public bool AddAddtionalAlternatePoc { get; set; }

        [MaxLength(50)]
        public string? AddtionalAlternatePocLastName { get; set; }
        [MaxLength(50)]
        public string? AddtionalAlternatePocFirstName { get; set; }
        [MaxLength(50)]
        public string? AddtionalAlternatePocRank { get; set; }
        [MaxLength(12)]
        public string? AddtionalAlternatePocPhonePrimary { get; set; }
        [MaxLength(12)]
        public string? AddtionalAlternatePocPhoneSecondary { get; set; }
        [MaxLength(50)]
        public string? AddtionalAlternatePocEmailPrimary { get; set; }
        [MaxLength(50)]
        public string? AddtionalAlternatePocEmailSecondary { get; set; }
        [MaxLength(50)]
        public string? AddtionalAlternatePocRole { get; set; }

        //Shipping Address

        [MaxLength(200)]
        public string? ShippingAddressLine1 { get; set; } // Shipping Address Line 1

        [MaxLength(200)]
        public string? ShippingAddressLine2 { get; set; } // Shipping Address Line 2

        [MaxLength(100)]
        public string? ShippingAddressState { get; set; } // Shipping State

        [MaxLength(100)]
        public string? ShippingAddressCity { get; set; } // Shipping City

        [MaxLength(20)]
        public string? ShippingAddressZipCode { get; set; } // Shipping Zip Code

        //Shipping POC

        [MaxLength(50)]
        public string? ShippingPocLastName { get; set; }

        [MaxLength(50)]
        public string? ShippingPocRank { get; set; }

        [MaxLength(50)]
        public string? ShippingPocFirstName { get; set; }

        [MaxLength(12)]
        public string? ShippingPocPrimaryPhone { get; set; }

        [MaxLength(12)]
        public string? ShippingPocSecondaryPhone { get; set; }

        [MaxLength(50)]
        public string? ShippingPocPrimaryEmail { get; set; }

        [MaxLength(50)]
        public string? ShippingPocSecondaryEmail { get; set; }

        public TimeSpan? ShippingPocOpenAt { get; set; }

        public TimeSpan? ShippingPocCloseAt { get; set; }

        [MaxLength(300)]
        public string? ShippingPocInstruction { get; set; }

        public DateTime? ShippingPocPickupDate { get; set; }

        public TimeSpan? ShippingPocPickupTime { get; set; }

        public DateTime? ShippingPocDeliveryFromDate { get; set; }

        public DateTime? ShippingPocDeliveryToDate { get; set; }

        [MaxLength(50)]
        public string? ShippingPocSuggestedHourlyFlow { get; set; }
        [MaxLength(300)]
        public string? ShippingPocSpecialGateInstructions { get; set; }

        [MaxLength(300)]
        public string? ShippingPocParkingInstructions { get; set; }

        [MaxLength(3)] // For "Yes"/"No"
        public string? ShippingPocTablesAndChairsAvailable { get; set; }

        [MaxLength(3)] // For "Yes"/"No"
        public string? ShippingPocLocationSecured { get; set; }

        [MaxLength(3)] // For "Yes"/"No"
        public string? ShippingPocRefrigeratorAvailable { get; set; }

        [MaxLength(3)] // For "Yes"/"No"
        public string? ShippingPocLockableRefrigerator { get; set; }

        public DateTime? ShippingPocEventSetupDate { get; set; }

        public TimeSpan? ShippingPocEventSetupTime { get; set; }

        //Pharmacy

        [MaxLength(50)]
        public string? PharmacyName { get; set; }

        [MaxLength(200)]
        public string? PharmacyAddressLine1 { get; set; }

        [MaxLength(200)]
        public string? PharmacyAddressLine2 { get; set; }

        [MaxLength(50)]
        public string? PharmacyState { get; set; }

        [MaxLength(50)]
        public string? PharmacyCity { get; set; }

        [MaxLength(50)]
        public string? PharmacyZipCode { get; set; }

        [MaxLength(12)]
        public string? PharmacyPhoneNumber { get; set; }

        [MaxLength(3)] // For "Yes"/"No"
        public string? PharmacyMilitaryArrangement { get; set; }

        
        
        //POC for military contact (HIV Supply)

        [MaxLength(3)] // For "Yes"/"No"
        public string? HIVSuppliesNeeded { get; set; }
        [MaxLength(50)]
        public string? HIVSupplyMilitaryContactPOCLastName { get; set; }

        [MaxLength(50)]
        public string? HIVSupplyMilitaryContactPOCRank { get; set; }

        [MaxLength(50)]
        public string? HIVSupplyMilitaryContactPOCFirstName { get; set; }

        [MaxLength(12)]
        public string? HIVSupplyMilitaryContactPOCPhonePrimary { get; set; }

        [MaxLength(12)]
        public string? HIVSupplyMilitaryContactPOCPhoneSecondary { get; set; }

        [MaxLength(50)]
        public string? HIVSupplyMilitaryContactPOCEmailPrimary { get; set; }

        [MaxLength(50)]
        public string? HIVSupplyMilitaryContactPOCEmailSecondary { get; set; }



        //POC for military contact (Immunization vaccine Supply)

        [MaxLength(3)] // For "Yes"/"No"
        public string? ImmunizationVaccineNeeded { get; set; }

        [MaxLength(50)]
        public string? ImmVaccineSupplyMilitaryContactPOCLastName { get; set; }

        [MaxLength(50)]
        public string? ImmVaccineSupplyMilitaryContactPOCRank { get; set; }

        [MaxLength(50)]
        public string? ImmVaccineSupplyMilitaryContactPOCFirstName { get; set; }

        [MaxLength(12)]
        public string? ImmVaccineSupplyMilitaryContactPOCPhonePrimary { get; set; }

        [MaxLength(12)]
        public string? ImmVaccineSupplyMilitaryContactPOCPhoneSecondary { get; set; }

        [MaxLength(50)]
        public string? ImmVaccineSupplyMilitaryContactPOCEmailPrimary { get; set; }

        [MaxLength(50)]
        public string? ImmVaccineSupplyMilitaryContactPOCEmailSecondary { get; set; }

        //Quest pick up arrangement or drop off address


        [MaxLength(200)]
        public string? QuestPickupAddressLine1 { get; set; }

        [MaxLength(200)]
        public string? QuestPickupAddressLine2 { get; set; }

        [MaxLength(50)]
        public string? QuestPickupState { get; set; }

        [MaxLength(50)]
        public string? QuestPickupCity { get; set; }

        [MaxLength(50)]
        public string? QuestPickupZipCode { get; set; }


        //HIV Drop off address with who will drop off

        public long? HIVDropOffStaffId { get; set; }
        public string? StatusDescription { get; set; }
        public string? CompletedSections { get; set; }
        public List<EventServiceDetail>? EventServiceDetailList { get; set; } = new List<EventServiceDetail>();

        [Required]
        public List<EventStartEndTimeDayWise> EventStartEndTimeDayWiseList { get; set; } = new List<EventStartEndTimeDayWise>();

        public List<EventStaffDetail> EventStaffDetailList { get; set; } = new List<EventStaffDetail>();
        public List<EventManagementTaskforces> EventManagementTaskforcesList { get; set; } = new List<EventManagementTaskforces>();
    }

    [Table("EventServiceDetail")]
    public class EventServiceDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("EventManagement")]
        public long EventManagementId { get; set; }

        public string? EventService { get; set; }
        public bool IsSelected { get; set; }
        public string? Type { get; set; }
        public int? ClientRequestInitial { get; set; }

        public int? InitialReportNumbers { get; set; }

        public int? FinalPreEventConfirmedNumbers { get; set; }
        public bool IsConfirmed { get; set; }

    }

    [Table("EventStartEndTimeDayWise")]
    public class EventStartEndTimeDayWise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("EventManagement")]
        public long EventManagementId { get; set; }

        [Required]
        public int EventDay { get; set; }

        public TimeSpan? EventStartTime { get; set; }
        public TimeSpan? EventEndTime { get; set; }

        [Required]
        public int ServiceMemberPercentPerDay { get; set; }

    }

    [Table("EventStaffDetail")]
    public class EventStaffDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("EventManagement")]
        public long EventManagementId { get; set; }

        [Required]
        [ForeignKey("EventStaff")]
        public long EventStaffId { get; set; }


        [ValidateNever]
        [JsonIgnore]
        public EventStaff EventStaff { get; set; }
        [NotMapped]
        public List<string> SelectedRoles { get; set; } = new List<string>();
        public List<EventWiseStaffRole> EventWiseStaffRoleList { get; set; } = new List<EventWiseStaffRole>();

        [NotMapped]
        public List<string> SelectedSecondaryRoles { get; set; } = new List<string>();
        public List<EventWiseStaffSecondaryRole> EventWiseStaffSecondaryRoleList { get; set; } = new List<EventWiseStaffSecondaryRole>();

        [NotMapped]
        public List<DateTime>? AvailabilityDates { get; set; } // List of selected dates
        public List<EventManagementStaffAvailability> AvailabilityDatesList { get; set; } = new List<EventManagementStaffAvailability>();
        public bool PreEventAvailability { get; set; } // Pre-Event checkbox
        public bool ProfileButtonAccess { get; set; }
        public string? SelectedStation { get; set; } // Pre-Event checkbox
        [NotMapped]
        public List<string>? SelectedSecondaryStationList { get; set; } = new List<string>();
        public string? SelectedSecondaryStation { get; set; }
    }

    [Table("EventManagementStaffAvailability")]
    public class EventManagementStaffAvailability
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("EventStaffDetail")]
        public long EventStaffDetailId { get; set; }

        public DateTime AvailabilityDate { get; set; }
    }

    [Table("EventWiseStaffRole")]
    public class EventWiseStaffRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("EventStaffDetail")]
        public long EventStaffDetailId { get; set; }

        [Required]
        public string RoleId { get; set; }
    }

    [Table("EventWiseStaffSecondaryRole")]
    public class EventWiseStaffSecondaryRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("EventStaffDetail")]
        public long EventStaffDetailId { get; set; }

        [Required]
        public string RoleId { get; set; }
    }

    [Table("EventManagementTaskforces")]
    public class EventManagementTaskforces
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("EventManagement")]
        public long EventManagementId { get; set; }

        [MaxLength(100)]
        public string? Taskforce { get; set; }
    }
}

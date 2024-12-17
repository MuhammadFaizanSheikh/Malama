using ExcelFilesCompiler.Models;
using System.ComponentModel.DataAnnotations;

namespace ExcelToCsv.Models
{
    public class EventStaffDto
    {
        //************************************************SectionB**********************************************
        public int StaffID { get; set; }
        public DateTime StartDate { get; set; }
        public string StaffStatus { get; set; }
        public string StaffLastName { get; set; }
        public string StaffFirstName { get; set; }
        public string StaffMiddleInitial { get; set; }
        public string StaffSSN { get; set; }
        public DateTime StaffDOB { get; set; }
        public List<string> StaffRoles { get; set; } = new List<string>();
        public List<string> RolesList { get; set; } = new List<string>(); // For checkboxes
        public string EventOnCallStaff { get; set; }
        public string SelectedEvent { get; set; }
        public List<LicenseInfoDTO> Licenses { get; set; } = new List<LicenseInfoDTO>();
        public string NPI { get; set; }
        public string DAE { get; set; }
        public DateTime CredentialingProcessDate { get; set; }
        public DateTime HistoricalCredentialingDate { get; set; }
        public DateTime? HistoricalCredentialingDateOptional { get; set; }
        public DateTime DAWSONInternalCredentialingCompleteDate { get; set; }
        public string OnboardingTrainingComplete { get; set; }
        public string OutstandingTrainings { get; set; }
        public string BackgroundCheckConcerns { get; set; }
        public DateTime? BLSCertDate { get; set; }
        public string BLSCertNumber { get; set; }
        public DateTime? ACLSCertDate { get; set; }
        public string ACLSCertNumber { get; set; }

        //************************************************SectionB**********************************************

        public string CACApplicationProcessStatus { get; set; }  // Dropdown options: In Progress, Pending, Complete, Not Started
        public bool StaffCAC { get; set; }  // Radio buttons: Yes (true), No (false)
        public string StaffDoDID { get; set; }  // Textbox
        public string EmployerSubcontractor { get; set; }  // Dropdown: Employer, Subcontractor
        public string StaffCellNumber { get; set; }  // Textbox
        public string StaffPhone2 { get; set; }  // Textbox (optional)
        public string StaffEmail { get; set; }  // Textbox

        // Primary Residence fields
        public string PrimaryAddress1 { get; set; }  // Textbox
        public string PrimaryAddress2 { get; set; }  // Textbox
        public string PrimaryCity { get; set; }  // Textbox
        public string PrimaryState { get; set; }  // Textbox
        public string PrimaryZip { get; set; }  // Textbox

        // Secondary Residence fields
        public string SecondaryAddress1 { get; set; }  // Textbox
        public string SecondaryAddress2 { get; set; }  // Textbox
        public string SecondaryCity { get; set; }  // Textbox
        public string SecondaryState { get; set; }  // Textbox
        public string SecondaryZip { get; set; }  // Textbox

        public List<string> StaffContractAffiliation { get; set; }

        // Staff info entered by (readonly field)
        public string StaffInfoEnteredBy { get; set; }  // Readonly label

        // Travel Honor Reward Numbers fields
        public List<Airline> Airlines { get; set; } = new List<Airline>();  // List of Airlines

        public bool TravelHonorCar { get; set; }  // Radio button for Car
        public string CarRentalCompany { get; set; }  // Textbox for Rental Company Name
        public string CarRentalRewards { get; set; }  // Textbox for Rental Company Rewards

        public bool TravelHonorHotel { get; set; }  // Radio button for Hotel
        public string HotelName { get; set; }  // Textbox for Hotel Name
        public string HotelRewards { get; set; }  // Textbox for Hotel Rewards
    }

    public class LicenseInfoDTO
    {
        public string LicenseNumber { get; set; }
        public string LicenseState { get; set; }
        public string LicenseType { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
    }

    public class Airline
    {
        public string AirlineName { get; set; }  // Airline name
        public string AirlineRewards { get; set; }  // Rewards associated with the airline
        public bool TravelHonorAir { get; set; }  // Represents whether this entry is selected via the radio button
    }
}

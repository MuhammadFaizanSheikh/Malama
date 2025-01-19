namespace ExcelFilesCompiler.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EventManagementViewModel
    {
        public List<EventManagement>? EventManagements { get; set; }
        public EventManagement SingleEventManagement { get; set; }
    }

    [Table("EventManagement")]
    public class EventManagement : GenericProperties
    {
        public int Id { get; set; } // Used for hidden field

        // Event Specific Details
        public string EventID { get; set; } // Read-only field
        public string SubEventID { get; set; }
        public string TaskForce { get; set; }
        public string EventStatus { get; set; }
        public string EventAddress1 { get; set; }
        public string EventAddress2 { get; set; }
        public string EventState { get; set; } // Dropdown
        public string EventCity { get; set; } // Dropdown
        public string EventZipCode { get; set; } // Dropdown
        public DateTime? EventStartDate { get; set; } // Calendar
        public DateTime? EventEndDate { get; set; } // Calendar
        public TimeSpan? EventStartTimeDay1 { get; set; } // Time only
        public TimeSpan? EventEndTimeDay1 { get; set; } // Time only
        public string Deploy { get; set; } // Radio button, "Deploying" or "Non-Deploying"
        public DateTime? MOBDate { get; set; } // Calendar
        public string RegardingSites { get; set; } // Multiline textbox

        // Event Main POC
        public string MainPocLastName { get; set; }
        public string MainPocFirstName { get; set; }
        public string MainPocRank { get; set; }
        public string MainPocPhonePrimary { get; set; }
        public string MainPocPhoneSecondary { get; set; }
        public string MainPocEmailPrimary { get; set; }
        public string MainPocEmailSecondary { get; set; }

        // Event Secondary POC
        public string SecondaryPocLastName { get; set; }
        public string SecondaryPocFirstName { get; set; }
        public string SecondaryPocRank { get; set; }
        public string SecondaryPocPhonePrimary { get; set; }
        public string SecondaryPocPhoneSecondary { get; set; }
        public string SecondaryPocEmailPrimary { get; set; }
        public string SecondaryPocEmailSecondary { get; set; }
    }
}

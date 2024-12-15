namespace ExcelFilesCompiler.Models
{
    public class ContractDetailsDto
    {
        // Contract Details Properties
        public int ContractID { get; set; }
        public string ContractAgency { get; set; }
        public string ContractServiceBranch { get; set; }
        public string ContractComponent { get; set; }
        public string ContractClient { get; set; }
        public string ContractType { get; set; }
        public string DawsonRoleOnContract { get; set; }
        public string ContractStatus { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }

        // Contract Officer (KO) Properties
        public string LastName { get; set; } // Required
        public string FirstName { get; set; } // Required
        public string KOPhone { get; set; } // Required
        public string KOPhone2 { get; set; } // Required
        public string KOEmail { get; set; } // Required
        public string KONotes { get; set; } // Required

        public string CORLastName { get; set; }
        public string CORFirstName { get; set; }
        public string CORKORank { get; set; }
        public string CORPhone { get; set; }
        public string CORPhone2 { get; set; }
        public string COREmail { get; set; }
        public string CORNotes { get; set; }

        public string DawsonProgramManagerLastName { get; set; }
        public string DawsonProgramManagerFirstName { get; set; }


        public string DawsonDeputyProgramManagerLastName { get; set; }
        public string DawsonDeputyProgramManagerFirstName { get; set; }


        public string DawsonProjectManagerLastName { get; set; }
        public string DawsonProjectManagerFirstName { get; set; }

    }
}

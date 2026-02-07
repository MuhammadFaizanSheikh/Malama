namespace Malama.Utilities
{
    public static class RoleAttributeConfig
    {
        public static readonly Dictionary<string, List<(string Role, string Attribute)>> RoleAttributeCombinations
        = new()
        {
            ["AccountRegistration_View"] = new List<(string Role, string Attribute)>
        {
            ("Super Admin", null),
        },
            ["AccountUser_View"] = new List<(string Role, string Attribute)>
        {
            ("Super Admin", null),
        },
            ["AccountRegistration_Save"] = new List<(string Role, string Attribute)>
        {
            ("Super Admin", null),
        },
            ["ContractDetails_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
        },
            ["ContractDetails_Save"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
        },
            ["SubContractorInfo_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
        },
            ["SubContractorInfo_Save"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
        },
            ["EventStaff_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
            ("DAWSON Admin - Staffing/Credentialing", null),
        },
            ["EventStaff_Save"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
            ("DAWSON Admin - Staffing/Credentialing", null),
        },
            ["ReportProcessor_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
            ["ReportProcessor_Save"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
            ["EventManagement_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
            ("Event Manager", null)
        },
            ["EventManagement_Save"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
            ("Event Manager", null)
        },
            ["Profile_View"] = new List<(string Role, string Attribute)>
        {
            ("Check in/out Staff", "CanAccessProfile")
        },
            ["LabStation_HIVSignInSheet_View"] = new List<(string Role, string Attribute)>
        {
            ("Event Manager", null)
        },
            ["CheckInOutStaff_View"] = new List<(string Role, string Attribute)>
        {
            ("Check in/out Staff", null),
            ("Super Admin", null),
            ("Event Manager", null)
        },
            ["CheckInOutStaff_Client_View"] = new List<(string Role, string Attribute)>
        {
            ("Check in/out Staff", null),
            ("Event Manager", null)
        },
            ["CheckInOutStaff_Admin_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
            // Audiologist
            ["Audiologist_View"] = new List<(string Role, string Attribute)>
        {
            ("Audiologist", null),
            ("Event Manager", null)
        },

            // Dental Exams
            ["DentalExams_View"] = new List<(string Role, string Attribute)>
        {
            ("DE- Dentist", "Exam"),
            ("DE-Dental X-Ray Tech", "Exam"),
            ("DE-Dental Assistant", "Exam"),
            ("DE-Dental Lead", "Exam"),
            ("DE-Dental Director", "Exam"),
            ("DT-Treatment Coordinator", "Treatment"),
            ("DT-Dentist", "General"),
            ("DT-Dentist", "Oral Surgery"),
            ("DT-Dentist", "Endo"),
            ("DT-Dental Assistant", "Treatment"),
            ("DT-Dental Director", "Treatment"),
            ("Event Manager", null)
        },

            // Dental Treatment
            ["DentalTreatment_View"] = new List<(string Role, string Attribute)>
        {
            ("DE-Dental X-Ray Tech", "Exam"),
            ("DE-Dental Director", "Exam"),
            ("DT-Treatment Coordinator", "Treatment"),
            ("DT-Dentist", "General"),
            ("DT-Dentist", "Oral Surgery"),
            ("DT-Dentist", "Endo"),
            ("DT-Dental Assistant", "Treatment"),
            ("DT-Dental Director", "Treatment"),
            ("Event Manager", null)
        },

            // Dental X-Ray
            ["DentalXRay_View"] = new List<(string Role, string Attribute)>
        {
            ("DE- Dentist", "Exam"),
            ("DE-Dental X-Ray Tech", "Exam"),
            ("DE-Dental Assistant", "Exam"),
            ("DE-Dental Lead", "Exam"),
            ("DE-Dental Director", "Exam"),
            ("DT-Treatment Coordinator", "Treatment"),
            ("DT-Dentist", "General"),
            ("DT-Dentist", "Oral Surgery"),
            ("DT-Dentist", "Endo"),
            ("DT-Dental Assistant", "Treatment"),
            ("DT-Dental Director", "Treatment"),
            ("Event Manager", null)
        },

            // EKG
            ["EKG_View"] = new List<(string Role, string Attribute)>
        {
            ("EKG Staff", null),
            ("Event Manager", null)
        },

            // Hearing
            ["Hearing_View"] = new List<(string Role, string Attribute)>
        {
            ("Audio Tech", null),
            ("Audiologist", null),
            ("Event Manager", null)
        },

            // Immunization forms
            ["ImmunizationStation_View"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Imms"),
            ("Imm Staff", "Cold Chain Cert"),
            ("Event Manager", null)
        },
            ["ImmunizationVaccineInfo_View"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Imms"),
            ("Imm Staff", "Cold Chain Cert"),
            ("Event Manager", null)
        },
            ["Container_View"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Imms"),
            ("Imm Staff", "Cold Chain Cert"),
            ("Event Manager", null)
        },

            // Labs
            ["LabStation_View"] = new List<(string Role, string Attribute)>
        {
            ("Lab Staff", null),
            ("Lab Admin", null),
            ("Event Manager", null)
        },

            // Optometrist
            ["Optometrist_View"] = new List<(string Role, string Attribute)>
        {
            ("Optometrist", null),
            ("Event Manager", null)
        },

            // Panoramic Dental X-Ray
            ["PanoramicDentalXRay_View"] = new List<(string Role, string Attribute)>
        {
            ("DE- Dentist", "Exam"),
            ("DE-Dental X-Ray Tech", "Exam"),
            ("DE-Dental Assistant", "Exam"),
            ("DE-Dental Lead", "Exam"),
            ("DE-Dental Director", "Exam"),
            ("DT-Treatment Coordinator", "Treatment"),
            ("DT-Dentist", "General"),
            ("DT-Dentist", "Oral Surgery"),
            ("DT-Dentist", "Endo"),
            ("DT-Dental Assistant", "Treatment"),
            ("DT-Dental Director", "Treatment"),
            ("Panorex X-ray", "Panorex X-ray"),
            ("Event Manager", null)
        },

            // es Review
            ["RecordsReview_View"] = new List<(string Role, string Attribute)>
        {
            ("PHA-Record Review", null),
            ("PHA Provider", null),
            ("Event Manager", null)
        },

            // Vision Screening
            ["VisionScreening_View"] = new List<(string Role, string Attribute)>
        {
            ("Vitals Staff", null),
            ("Vision Tech", null),
            ("Optometrist", null),
            ("Event Manager", null)
        },

            // Vitals
            ["Vitals_View"] = new List<(string Role, string Attribute)>
        {
            ("Vitals Staff", null),
            ("Event Manager", null)
        },
            ["ImmunizationStation_Save"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Imms"),
            ("Imm Staff", "Cold Chain Cert"),
            ("Event Manager", null)
        },

            ["ImmunizationVaccineInfo_Save"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Event Manager", null)
        },

            ["Container_Save"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Cold Chain Cert"),
            ("Event Manager", null)
        },
            
            ["CheckInOutStaff_Save"] = new List<(string Role, string Attribute)>
        {
            ("Check in/out Staff", null),
            ("Super Admin", null),
            ("Event Manager", null)
        },
            ["LabStation_Save"] = new List<(string Role, string Attribute)>
        {
            ("Lab Staff", null),
            ("Lab Admin", null),
            ("Event Manager", null)
        }

        };
    }

}

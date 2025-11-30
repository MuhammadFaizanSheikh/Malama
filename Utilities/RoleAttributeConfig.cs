namespace Malama.Utilities
{
    public static class RoleAttributeConfig
    {
        public static readonly Dictionary<string, List<(string Role, string Attribute)>> RoleAttributeCombinations
        = new()
        {
            // Admin page
            ["AccountRegistration_View"] = new List<(string Role, string Attribute)>
        {
            ("Super Admin", null),
            ("Project Manager & Program Manager", null)
        },

            // Malama Check In Out
            ["CheckInOutStaff_View"] = new List<(string Role, string Attribute)>
        {
            ("Check in/out Staff", null)
        },

            // Audiologist
            ["Audiologist_View"] = new List<(string Role, string Attribute)>
        {
            ("Audiologist", null)
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
            ("DT-Dental Director", "Treatment")
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
            ("DT-Dental Director", "Treatment")
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
            ("DT-Dental Director", "Treatment")
        },

            // EKG
            ["EKG_View"] = new List<(string Role, string Attribute)>
        {
            ("EKG Staff", null)
        },

            // Hearing
            ["Hearing_View"] = new List<(string Role, string Attribute)>
        {
            ("Audio Tech", null),
            ("Audiologist", null)
        },

            // Immunization forms
            ["ImmunizationStation_View"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Imms"),
            ("Imm Staff", "Cold Chain Cert")
        },
            ["ImmunizationVaccineInfo_View"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Imms"),
            ("Imm Staff", "Cold Chain Cert")
        },
            ["Container_View"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Imms"),
            ("Imm Staff", "Cold Chain Cert")
        },

            // Labs
            ["Labs_View"] = new List<(string Role, string Attribute)>
        {
            ("Lab Staff", null),
            ("Lab Admin", null)
        },

            // Optometrist
            ["Optometrist_View"] = new List<(string Role, string Attribute)>
        {
            ("Optometrist", null)
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
            ("Panorex X-ray", "Panorex X-ray")
        },

            // es Review
            ["RecordsReview_View"] = new List<(string Role, string Attribute)>
        {
            ("PHA-Record Review", null),
            ("PHA Provider", null)
        },

            // Vision Screening
            ["VisionScreening_View"] = new List<(string Role, string Attribute)>
        {
            ("Vitals Staff", null),
            ("Vision Tech", null),
            ("Optometrist", null)
        },

            // Vitals
            ["Vitals_View"] = new List<(string Role, string Attribute)>
        {
            ("Vitals Staff", null)
        },
            ["ImmunizationStation_Save"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Imms"),
            ("Imm Staff", "Cold Chain Cert")
        },

            ["ImmunizationVaccineInfo_Save"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert")
        },

            ["Container_Save"] = new List<(string Role, string Attribute)>
        {
            ("Imm RN", "Cold Chain Cert"),
            ("Imm Staff", "Cold Chain Cert")
        }

        };
    }

}

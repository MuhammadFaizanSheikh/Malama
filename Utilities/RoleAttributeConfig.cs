using Malama.Models;

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
            ("DAWSON Admin - System", null)
        },
            ["DawsonUser_View"] = new List<(string Role, string Attribute)>
        {
            ("Super Admin", null),
            ("DAWSON Admin - System", null)
        },
            ["EventUser_View"] = new List<(string Role, string Attribute)>
        {
             ("Project Manager & Program Manager", null),
             ("Event Manager", null)
        },
            ["AccountRegistration_Save"] = new List<(string Role, string Attribute)>
        {
            ("Super Admin", null),
            ("DAWSON Admin - System", null)
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
            ["EventManagement_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
            ("Event Manager", null)
        },
            ["EventManagement_Add"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
        
            ["EventManagement_Update"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
            ("Event Manager", null)
        },
            ["LabStation_HIVSignInSheet_View"] = new List<(string Role, string Attribute)>
        {
            ("Event Manager", null)
        },
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
            ["DentalExams_Save"] = new List<(string Role, string Attribute)>
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
            ["DentalXRay_Save"] = new List<(string Role, string Attribute)>
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

            // Vitals (dental exam roles included so Dental X-Ray workflow can redirect here for BP)
            ["VitalStation_View"] = new List<(string Role, string Attribute)>
        {
            ("Vitals Staff", null),
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
            ["VitalStation_Save"] = new List<(string Role, string Attribute)>
        {
            ("Vitals Staff", null),
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
            ["PostEventManagement_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
            ["PostEventDataAnalysis_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
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
            ["LabStation_Save"] = new List<(string Role, string Attribute)>
        {
            ("Lab Staff", null),
            ("Lab Admin", null),
            ("Event Manager", null)
        },
            ["Profile_View"] = new List<(string Role, string Attribute)>
        {
            ("Check in/out Staff", "CanAccessProfile")
        },


            ["CheckInOutStaff_View"] = new List<(string Role, string Attribute)>
        {
            ("Check in/out Staff", null),
            ("Event Manager", null)
        },
            ["ScrubbedSheetUploader_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
            ["ReportProcessor_View"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
            ["CheckInOutStaff_Save"] = new List<(string Role, string Attribute)>
        {
            ("Check in/out Staff", null),
            ("Event Manager", null)
        },
            ["ScrubbedSheetUploader_Save"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
            ["ReportProcessor_Save"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
            ["PostEventManagement_Save"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null)
        },
            ["PostEventDataAnalysis_Save"] = new List<(string Role, string Attribute)>
        {
            ("Project Manager & Program Manager", null),
            ("Event Manager", null)
        }
        };

        public static readonly Dictionary<string, string> PageNames = new()
        {
            ["AccountRegistration"] = "Account Registration",
            ["AccountUser"] = "Account User Register",
            ["ContractDetails"] = "Contract Details",
            ["SubContractorInfo"] = "Sub-Contractor Information",
            ["EventStaff"] = "Event Staff",
            ["ReportProcessor"] = "Report Processor",
            ["EventManagement"] = "Event Management",
            ["Profile"] = "User Profile",
            ["LabStation_HIVSignInSheet"] = "HIV Sign-In Sheet",
            ["ScrubbedSheetUploader"] = "Scrubbed Sheet Uploader",
            ["CheckInOutStaff"] = "Check-In/Out Staff",
            ["Audiologist"] = "Audiologist",
            ["DentalExams"] = "Dental Exams",
            ["DentalTreatment"] = "Dental Treatment",
            ["DentalXRay"] = "Dental X-Ray",
            ["EKG"] = "EKG",
            ["Hearing"] = "Hearing",
            ["ImmunizationStation"] = "Immunization Station",
            ["ImmunizationVaccineInfo"] = "Vaccine Information",
            ["Container"] = "Vaccine Container",
            ["LabStation"] = "Lab Station",
            ["Optometrist"] = "Optometrist",
            ["PanoramicDentalXRay"] = "Panoramic Dental X-Ray",
            ["RecordsReview"] = "Records Review",
            ["VisionScreening"] = "Vision Screening",
            ["VitalStation"] = "Vitals",
            ["PostEventManagement"] = "Post Event Management",
            ["PostEventDataAnalysis"] = "Post Event Data Analysis"
        };

        public static class PagePermissionResolver
        {
            public static List<UserPagePermissionDto> ResolveUserPages(
                IEnumerable<string> userRoles,
                IEnumerable<string>? userAttributes = null)
            {
                var result = new Dictionary<string, UserPagePermissionDto>();

                foreach (var entry in RoleAttributeConfig.RoleAttributeCombinations)
                {
                    var feature = entry.Key;
                    var allowed = entry.Value;

                    var splitIndex = feature.LastIndexOf('_');
                    if (splitIndex <= 0)
                        continue;

                    var pageKey = feature[..splitIndex];
                    var action = feature[(splitIndex + 1)..]; // View / Save / Add / Update

                    var isAllowed = allowed.Any(a =>
                        userRoles.Contains(a.Role) &&
                        (
                            a.Attribute == null ||
                            userAttributes == null ||
                            userAttributes.Contains(a.Attribute)
                        )
                    );

                    if (!isAllowed)
                        continue;

                    if (!result.TryGetValue(pageKey, out var page))
                    {
                        result[pageKey] = page = new UserPagePermissionDto
                        {
                            PageKey = pageKey,
                            PageName = RoleAttributeConfig.PageNames.TryGetValue(pageKey, out var name)
                                ? name
                                : pageKey,
                            Access = PageAccessLevel.None
                        };
                    }

                    if (action.Equals("View", StringComparison.OrdinalIgnoreCase))
                    {
                        page.Access |= PageAccessLevel.View;
                    }
                    else if (action.Equals("Save", StringComparison.OrdinalIgnoreCase))
                    {
                        // Legacy: Save = Add + Update
                        page.Access |= PageAccessLevel.Add;
                        page.Access |= PageAccessLevel.Update;
                    }
                    else if (action.Equals("Add", StringComparison.OrdinalIgnoreCase))
                    {
                        page.Access |= PageAccessLevel.Add;
                    }
                    else if (action.Equals("Update", StringComparison.OrdinalIgnoreCase))
                    {
                        page.Access |= PageAccessLevel.Update;
                    }
                }

                return result.Values.ToList();
            }
        }

    }
}

[Flags]
public enum PageAccessLevel
{
    None = 0,
    View = 1,
    Add = 2,
    Update = 4
}



//*************************************************this is the line where edit work was completed and tested********************************************************
const keys = [
    "Actions", "SM ID", "FULL NAME", "FULL SSN", "LAST 4", "DOD ID", "RANK", "AGE", "SEX", "MOS",
    "AGR", "UIC", "MRC", "DOB", "OVER 40", "Dental Due", "Dental Exam", "Dental Needed",
    "PANO Needed", "BWX Needed", "DRC", "PHA Date", "PHA Due", "PHA Needed", "PULHES", "Vision Date",
    "VISION Needed", "NEAR VISION Needed", "VRC", "Vision 2PG", "Vision Mask Insert", "Hearing Date", "HEARING Needed",
    "HRC", "Hearing Profile", "Lab Requisition", "Lab Needed", "ABO", "ABO Needed", "DNA", "Sickle Date",
    "SICKLE", "G6PD", "G6PD Date", "G6PD Status", "HIV NEXT TEST DATE", "HIV", "Lipid Needed",
    "LIPID PANEL", "Cholesterol / HDL Cholesterol", "Framingham", "EKG (Date)", "EKG NEEDED", "Pregnancy Test Needed",
    "IMM Needed", "Hep B Needed", "Hep A Needed", "FLU Needed", "Tet/TDP Needed", "MMR Needed", "Varicella Needed", "TaskForce", "Notes", "Over 44",
    "EventDate", "Event End Date", "EventID", "Vision Win", "Dental Win", "PHA Win", "HIV Win", "Hearing WIN", "Barcode",
    "Vitals Status", "Hep B Needed Status", "Hep B Reason", "FLU Needed Status", "FLU Reason", "MMR Status", "MMR Reason", "Hep A Needed Status", "Hep A Reason", "Tet/TDP Needed Status", "Tet/TDP Reason", "Varicella Needed Status", "Varicella Reason", "SICKLE Status", "Sickle Reason", "HIV Status", "HIV Barcode", "HIV Reason", "ABO Status", "ABO Reason", "DNA Status", "DNA Reason", "G6PD Checkout Status", "G6PD Reason", "Lipid Status", "Lipid Reason", "EKG Needed Status", "EKG Needed Reason", "PHA Status", "PHA Follow Up", "Hearing Status", "Audiologist Service Completed", "Vision Status", "Optometrist Service Completed", "Dental X-Ray Status", "Panoramic X-Ray", "Dental Exam Status", "Class (Dental Exam)", "Dental Treatment", "Final Dental Class", "Dental Treatment Reason",
    "Checked In", "Checked Out", "Checked In By",
    "Checked Out By", "Checked In Time", "Checked Out Time", "Walk-In Service Member"
];

const tableToKeysIndexMap = [
    keys.indexOf("FULL NAME"),      // FULL NAME 
    keys.indexOf("AGE"),            // AGE
    keys.indexOf("UIC"),            // UIC
    keys.indexOf("FULL SSN"),         // LAST 4Final Dental Class
    keys.indexOf("SEX"),            // SEX
    keys.indexOf("MRC"),            // MRC
    keys.indexOf("DOD ID"),         // DOD ID
    keys.indexOf("MOS"),            // MOS
    keys.indexOf("DOB"),            // DOB
    keys.indexOf("RANK"),           // RANK
    keys.indexOf("AGR"),            // AGR
    keys.indexOf("OVER 40"),        // OVER 40

    // Taskforce Information
    keys.indexOf("TaskForce"),      // TaskForce
    keys.indexOf("EventID"),        // EventID
    keys.indexOf("Over 44"),        // Over 44
    keys.indexOf("EventDate"),      // EventDate
    keys.indexOf("Event End Date"), // Event End Date
    keys.indexOf("Vision Win"),     // Vision_Win
    keys.indexOf("PHA Win"),        // PHA_Win
    keys.indexOf("Hearing WIN"),    // Hearing_WIN
    keys.indexOf("Dental Win"),     // Dental_Win
    keys.indexOf("HIV Win"),        // HIV_Win
    keys.indexOf("Notes"),          // Notes

    // Dental Information
    keys.indexOf("Dental Due"),     // Dental Due
    keys.indexOf("PANO Needed"),    // PANO Needed
    keys.indexOf("Dental Needed"),  // Dental Needed
    keys.indexOf("Dental Exam"),    // Dental Exam
    keys.indexOf("BWX Needed"),     // BWX Needed
    keys.indexOf("DRC"),            // DRC

    // PHA Information
    keys.indexOf("PHA Date"),       // PHA Date
    keys.indexOf("PHA Needed"),            // PHA
    keys.indexOf("PULHES"),         // PULHES
    keys.indexOf("PHA Due"),        // PHA Due

    // Vision Information
    keys.indexOf("Vision Date"),    // Vision Date
    keys.indexOf("NEAR VISION Needed"),    // NEAR VISION
    keys.indexOf("Vision 2PG"),     // Vision 2PG
    keys.indexOf("VISION Needed"),         // VISION
    keys.indexOf("VRC"),            // VRC
    keys.indexOf("Vision Mask Insert"),     // Vision 1MI

    // Hearing Information
    keys.indexOf("Hearing Date"),   // Hearing Date
    keys.indexOf("HEARING Needed"),        // HEARING
    keys.indexOf("HRC"),            // HRC
    keys.indexOf("Hearing Profile"),// Hearing Profile

    // Lab Information
    keys.indexOf("Sickle Date"),    // Sickle Date
    keys.indexOf("HIV NEXT TEST DATE"), // HIV NEXT TEST DATE
    keys.indexOf("ABO"),            // ABO
    keys.indexOf("SICKLE"),         // SICKLE
    keys.indexOf("HIV"),            // HIV
    keys.indexOf("ABO Needed"),     // ABO Needed
    keys.indexOf("Lab Requisition"),          // QUEST
    keys.indexOf("Lab Needed"),     // Lab Needed
    keys.indexOf("DNA"),            // DNA
    keys.indexOf("G6PD"),           // G6PD
    keys.indexOf("G6PD Date"),      // G6PD Date
    keys.indexOf("G6PD Status"),    // G6PD Status
    keys.indexOf("Lipid Needed"),   // Lipid Needed
    keys.indexOf("Cholesterol / HDL Cholesterol"), // Cholesterol / HDL Cholesterol
    keys.indexOf("EKG (Date)"),            // EKG
    keys.indexOf("LIPID PANEL"),    // LIPID PANEL
    keys.indexOf("Framingham"),     // Framingham
    keys.indexOf("EKG NEEDED"),     // EKG NEEDED
    keys.indexOf("Pregnancy Test Needed"),            // Pregnancy Test Needed
    // Immunization information
    keys.indexOf("IMM Needed"),            // IMM
    keys.indexOf("Hep B Needed"),          // Hep B
    keys.indexOf("FLU Needed"),            // FLU
    keys.indexOf("MMR Needed"),            // MMR
    keys.indexOf("Hep A Needed"),          // Hep A
    keys.indexOf("Tet/TDP Needed"),        // Tet/TDP
    keys.indexOf("Varicella Needed"),       // Varicella
    keys.indexOf("Checked In"),
    keys.indexOf("Checked Out"),
    keys.indexOf("Checked In By"),
    keys.indexOf("Checked Out By"),
    keys.indexOf("Vitals Status"),
    keys.indexOf("Hep B Needed Status"),
    keys.indexOf("Hep B Reason"),
    keys.indexOf("FLU Needed Status"),
    keys.indexOf("FLU Reason"),
    keys.indexOf("MMR Status"),
    keys.indexOf("MMR Reason"),
    keys.indexOf("Hep A Needed Status"),
    keys.indexOf("Hep A Reason"),
    keys.indexOf("Tet/TDP Needed Status"),
    keys.indexOf("Tet/TDP Reason"),
    keys.indexOf("Varicella Needed Status"),
    keys.indexOf("Varicella Reason"),
    keys.indexOf("SICKLE Status"),
    keys.indexOf("Sickle Reason"),
    keys.indexOf("HIV Status"),
    keys.indexOf("HIV Barcode"),
    keys.indexOf("HIV Reason"),
    keys.indexOf("ABO Status"),
    keys.indexOf("ABO Reason"),
    keys.indexOf("DNA Status"),
    keys.indexOf("DNA Reason"),
    keys.indexOf("G6PD Checkout Status"),
    keys.indexOf("G6PD Reason"),
    keys.indexOf("Lipid Status"),
    keys.indexOf("Lipid Reason"),
    keys.indexOf("EKG Needed Status"),
    keys.indexOf("EKG Needed Reason"),
    keys.indexOf("PHA Status"),
    keys.indexOf("PHA Follow Up"),
    keys.indexOf("Hearing Status"),
    keys.indexOf("Audiologist Service Completed"),
    keys.indexOf("Vision Status"),
    keys.indexOf("Optometrist Service Completed"),
    keys.indexOf("Dental X-Ray Status"),
    keys.indexOf("Panoramic X-Ray"),
    keys.indexOf("Dental Exam Status"),
    keys.indexOf("Class (Dental Exam)"),
    keys.indexOf("Dental Treatment"),
    keys.indexOf("Final Dental Class"),
    keys.indexOf("Dental Treatment Reason")

];

function getCellValue($cells, columnName) {
    const index = keys.indexOf(columnName);
    return index !== -1 ? $cells.eq(index).text() : '';
}



const categories = {
    "Personal Information": [
        "FULL NAME", "AGE", "UIC", "FULL SSN", "SEX", "MRC", "DOD ID", "MOS", "DOB", "RANK", "AGR", "OVER 40"
    ],
    "Taskforce Information": [
        "TaskForce", "EventID", "Over 44", "EventDate", "Event End Date", "Vision Win", "PHA Win",
        "Hearing WIN", "Dental Win", "HIV Win", "Notes"
    ],
    "Dental Information": [
        "Dental Due", "PANO Needed", "Dental Needed", "Dental Exam", "BWX Needed", "DRC"
    ],
    "PHA Information": [
        "PHA Date", "PHA Needed", "PULHES", "PHA Due"
    ],
    "Vision Information": [
        "Vision Date", "NEAR VISION Needed", "Vision 2PG", "VISION Needed", "VRC", "Vision Mask Insert"
    ],
    "Hearing Information": [
        "Hearing Date", "HEARING Needed", "HRC", "Hearing Profile"
    ],
    "Lab Information": [
        "Sickle Date", "HIV NEXT TEST DATE", "ABO", "SICKLE", "HIV", "ABO Needed", "Lab Requisition", "Lab Needed", "DNA", "G6PD", "G6PD Date", "G6PD Status", "Lipid Needed", "Cholesterol / HDL Cholesterol"
        , "EKG (Date)", "LIPID PANEL", "Framingham", "EKG NEEDED", "Pregnancy Test Needed"
    ],
    "Immunization Information": [
        "IMM Needed", "Hep B Needed", "FLU Needed", "MMR Needed", "Hep A Needed", "Tet/TDP Needed", "Varicella Needed"
    ],
    "Check In Out Information": [
        "Checked In", "Checked Out", "Checked In By", "Checked Out By"
    ],
    "Check Out Section": {
        "Vitals": [
            [
                {
                    field: "Vitals Status",
                    dependsOn: null,
                    showWhen: null
                }
            ]
        ],
        "Immunizations": [
            [
                { field: "Hep B Needed Status", dependsOn: "Hep B Needed", showWhen: ["NEEDED"] },
                { field: "Hep B Reason", dependsOn: "Hep B Needed Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "FLU Needed Status", dependsOn: "FLU Needed", showWhen: ["NEEDED"] },
                { field: "FLU Reason", dependsOn: "FLU Needed Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "MMR Status", dependsOn: "MMR Needed", showWhen: ["NEEDED"] },
                { field: "MMR Reason", dependsOn: "MMR Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "Hep A Needed Status", dependsOn: "Hep A Needed", showWhen: ["NEEDED"] },
                { field: "Hep A Reason", dependsOn: "Hep A Needed Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "Tet/TDP Needed Status", dependsOn: "Tet/TDP Needed", showWhen: ["NEEDED"] },
                { field: "Tet/TDP Reason", dependsOn: "Tet/TDP Needed Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "Varicella Needed Status", dependsOn: "Varicella Needed", showWhen: ["NEEDED"] },
                { field: "Varicella Reason", dependsOn: "Varicella Needed Status", showWhen: ["Not Completed"] }
            ]
        ],
        "Labs": [
            [
                { field: "SICKLE Status", dependsOn: "SICKLE", showWhen: ["NEEDED"] },
                { field: "Sickle Reason", dependsOn: "SICKLE Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "HIV Status", dependsOn: "HIV", showWhen: ["NEEDED"] },
                { field: "HIV Reason", dependsOn: "HIV Status", showWhen: ["Not Completed"] },
                { field: "HIV Barcode", dependsOn: "HIV Status", showWhen: ["Completed"] }
            ],
            [
                { field: "ABO Status", dependsOn: "ABO Needed", showWhen: ["NEEDED"] },
                { field: "ABO Reason", dependsOn: "ABO Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "DNA Status", dependsOn: "DNA", showWhen: ["NEEDED"] },
                { field: "DNA Reason", dependsOn: "DNA Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "G6PD Checkout Status", dependsOn: "G6PD", showWhen: ["NEEDED"] },
                { field: "G6PD Reason", dependsOn: "G6PD Checkout Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "Lipid Status", dependsOn: "Lipid Needed", showWhen: ["NEEDED"] },
                { field: "Lipid Reason", dependsOn: "Lipid Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "EKG Needed Status", dependsOn: "EKG NEEDED", showWhen: ["NEEDED"] },
                { field: "EKG Needed Reason", dependsOn: "EKG Needed Status", showWhen: ["Not Completed"] }
            ],
            [
                { field: "Pregnancy Test Status", dependsOn: "Pregnancy Test Needed", showWhen: ["NEEDED"] },
                { field: "Pregnancy Test Reason", dependsOn: "Pregnancy Test Status", showWhen: ["Not Completed"] }
            ]
        ],
        "PHA": [
            [
                { field: "PHA Status", dependsOn: "PHA Needed", showWhen: ["NEEDED"] },
                { field: "PHA Follow Up", dependsOn: "PHA Status", showWhen: ["Completed"] }
            ]
        ],
        "Audiologist": [
            [
                { field: "Hearing Status", dependsOn: "HEARING Needed", showWhen: ["NEEDED"] },
                { field: "Audiologist Service Completed", dependsOn: "Hearing Status", showWhen: ["Completed"] }
            ]
        ],
        "Vision": [
            [
                { field: "Vision Status", dependsOn: "VISION Needed", showWhen: ["NEEDED"] },
                { field: "Optometrist Service Completed", dependsOn: "Vision Status", showWhen: ["Completed"] }
            ]
        ],
        "Dental": [
            [
                { field: "Dental X-Ray Status", dependsOn: null, showWhen: null }
            ],
            [
                { field: "Panoramic X-Ray", dependsOn: "PANO Needed", showWhen: ["NEEDED"] }
            ],
            [
                { field: "Dental Exam Status", dependsOn: "Dental Needed", showWhen: ["NEEDED"] },
                { field: "Class (Dental Exam)", dependsOn: "Dental Exam Status", showWhen: ["Completed"] },
                { field: "Dental Treatment", dependsOn: "Class (Dental Exam)", showWhen: ["3"] },
                { field: "Final Dental Class", dependsOn: "Dental Treatment", showWhen: ["Yes"] },
                { field: "Dental Treatment Reason", dependsOn: "Dental Treatment", showWhen: ["No"] }
            ]
        ]

    }



};


let smIdEditing = 0;
let currentRow;
let isAddingNewRow = false;
let primaryKeyForEdit = 0;

function editRow(button) {
    currentRow = $(button).closest('tr');  // Get the row clicked for editing
    const rowData = {};
    isAddingNewRow = false;
    // Correct mapping: use tableToKeysIndexMap to map the table column index to the keys array
    tableToKeysIndexMap.forEach((tableColIndex, keysIndex) => {
        const key = keys[tableColIndex];  // Get the correct key from the keys array
        rowData[key] = currentRow.find('td').eq(tableColIndex).text().trim() || '';  // Get the value from the correct table column
    });

    smIdEditing = currentRow.find('td').eq(keys.indexOf('SM ID')).text();
    primaryKeyForEdit = currentRow.find('.row-id').val();
    populateModalForEdit(rowData);  // Pass the mapped data to populateModal
    document.getElementById("editModalLabel").innerText = "Edit Service Member";
    $('#editModal').modal('show');  // Show the modal
}

//for edit
//const readOnlyIndexesForEdit = [7, 9, 10, 12, 14, 66, 63, 64, 65, 70, 67, 69, 71, 68, 15, 16, 21, 24, 22, 25, 31, 33, 34, 36, 40, 45, 35, 43, 44, 54];
//const dropdownIndexesForEdit = [8, 10, 12, 17, 19, 20, 23, 26, 27, 28, 29, 30, 32, 37, 38, 39, 41, 42, 46, 47, 52, 53, 55, 56, 57, 58, 59, 60];
//const tableDataFieldsForEdit = [61];
//const customFieldsForEdit = [18, 48, 49, 50, 51];

const readOnlyFieldsForEdit = [
    "AGE", "MOS", "AGR", "MRC", "OVER 40", "EventID", "Over 44", "EventDate", "Event End Date",
    "HIV Win", "Vision Win", "PHA Win", "Hearing WIN", "Dental Win",
    "Dental Due", "Dental Exam", "PHA Date", "PULHES", "PHA Due", "Vision Date",
    "Hearing Date", "HRC", "Hearing Profile", "Lab Requisition", "Sickle Date",
    "HIV NEXT TEST DATE", "Quest", "G6PD Date", "G6PD Status", "IMM Needed"
    //Check out section fields
    ,"Vitals Status"
];

const requiredFields = ['LAST NAME', 'FIRST NAME', 'FULL NAME', 'FULL SSN', 'DOD ID', 'DOB', 'TaskForce', 'SEX'];

const dropdownFieldsForEdit = [
    "SEX", "AGR", "MRC", "Dental Needed", "BWX Needed", "DRC", "PHA Needed",
    "VISION Needed", "NEAR VISION Needed", "VRC", "Vision 2PG", "Vision Mask Insert",
    "HEARING Needed", "ABO", "ABO Needed", "DNA", "SICKLE", "G6PD", "HIV", "Lipid Needed", "EKG NEEDED", "Pregnancy Test Needed", "Hep B Needed", "Hep A Needed", "FLU Needed", "Tet/TDP Needed", "MMR Needed", "Varicella Needed",
    "Vitals Status",
    "Hep B Needed Status", "Hep B Reason", "FLU Needed Status", "FLU Reason", "MMR Status", "MMR Reason", "Hep A Needed Status", "Hep A Reason", "Tet/TDP Needed Status", "Tet/TDP Reason", "Varicella Needed Status", "Varicella Reason",
    "SICKLE Status", "Sickle Reason", "HIV Status", "HIV Reason", "ABO Status", "ABO Reason", "DNA Status", "DNA Reason", "G6PD Checkout Status", "G6PD Reason", "Lipid Status", "Lipid Reason", "EKG Needed Status", "EKG Needed Reason", "Pregnancy Test Status", "Pregnancy Test Reason",
    "Hearing Status", "Audiologist Service Completed",
    "Vision Status", "Optometrist Service Completed",
    "PHA Status", "PHA Follow Up",
    "Dental X-Ray Status", "Panoramic X-Ray", "Dental Exam Status", "Class (Dental Exam)", "Dental Treatment", "Final Dental Class", "Dental Treatment Reason"
];

const tableDataFieldsForEdit = ["TaskForce"];

const customFieldsForEdit = [
    "PANO Needed", "LIPID PANEL", "Cholesterol / HDL Cholesterol", "Framingham", "EKG (Date)"
];


//for add
//const dropdownIndexesForAdd = [8, 17, 18, 19, 20, 23, 26, 27, 28, 29, 30, 32, 37, 38, 39, 41, 42, 46, 47, 48, 49, 50, 51, 52, 53, 55, 56, 57, 58, 59, 60];
//const readOnlyIndexesForAdd = [7, 9, 10, 12, 14, 66, 63, 64, 65, 70, 67, 69, 71, 68, 15, 16, 21, 24, 22, 25, 31, 33, 34, 36, 40, 45, 35, 43, 44, 54];
//const tableDataFieldsForAdd = [61, 64, 65, 66, 67, 68, 69, 70, 71];

const dropdownFieldsForAdd = [
    "SEX", "Dental Needed", "PANO Needed", "BWX Needed", "DRC", "PHA Needed",
    "VISION Needed", "NEAR VISION Needed", "VRC", "Vision 2PG", "Vision Mask Insert",
    "HEARING Needed", "ABO", "ABO Needed", "DNA", "SICKLE", "G6PD", "HIV", "Lipid Needed",
    "LIPID PANEL", "Cholesterol / HDL Cholesterol", "Framingham", "EKG (Date)", "EKG NEEDED", "Pregnancy Test Needed",
    "Hep B Needed", "Hep A Needed", "FLU Needed", "Tet/TDP Needed", "MMR Needed", "Varicella Needed",
    "Vitals Status",
    "Hep B Needed Status", "Hep B Reason", "FLU Needed Status", "FLU Reason", "MMR Status", "MMR Reason", "Hep A Needed Status", "Hep A Reason", "Tet/TDP Needed Status", "Tet/TDP Reason", "Varicella Needed Status", "Varicella Reason",
    "SICKLE Status", "Sickle Reason", "HIV Status", "HIV Reason", "ABO Status", "ABO Reason", "DNA Status", "DNA Reason", "G6PD Checkout Status", "G6PD Reason", "Lipid Status", "Lipid Reason", "EKG Needed Status", "EKG Needed Reason", "Pregnancy Test Status", "Pregnancy Test Reason",
    "Hearing Status", "Audiologist Service Completed",
    "Vision Status", "Optometrist Service Completed",
    "PHA Status", "PHA Follow Up",
    "Dental X-Ray Status", "Panoramic X-Ray", "Dental Exam Status", "Class (Dental Exam)", "Dental Treatment", "Final Dental Class", "Dental Treatment Reason"

];

const readOnlyFieldsForAdd = [
    "AGE", "MOS", "AGR", "MRC", "OVER 40", "EventID", "Over 44", "EventDate", "Event End Date",
    "HIV Win", "Vision Win", "PHA Win", "Hearing WIN", "Dental Win",
    "Dental Due", "Dental Exam", "PHA Date", "PULHES", "PHA Due", "Vision Date",
    "Hearing Date", "HRC", "Hearing Profile", "Lab Requisition", "Sickle Date",
    "HIV NEXT TEST DATE", "Quest", "G6PD Date", "G6PD Status", "IMM Needed"
    //Check out section fields
    , "Vitals Status"
];

const tableDataFieldsForAdd = [
    "TaskForce", "EventDate", "Event End Date", "EventID",
    "Vision Win", "Dental Win", "PHA Win", "HIV Win", "Hearing WIN"
];

//for edit and add both
const calendarFields = ["DOB"];
const multilineTextbox = ["Notes"];


// Define specific dropdown options for certain fields
const dropdownOptionsMapping = {
    "SEX": [
        { value: "", label: "" },
        { value: "M", label: "M" },
        { value: "F", label: "F" }
    ],
    "MRC": [
        { value: "1", label: "1" },
        { value: "2", label: "2" },
        { value: "3", label: "3" },
        { value: "4", label: "4" }
    ],
    "AGR": [
        { value: "N", label: "N" },
        { value: "Y", label: "Y" }
    ],
    "DRC": [
        { value: "1", label: "1" },
        { value: "2", label: "2" },
        { value: "3", label: "3" },
        { value: "4", label: "4" }
    ],
    "VRC": [
        { value: "", label: "" },
        { value: "1", label: "1" },
        { value: "2", label: "2" },
        { value: "3O", label: "3O" },
        { value: "3V", label: "3V" }
    ],
    "Vision 2PG": [
        { value: "N", label: "N" },
        { value: "Y", label: "Y" },
        { value: "NA", label: "NA" }
    ],
    "Vision Mask Insert": [
        { value: "", label: "" },
        { value: "N", label: "N" },
        { value: "Y", label: "Y" },
        { value: "NA", label: "NA" }
    ],
    "Hearing Profile": [
        { value: "1", label: "1" },
        { value: "2", label: "2" },
        { value: "3", label: "3" },
        { value: "4", label: "4" }
    ],
    "ABO": [
        { value: "", label: "" },
        { value: "A+", label: "A+" },
        { value: "A-", label: "A-" },
        { value: "B+", label: "B+" },
        { value: "B-", label: "B-" },
        { value: "AB+", label: "AB+" },
        { value: "AB-", label: "AB-" },
        { value: "O+", label: "O+" },
        { value: "O-", label: "O-" }
    ],
    "Pregnancy Test Needed": [
        { value: "N/A", label: "N/A" },
        { value: "Needed", label: "Needed" }
    ],
    "ABO Needed": [
        { value: "NEEDED", label: "NEEDED" },
        { value: "N/A", label: "N/A" }
    ],
    "Vitals Status": [
        { value: "Completed", label: "Completed" }
    ],
    "Hep B Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Refused", label: "Refused" },
        { value: "Contraindication", label: "Contraindication" }
    ],
    "FLU Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Refused", label: "Refused" },
        { value: "Contraindication", label: "Contraindication" }
    ],
    "MMR Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Refused", label: "Refused" },
        { value: "Contraindication", label: "Contraindication" }
    ],
    "Hep A Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Refused", label: "Refused" },
        { value: "Contraindication", label: "Contraindication" }
    ],
    "Tet/TDP Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Refused", label: "Refused" },
        { value: "Contraindication", label: "Contraindication" }
    ],
    "Varicella Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Refused", label: "Refused" },
        { value: "Contraindication", label: "Contraindication" }
    ],
    "Sickle Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Unable to draw", label: "Unable to draw" },
        { value: "Proof of service", label: "Proof of service" },
        { value: "Not due", label: "Not due" }
    ],
    "HIV Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Unable to draw", label: "Unable to draw" },
        { value: "Proof of service", label: "Proof of service" },
        { value: "Not due", label: "Not due" }
    ],
    "ABO Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Unable to draw", label: "Unable to draw" },
        { value: "Proof of service", label: "Proof of service" },
        { value: "Not due", label: "Not due" }
    ],
    "DNA Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Unable to draw", label: "Unable to draw" },
        { value: "Proof of service", label: "Proof of service" },
        { value: "Not due", label: "Not due" }
    ],
    "G6PD Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Unable to draw", label: "Unable to draw" },
        { value: "Proof of service", label: "Proof of service" },
        { value: "Not due", label: "Not due" }
    ],
    "Lipid Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Unable to draw", label: "Unable to draw" },
        { value: "Proof of service", label: "Proof of service" },
        { value: "Not due", label: "Not due" }
    ],
    "EKG Needed Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Unable to draw", label: "Unable to draw" },
        { value: "Proof of service", label: "Proof of service" },
        { value: "Not due", label: "Not due" }
    ],
    "Pregnancy Test Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "Unable to draw", label: "Unable to draw" },
        { value: "Proof of service", label: "Proof of service" },
        { value: "Not due", label: "Not due" }
    ]
   ,
    "Dental Treatment Reason": [
        { value: "", label: "" },
        { value: "Insufficient Supplies", label: "Insufficient Supplies" },
        { value: "SM Refused", label: "SM Refused" },
        { value: "Time limitation", label: "Time limitation" },
        { value: "Procedure not offered", label: "Procedure not offered" },
        { value: "Military excused", label: "Military excused" },
        { value: "SM current receiving treatment", label: "SM current receiving treatment" }
    ], "Dental Treatment": [
        { value: "", label: "" },
        { value: "Yes", label: "Yes" },
        { value: "No", label: "No" }
    ]
    , "PHA Follow Up": [
        { value: "", label: "" },
        { value: "Yes", label: "Yes" },
        { value: "No", label: "No" }
    ]
    , "Audiologist Service Completed": [
        { value: "", label: "" },
        { value: "Yes", label: "Yes" },
        { value: "No", label: "No" }
    ]
    , "Optometrist Service Completed": [
        { value: "", label: "" },
        { value: "Yes", label: "Yes" },
        { value: "No", label: "No" }
    ]
    , "Class (Dental Exam)": [
        { value: "", label: "" },
        { value: "1", label: "1" },
        { value: "2", label: "2" },
        { value: "3", label: "3" }
    ]
    , "Final Dental Class": [
        { value: "", label: "" },
        { value: "1", label: "1" },
        { value: "2", label: "2" },
        { value: "3", label: "3" }
    ]

};

function applyRequired(modalContent) {
    // Only consider visible dependent fields inside Check Out Section
    modalContent.find('.dependent-field:visible').each(function () {
        const container = $(this);
        const input = container.find('input, select, textarea');

        // Set required if visible
        input.prop('required', true);
    });

    // Remove required from hidden fields
    modalContent.find('.dependent-field:hidden').each(function () {
        const container = $(this);
        const input = container.find('input, select, textarea');
        input.prop('required', false);
    });
}


function applyDependencies(modalContent, data) {
    function hideWithChildren(fieldName) {
        modalContent.find(`.dependent-field[data-depends-on="${fieldName}"]`).each(function () {
            const child = $(this);
            child.hide();
            child.find('input, select, textarea').val('');
            hideWithChildren(child.data('field')); // recursively hide children
        });
    }

    const checkedOutVal = $("#checkedOut").val();

    modalContent.find('.dependent-field').each(function () {
        const container = $(this);
        const field = container.data('field');
        const dependsOn = container.data('depends-on');
        const showWhen = container.data('show-when');

        if (container.closest('[data-subcategory]').length > 0 && checkedOutVal !== "Yes") {
            container.hide();
            container.find('input, select, textarea').val('');
            hideWithChildren(field);
            return;
        }

        if (!dependsOn) {
            container.show();
            return;
        }

        const dependsVal = data[dependsOn] || modalContent.find(`[name="${dependsOn}"]`).val();

        if (showWhen.includes(dependsVal)) {
            container.show();
        } else {
            container.hide();
            container.find('input, select, textarea').val('');
            hideWithChildren(field);
        }
    });

    // 🔹 Hide sub-category headers if all fields are hidden
    modalContent.find('.subcategory-header').each(function () {
        const header = $(this);
        const subCategory = header.text();
        const allFields = modalContent.find(`.row[data-subcategory="${subCategory}"] .dependent-field`);
        const anyVisible = allFields.filter(':visible').length > 0;

        if (anyVisible) {
            header.show();
            header.next('hr').show(); // show the hr
        } else {
            header.hide();
            header.next('hr').hide(); // hide the hr
        }
    });

    applyRequired(modalContent);
}

$("#checkedOut").on("change", function () {
    const val = $(this).val();
    if (val === "Yes") {
        // Set Vital Status explicitly
        $('[name="Vitals Status"]').val("Completed");
    } else {
        // Clear Vital Status
        $('[name="Vitals Status"]').val("");
    }

    // Re-evaluate dependencies and required
    applyDependencies($("#modalBodyContent"), {});
});


const modalContent = $('#modalBodyContent');

function populateModalForEdit(data) {
    modalContent.empty(); // Clear previous content
    let textColor = 'style="color: black;"'; // Set text color to black
    const fieldsPerRow = 5; // Set to 5 fields per row now

    for (const [categoryName, categoryKeys] of Object.entries(categories)) {
        if (categoryName !== 'Check In Out Information') {
            modalContent.append(`<h5 class="category-header">${categoryName}</h5><hr/>`);
        }
  
        if (categoryName === "Check Out Section") {
            for (const [subCategory, rows] of Object.entries(categoryKeys)) {
                // Sub-category heading
                modalContent.append(`<h5 class="subcategory-header">${subCategory}</h5><hr/>`);

                // Each row is an array of field objects
                rows.forEach(rowFields => {
                    let rowHtml = `<div class="row" data-subcategory="${subCategory}">`;

                    rowFields.forEach(fieldObj => {
                        const key = fieldObj.field;
                        const value = data[key] || '';
                        let inputHtml = '';
                        let disabled = readOnlyFieldsForEdit.includes(key) ? 'disabled' : '';
                        
                        if (dropdownFieldsForEdit.includes(key)) {
                            const dropdownOptions = dropdownOptionsMapping[key] || [
                                { value: "", label: "" },
                                { value: "Completed", label: "Completed" },
                                { value: "Not Completed", label: "Not Completed" }
                            ];

                            let optionsHtml = dropdownOptions.map(option =>
                                `<option value="${option.value}" ${value === option.value ? 'selected' : ''}>${option.label}</option>`
                            ).join('');


                            inputHtml = `
                            <div class="col-md-3 dependent-field" 
                                 data-field="${key}" 
                                 data-depends-on="${fieldObj.dependsOn || ''}" 
                                 data-show-when='${JSON.stringify(fieldObj.showWhen || [])}'>
                              <div class="form-group">
                                <label>${key}</label>
                                <select class="form-control" name="${key}" ${disabled} ${textColor}>
                                  ${optionsHtml}
                                </select>
                              </div>
                            </div>`;
                        }
                        else {
                            // 🔹 Textbox logic (still uses rule engine)
                            inputHtml = `
                            <div class="col-md-3 dependent-field"
                                 data-field="${key}"
                                 data-depends-on="${fieldObj.dependsOn || ''}"
                                 data-show-when='${JSON.stringify(fieldObj.showWhen || [])}'>
                                <div class="form-group">
                                    <label>${key}</label>
                                    <input type="text" class="form-control" name="${key}" value="${value}" ${disabled} ${textColor} />
                                </div>
                            </div>
                        `;
                        }

                        rowHtml += inputHtml;
                    });

                    rowHtml += `</div>`; // close row
                    modalContent.append(rowHtml);
                });
            }
        }


        else {
            let rowHtml = '<div class="row">';
            let inputCount = 0;
            categoryKeys.forEach((key, index) => {
                const value = data[key] || '';
                let inputHtml = '';
                let keyIndex = keys.indexOf(key);
                let readOnly = readOnlyFieldsForEdit.includes(key) ? 'readonly' : '';
                let required = requiredFields.includes(key) ? 'required' : '';


                if (tableDataFieldsForEdit.includes(key)) {
                    const table = $('#previewTable').DataTable();
                    const headers = table.columns().header().toArray().map(th => $(th).text().trim());
                    const taskForceIndex = headers.indexOf("TaskForce");

                    if (taskForceIndex === -1) return; // If column not found, exit early

                    // Collect distinct TaskForce values
                    const taskForceValues = [...new Set(table.rows().data().toArray().map(row => row[taskForceIndex]).filter(Boolean))];

                    // Add default option first
                    let optionsHtml = `<option value="">---Select Taskforce---</option>`;

                    // Add dynamic options
                    optionsHtml += taskForceValues
                        .map(val => `<option value="${val}" ${value === val ? 'selected' : ''}>${val}</option>`)
                        .join('');

                    let disabled = readOnlyFieldsForEdit.includes(key) ? 'disabled' : '';

                    inputHtml = `
                <div class="form-group col-lg-2">
                    <label>${key}</label>
                    <select class="form-control" name="${key}" ${disabled} ${textColor} ${required}>
                        ${optionsHtml}
                    </select>
                </div>
            `;
                }

                else if (customFieldsForEdit.includes(key)) {
                    if (!window.isCheckInOutPage) {

                        if (key === 'LIPID PANEL' || key === 'EKG (Date)') {
                            if (value.trim().toLowerCase() === "n/a") {
                                inputHtml = `
                                                    <div class="form-group col-lg-2">
                                                                <label>${key}</label>
                                                                            <input type="text" class="form-control" name="${key}" value="${value}" ${required} readonly ${textColor} />
                                                            </div>`;

                            } else {
                                let dateValue = isValidDate(value) ? formatDateToYYYYMMDD(value) : '';
                                inputHtml = `
                                                            <div class="form-group col-lg-2">
                                                                <label>${key}</label>
                                                                <input type="date" class="form-control" name="${key}" value="${dateValue}" placeholder="mm/dd/yyyy" ${required} ${readOnly} ${textColor} />
                                                            </div>`;
                            }
                        }
                        else if (key === 'Cholesterol / HDL Cholesterol') {
                            if (value.trim().toLowerCase() === "n/a") {
                                inputHtml = `<div class="form-group col-lg-2">
                                                                            <label>${key}</label>
                                                                                        <input type="text" class="form-control" name="${key}" value="${value}" ${required} readonly ${textColor} />
                                                                        </div>`;

                            } else {
                                inputHtml = `<div class="form-group col-lg-2">
                                                                                        <label>${key}</label>
                                                                                                    <input type="text" class="form-control" name="${key}" value="${value.toLowerCase() === 'needed' ? '' : value}" ${readOnly} ${required} ${textColor} />
                                                                                    </div>`;
                            }
                        }
                        else if (key === 'Framingham') {
                            if (value.trim().toLowerCase() === "n/a") {
                                inputHtml = `<div class="form-group col-lg-2">
                                                                                        <label>${key}</label>
                                                                                                                <input type="text" class="form-control decimal-input" name="${key}" value="${value}" ${required} readonly ${textColor} />
                                                                                    </div>`;

                            } else {
                                inputHtml = `<div class="form-group col-lg-2">
                                                                                        <label>${key}</label>
                                                                                            <input type="number" class="form-control decimal-input" name="${key}"
                                                                                            value="${value.toLowerCase() === 'needed' ? '' : value}"
                                                                                            step="0.1" inputmode="decimal" ${readOnly} ${required} ${textColor} />

                                                                                    </div>`;
                            }
                        }
                        if (key === 'PANO Needed') {
                            if (value.trim().toLowerCase() === "n/a") {
                                inputHtml = `
                    <div class="form-group col-lg-2">
                        <label>${key}</label>
                        <select class="form-control" name="${key}" ${textColor}>
                            <option value="NEEDED">NEEDED</option>
                            <option value="N/A" selected>N/A</option>
                        </select>
                    </div>`;
                            } else {
                                let dateValue = isValidDate(value) ? formatDateToYYYYMMDD(value) : '';
                                inputHtml = `
                    <div class="form-group col-lg-2">
                        <label>${key}</label>
                        <input type="date" class="form-control" name="${key}" value="${dateValue}" placeholder="mm/dd/yyyy" ${readOnly} ${required} ${textColor} />
                    </div>`;
                            }
                        }
                    }
                    else {
                        const dropdownOptions = [
                            { value: "N/A", label: "N/A" },
                            { value: "NEEDED", label: "NEEDED" }
                        ];

                        // Build dropdown options dynamically
                        let optionsHtml = dropdownOptions.map(option =>
                            `<option value="${option.value}" ${value === option.value ? 'selected' : ''}>${option.label}</option>`
                        ).join('');

                        let disabled = readOnlyFieldsForEdit.includes(key) ? 'disabled' : '';

                        inputHtml = `
                                        <div class="form-group col-lg-2">
                                            <label>${key}</label>
                                            <select class="form-control" name="${key}" ${disabled} ${required} ${textColor}>

                                                ${optionsHtml}
                                            </select>
                                        </div>
                                    `;
                    }
                }
                else if (multilineTextbox.includes(key)) {
                    inputHtml = `
                            <div class="form-group col-lg-12">
                                <label>${key}</label>
                                <textarea class="form-control" name="${key}" rows="4" ${readOnly} ${required} ${textColor}>${value}</textarea>
                            </div>
                        `;
                }
                else if (calendarFields.includes(key)) {
                    let dateValue = isValidDate(value) ? formatDateToYYYYMMDD(value) : '';
                    inputHtml = `
                                    <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <input type="date" class="form-control" name="${key}" min="1900-01-01" max="3000-12-31" value="${dateValue}" placeholder="mm/dd/yyyy" ${readOnly} ${required} ${textColor} />
                                    </div>
                                `;
                }
                // Dropdown field
                else if (dropdownFieldsForEdit.includes(key)) {
                    const dropdownOptions = dropdownOptionsMapping[key] || [
                        { value: "N/A", label: "N/A" },
                        { value: "NEEDED", label: "NEEDED" }
                    ];

                    // Build dropdown options dynamically
                    let optionsHtml = dropdownOptions.map(option =>
                        `<option value="${option.value}" ${value === option.value ? 'selected' : ''}>${option.label}</option>`
                    ).join('');

                    let disabled = readOnlyFieldsForEdit.includes(key) ? 'disabled' : '';

                    inputHtml = `
                                    <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <select class="form-control" name="${key}" ${disabled} ${required} ${textColor}>

                                            ${optionsHtml}
                                        </select>
                                    </div>
                                `;
                }
                else if (key === 'Checked In' || key === 'Checked Out' || key === 'Checked In By' || key === 'Checked Out By') {
                    if (key === 'Checked In') {
                        $("#checkedIn").val(value).trigger("change");
                    }
                    else if (key === 'Checked Out') {
                        $("#checkedOut").val(value);
                    }
                    else if (key === 'Checked In By') {
                        $("#checkedInBy").val(value);
                    }
                    else if (key === 'Checked Out By') {
                        $("#checkedOutBy").val(value);
                    }
                }
                // Default text field
                else {
                    inputHtml = `
                                    <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <input type="text" class="form-control" name="${key}" value="${value}" ${required} ${readOnly} ${textColor} />
                                    </div>
                                `;
                }

                if (window.isCheckInOutPage) {//temporary hide fields for checkincheckout page i.e for keuler application, for revert : Just remove if condition else will be working for all scenerios.(rowHtml += inputHtml;  inputCount++; )
                    if (key !== 'LIPID PANEL' && key !== 'Cholesterol / HDL Cholesterol' && key !== 'Framingham') {
                        rowHtml += inputHtml;
                        inputCount++;
                    }
                }
                else {
                    rowHtml += inputHtml;
                    inputCount++;
                }

                // If we have 5 fields, close the row and start a new one
                if (inputCount % fieldsPerRow === 0) {
                    rowHtml += '</div><div class="row">';
                }
            });

            if (inputCount % fieldsPerRow !== 0) {
                const emptyDivsNeeded = fieldsPerRow - (inputCount % fieldsPerRow);
                for (let i = 0; i < emptyDivsNeeded; i++) {
                    rowHtml += '<div class="form-group col-lg-2"></div>';
                }
            }

            rowHtml += '</div>';
            modalContent.append(rowHtml);

        }
        // Attach input validation listeners to all text fields
        modalContent.find('input[type="text"]').on('input', function () {
            const value = $(this).val();
            validateInput(this, value);
        });
    }

    //Event listner for modal dropdown change to hide/show checkout sections controls
    modalContent.on('change', 'select', function () {
        applyDependencies(modalContent, {});
    });

    //apply rule engine on modal load
    applyDependencies(modalContent, data);

    //Event listner on Dropdown Check Out Section controls to apply dependencies based on rule engine
    $("#modalBodyContent").on('change', '.dependent-field select', function () {
        applyDependencies($("#modalBodyContent"), {});
    });

    //Event listner on Textbox Check Out Section controls to apply dependencies based on rule engine
    $("#modalBodyContent").on('input', '.dependent-field input[type="text"]', function () {
        applyDependencies($("#modalBodyContent"), {});
    });

    // After modal is populated, bind the event listener to the DOB field
    const dobField = modalContent.find('input[name="DOB"]');
    if (dobField.length > 0) {
        dobField.on('change', function () {
            const dobValue = $(this).val();
            if (dobValue) {
                handleColumnsRelatedToDob(dobValue)
                // const age = calculateAge(dobValue); // Calculate age based on DOB
                // updateFieldsBasedOnAge(age); // Update other fields based on the age
            }
        });
    }

    //****************************************************This can be needed in future*************************************************/
    // const aboField = modalContent.find('select[name="ABO"]');
    // const aboNeededField = modalContent.find('select[name="ABO Needed"]');

    // if (aboField.length > 0 && aboNeededField.length > 0) {
    //     aboField.on('change', function () {
    //         const aboValue = $(this).val();
    //         if (aboValue === "") { // If "ABO" is blank
    //             aboNeededField.val("NEEDED").change(); // Set "ABO NEEDED" to "NEEDED"
    //         } else {
    //             aboNeededField.val("N/A").change(); // Optionally reset the field if "ABO" is not blank
    //         }
    //     });
    // }

    const fieldsToCheckQuest = ['ABO Needed', 'G6PD', 'SICKLE', 'Lipid Needed'];
    const fieldsToCheckLab = ['ABO Needed', 'G6PD', 'SICKLE', 'Lipid Needed', 'HIV', 'DNA'];
    const fieldsToCheckImmunization = ['Hep B Needed', 'Hep A Needed', 'FLU Needed', 'Tet/TDP Needed', 'MMR Needed', 'Varicella Needed'];

    fieldsToCheckQuest.forEach(field => {
        const inputField = modalContent.find(`select[name="${field}"]`);
        if (inputField.length > 0) {
            inputField.on('change', checkLabRequisitionField);
        }
    });

    fieldsToCheckLab.forEach(field => {
        const inputField = modalContent.find(`select[name="${field}"]`);
        if (inputField.length > 0) {
            inputField.on('change', checkLabNeededField);
        }
    });

    fieldsToCheckImmunization.forEach(field => {
        const inputField = modalContent.find(`select[name="${field}"]`);
        if (inputField.length > 0) {
            inputField.on('change', checkIMMNeededField);
        }
    });

    // After modal is populated, bind the event listener to the Dental Needed field
    const dentalNeededField = modalContent.find('select[name="Dental Needed"]');
    if (dentalNeededField.length > 0) {
        dentalNeededField.on('change', function () {
            const dentalNeededValue = $(this).val();
            if (dentalNeededValue) {
                handleBWXNeededLogicOnDentalNeeded(dentalNeededValue)
            }
        });
    }

    const aboField = document.querySelector('[name="ABO"]');
    //handleABOtoABONeededLogic(aboField.value);
    if (aboField) {
        aboField.addEventListener('change', function () {
            handleABOtoABONeededLogic(this.value);
        });
    }

    const nearVisionNeededField = document.querySelector('[name="NEAR VISION Needed"]');
    if (nearVisionNeededField) {
        nearVisionNeededField.addEventListener('change', function () {
            handleNearVisionNeededToVisionNeededLogic(this.value);
        });
    }
}

function populateModalForAdd(data) {
    modalContent.empty(); // Clear previous content
    let textColor = 'style="color: black;"'; // Set text color to black
    const fieldsPerRow = 5; // Set to 5 fields per row now

    for (const [categoryName, categoryKeys] of Object.entries(categories)) {
        if (categoryName !== 'Check In Out Information') {
            modalContent.append(`<h5 class="category-header">${categoryName}</h5><hr/>`);
        }

        if (categoryName === "Check Out Section") {
            for (const [subCategory, rows] of Object.entries(categoryKeys)) {
                // Sub-category heading
                modalContent.append(`<h5 class="subcategory-header">${subCategory}</h5><hr/>`);

                // Each row is an array of field objects
                rows.forEach(rowFields => {
                    let rowHtml = `<div class="row" data-subcategory="${subCategory}">`;

                    rowFields.forEach(fieldObj => {
                        const key = fieldObj.field;
                        const value = data[key] || '';
                        let inputHtml = '';
                        let disabled = readOnlyFieldsForAdd.includes(key) ? 'disabled' : '';

                        if (dropdownFieldsForAdd.includes(key)) {
                            const dropdownOptions = dropdownOptionsMapping[key] || [
                                { value: "", label: "" },
                                { value: "Completed", label: "Completed" },
                                { value: "Not Completed", label: "Not Completed" }
                            ];

                            let optionsHtml = dropdownOptions.map(option =>
                                `<option value="${option.value}" ${value === option.value ? 'selected' : ''}>${option.label}</option>`
                            ).join('');


                            inputHtml = `
                            <div class="col-md-3 dependent-field" 
                                 data-field="${key}" 
                                 data-depends-on="${fieldObj.dependsOn || ''}" 
                                 data-show-when='${JSON.stringify(fieldObj.showWhen || [])}'>
                              <div class="form-group">
                                <label>${key}</label>
                                <select class="form-control" name="${key}" ${disabled} ${textColor}>
                                  ${optionsHtml}
                                </select>
                              </div>
                            </div>`;
                        }
                        else {
                            // 🔹 Textbox logic (still uses rule engine)
                            inputHtml = `
                            <div class="col-md-3 dependent-field"
                                 data-field="${key}"
                                 data-depends-on="${fieldObj.dependsOn || ''}"
                                 data-show-when='${JSON.stringify(fieldObj.showWhen || [])}'>
                                <div class="form-group">
                                    <label>${key}</label>
                                    <input type="text" class="form-control" name="${key}" value="${value}" ${disabled} ${textColor} />
                                </div>
                            </div>
                        `;
                        }

                        rowHtml += inputHtml;
                    });

                    rowHtml += `</div>`; // close row
                    modalContent.append(rowHtml);
                });
            }
        }
        else {
            let rowHtml = '<div class="row">';
            let inputCount = 0;
            categoryKeys.forEach((key, index) => {
                const value = data[key] || '';
                let inputHtml = '';
                let keyIndex = keys.indexOf(key);
                let readOnly = readOnlyFieldsForAdd.includes(key) ? 'readonly' : '';
                let required = requiredFields.includes(key) ? 'required' : '';

                if (multilineTextbox.includes(key)) {
                    inputHtml = `
                            <div class="form-group col-lg-12">
                                <label>${key}</label>
                                <textarea class="form-control" name="${key}" rows="4" ${readOnly} ${required} ${textColor}>${value}</textarea>
                            </div>
                        `;
                }
                else if (calendarFields.includes(key)) {
                    let dateValue = isValidDate(value) ? formatDateToYYYYMMDD(value) : '';
                    inputHtml = `
                                    <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <input type="date" class="form-control" name="${key}" min="1900-01-01" max="3000-12-31" value="${dateValue}" placeholder="mm/dd/yyyy" ${readOnly} ${required} ${textColor} />
                                    </div>
                                `;
                }
                // Dropdown field
                else if (dropdownFieldsForAdd.includes(key)) {
                    const dropdownOptions = dropdownOptionsMapping[key] || [
                        { value: "N/A", label: "N/A" },
                        { value: "NEEDED", label: "NEEDED" }
                    ];

                    // Build dropdown options dynamically
                    let optionsHtml = dropdownOptions.map(option =>
                        `<option value="${option.value}" ${value === option.value ? 'selected' : ''}>${option.label}</option>`
                    ).join('');

                    let disabled = readOnlyFieldsForAdd.includes(key) ? 'disabled' : '';

                    inputHtml = `
                                    <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <select class="form-control" name="${key}" ${disabled} ${required} ${textColor}>

                                            ${optionsHtml}
                                        </select>
                                    </div>
                                `;
                }
                else if (tableDataFieldsForAdd.includes(key)) {
                    if (key === "TaskForce") {
                        const table = $('#previewTable').DataTable();

                        const taskForceIndex = keys.indexOf("TaskForce");
                        if (taskForceIndex === -1) return;

                        // Get all unique TaskForce values from all rows
                        const taskForceValues = [...new Set(table.rows().data().toArray().map(row => row[taskForceIndex]).filter(Boolean))];

                        // Add default option
                        let optionsHtml = `<option value="">---Select Taskforce---</option>`;

                        // Add each option dynamically
                        optionsHtml += taskForceValues.map(val => `<option value="${val}" ${value === val ? 'selected' : ''}>${val}</option>`).join('');

                        let disabled = readOnlyFieldsForAdd.includes(key) ? 'disabled' : '';

                        inputHtml = `
                    <div class="form-group col-lg-2">
                        <label>${key}</label>
                        <select class="form-control" name="${key}" ${disabled} ${required} ${textColor}>
                            ${optionsHtml}
                        </select>
                    </div>
                    `;
                    }
                    else {
                        var table = $('#previewTable').DataTable();
                        var valueOfColumn = table.row(0).data()[keys.indexOf(key)];

                        inputHtml = `
                                     <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <input type="text" class="form-control" name="${key}" value="${valueOfColumn}" ${readOnly} ${required} ${textColor} />
                                    </div>
                                `;
                    }
                }


                // Default text field
                else {
                    if (key === 'FULL NAME') {
                        // You can optionally split value if it contains full name parts
                        const [lastName = '', firstName = '', middleName = ''] = value.split(' ');

                        inputHtml = `
            <div class="form-group col-lg-2">
                <label>LAST NAME</label>
                <input type="text" class="form-control" name="LAST NAME" value="${lastName}" ${required} ${textColor} />
            </div>
            <div class="form-group col-lg-2">
                <label>FIRST NAME</label>
                <input type="text" class="form-control" name="FIRST NAME" value="${firstName}" ${required} ${textColor} />
            </div>
            <div class="form-group col-lg-2">
                <label>MIDDLE NAME</label>
                <input type="text" class="form-control" name="MIDDLE NAME" value="${middleName}" ${textColor} />
            </div>
        `;

                        inputCount += 2; // Account for 3 fields added
                    } else if (key !== 'Checked In' && key !== 'Checked Out' && key !== 'Checked In By' && key !== 'Checked Out By') {
                        inputHtml = `
                                    <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <input type="text" class="form-control" name="${key}" value="${value}" ${readOnly} ${required} ${textColor} />
                                    </div>
                                `;
                    }
                }

                if (window.isCheckInOutPage) {//temporary hide fields for checkincheckout page i.e for keuler application, for revert : Just remove if condition else will be working for all scenerios.(rowHtml += inputHtml;  inputCount++; )
                    if (key !== 'LIPID PANEL' && key !== 'Cholesterol / HDL Cholesterol' && key !== 'Framingham') {
                        rowHtml += inputHtml;
                        inputCount++;
                    }
                }
                else {
                    rowHtml += inputHtml;
                    inputCount++;
                }

                // If we have 5 fields, close the row and start a new one
                if (inputCount % fieldsPerRow === 0) {
                    rowHtml += '</div><div class="row">';
                }
            });

            if (inputCount % fieldsPerRow !== 0) {
                const emptyDivsNeeded = fieldsPerRow - (inputCount % fieldsPerRow);
                for (let i = 0; i < emptyDivsNeeded; i++) {
                    rowHtml += '<div class="form-group col-lg-2"></div>';
                }
            }

            rowHtml += '</div>';
            modalContent.append(rowHtml);

            // Attach input validation listeners to all text fields
            modalContent.find('input[type="text"]').on('input', function () {
                const value = $(this).val();
                validateInput(this, value);
            });
        }
    }

    //Event listner for modal dropdown change to hide/show checkout sections controls
    modalContent.on('change', 'select', function () {
        applyDependencies(modalContent, {});
    });

    //apply rule engine on modal load
    applyDependencies(modalContent, data);

    //Event listner on Dropdown Check Out Section controls to apply dependencies based on rule engine
    $("#modalBodyContent").on('change', '.dependent-field select', function () {
        applyDependencies($("#modalBodyContent"), {});
    });

    //Event listner on Textbox Check Out Section controls to apply dependencies based on rule engine
    $("#modalBodyContent").on('input', '.dependent-field input[type="text"]', function () {
        applyDependencies($("#modalBodyContent"), {});
    });

    checkLabRequisitionField();
    checkLabNeededField();
    checkIMMNeededField();

    // After modal is populated, bind the event listener to the Dental Needed field
    const dentalNeededField = modalContent.find('select[name="Dental Needed"]');
    if (dentalNeededField.length > 0) {
        dentalNeededField.on('change', function () {
            const dentalNeededValue = $(this).val();
            if (dentalNeededValue) {
                handleBWXNeededLogicOnDentalNeeded(dentalNeededValue)
            }
        });
    }

    // After modal is populated, bind the event listener to the DOB field
    const dobField = modalContent.find('input[name="DOB"]');
    if (dobField.length > 0) {
        dobField.on('change', function () {
            const dobValue = $(this).val();
            if (dobValue) {
                handleColumnsRelatedToDob(dobValue)
                // const age = calculateAge(dobValue); // Calculate age based on DOB
                // updateFieldsBasedOnAge(age); // Update other fields based on the age
            }
        });
    }

    //****************************************************This can be needed in future*************************************************/
    // const aboField = modalContent.find('select[name="ABO"]');
    // const aboNeededField = modalContent.find('select[name="ABO Needed"]');

    // if (aboField.length > 0 && aboNeededField.length > 0) {
    //     aboField.on('change', function () {
    //         const aboValue = $(this).val();
    //         if (aboValue === "") { // If "ABO" is blank
    //             aboNeededField.val("NEEDED").change(); // Set "ABO NEEDED" to "NEEDED"
    //         } else {
    //             aboNeededField.val("N/A").change(); // Optionally reset the field if "ABO" is not blank
    //         }
    //     });
    // }

    const fieldsToCheckQuest = ['ABO Needed', 'G6PD', 'SICKLE', 'Lipid Needed'];
    const fieldsToCheckLab = ['ABO Needed', 'G6PD', 'SICKLE', 'Lipid Needed', 'HIV', 'DNA'];
    const fieldsToCheckImmunization = ['Hep B Needed', 'Hep A Needed', 'FLU Needed', 'Tet/TDP Needed', 'MMR Needed', 'Varicella Needed'];

    fieldsToCheckQuest.forEach(field => {
        const inputField = modalContent.find(`select[name="${field}"]`);
        if (inputField.length > 0) {
            inputField.on('change', checkLabRequisitionField);
        }
    });

    fieldsToCheckLab.forEach(field => {
        const inputField = modalContent.find(`select[name="${field}"]`);
        if (inputField.length > 0) {
            inputField.on('change', checkLabNeededField);
        }
    });

    fieldsToCheckImmunization.forEach(field => {
        const inputField = modalContent.find(`select[name="${field}"]`);
        if (inputField.length > 0) {
            inputField.on('change', checkIMMNeededField);
        }
    });

    const aboField = document.querySelector('[name="ABO"]');
    if (aboField) {
        aboField.addEventListener('change', function () {
            handleABOtoABONeededLogic(this.value);
        });
    }

    const nearVisionNeededField = document.querySelector('[name="NEAR VISION Needed"]');
    if (nearVisionNeededField) {
        nearVisionNeededField.addEventListener('change', function () {
            handleNearVisionNeededToVisionNeededLogic(this.value);
        });
    }

    const sexField = modalContent.find('select[name="SEX"]');
    const pregnancyTestNeededField = modalContent.find('select[name="Pregnancy Test Needed"]');

    if (sexField.length > 0 && pregnancyTestNeededField.length > 0) {
        sexField.on('change', function () {
            const selectedSex = $(this).val();
            if (selectedSex === 'M') {
                pregnancyTestNeededField.val('N/A');
                pregnancyTestNeededField.prop('disabled', true); // Make it readonly
            } else {
                pregnancyTestNeededField.prop('disabled', false); // Re-enable for other values
            }
        });

        // Trigger the logic initially in case SEX is pre-filled with F
        sexField.trigger('change');
    }
}

function handleBWXNeededLogicOnDentalNeeded(dentalNeededValue) {
    const bwxNeeded = document.querySelector('[name="BWX Needed"]');

    if (!bwxNeeded) return;

    if (dentalNeededValue === "NEEDED") {
        bwxNeeded.value = "NEEDED";
    } else {
        bwxNeeded.value = "N/A";
    }

    // Trigger change event manually in case other handlers are listening
    const event = new Event('change', { bubbles: true });
    bwxNeeded.dispatchEvent(event);
}

function handleABOtoABONeededLogic(aboValue) {
    const aboNeeded = document.querySelector('[name="ABO Needed"]');

    if (!aboNeeded) return;

    if (aboValue === "") {
        aboNeeded.value = "NEEDED";
        aboNeeded.disabled = false; // make editable
    } else {
        aboNeeded.value = "N/A";
        aboNeeded.disabled = true; // make readonly
    }

    // Trigger change event manually in case other handlers are listening
    const event = new Event('change', { bubbles: true });
    aboNeeded.dispatchEvent(event);
}

function handleNearVisionNeededToVisionNeededLogic(nearVisionNeededValue) {
    const visionNeededField = document.querySelector('[name="VISION Needed"]');

    if (!visionNeededField) return;

    if (nearVisionNeededValue === "NEEDED") {
        visionNeededField.value = "NEEDED";
        visionNeededField.disabled = true; // make editable
    } else {
        visionNeededField.value = "N/A";
        visionNeededField.disabled = false; // make readonly
    }

    // Trigger change event manually in case other handlers are listening
    //const event = new Event('change', { bubbles: true });
    //aboNeeded.dispatchEvent(event);
}

//****************************************************This can be needed in future*************************************************/
function checkLabRequisitionField() {
    const aboNeededValue = modalContent.find('select[name="ABO Needed"]').val();
    const g6pdValue = modalContent.find('select[name="G6PD"]').val();
    const sickleValue = modalContent.find('select[name="SICKLE"]').val();
    const lipidNeededValue = modalContent.find('select[name="Lipid Needed"]').val();

    const questField = modalContent.find('[name="Lab Requisition"]');

    if (aboNeededValue === "NEEDED" || g6pdValue === "NEEDED" || sickleValue === "NEEDED" || lipidNeededValue === "NEEDED") {
        questField.val("NEEDED");
    }
    else {
        questField.val("N/A");
    }
}

//****************************************************This can be needed in future*************************************************/
function checkLabNeededField() {
    const aboNeededValue = modalContent.find('select[name="ABO Needed"]').val();
    const g6pdValue = modalContent.find('select[name="G6PD"]').val();
    const sickleValue = modalContent.find('select[name="SICKLE"]').val();
    const lipidNeededValue = modalContent.find('select[name="Lipid Needed"]').val();
    const hivValue = modalContent.find('select[name="HIV"]').val();
    const dnaValue = modalContent.find('select[name="DNA"]').val();

    /*const labNeededField = modalContent.find('input[name="Lab Needed"]');*/
    const labNeededField = modalContent.find('[name="Lab Needed"]');

    if (aboNeededValue === "NEEDED" || g6pdValue === "NEEDED" || sickleValue === "NEEDED" || lipidNeededValue === "NEEDED" || hivValue === "NEEDED" || dnaValue === "NEEDED") {
        labNeededField.val("NEEDED");
    } else {
        labNeededField.val("N/A");
    }
}

function checkIMMNeededField() {
    const hepBValue = modalContent.find('select[name="Hep B Needed"]').val();
    const hepAValue = modalContent.find('select[name="Hep A Needed"]').val();
    const fluValue = modalContent.find('select[name="FLU Needed"]').val();
    const tetTdpValue = modalContent.find('select[name="Tet/TDP Needed"]').val();
    const mmrValue = modalContent.find('select[name="MMR Needed"]').val();
    const vericellaValue = modalContent.find('select[name="Varicella Needed"]').val();

    const immNeededField = modalContent.find('input[name="IMM Needed"]');

    if (hepBValue === "NEEDED" || hepAValue === "NEEDED" || fluValue === "NEEDED" || tetTdpValue === "NEEDED" || mmrValue === "NEEDED" || vericellaValue === "NEEDED") {
        immNeededField.val("NEEDED");
    } else {
        immNeededField.val("N/A");
    }
}

$(document).on("input", ".decimal-input", function () {
    let value = $(this).val();

    // Allow only numbers with a single decimal place
    let validValue = value.match(/^\d*(\.\d{0,1})?$/);

    if (!validValue) {
        $(this).val(value.slice(0, -1)); // Remove last invalid character
    }
});

// function calculateAge(dob) {
//     const birthDate = new Date(dob);
//         const today = new Date(document.getElementById("calendarInput").value);
//     let age = today.getFullYear() - birthDate.getFullYear();
//     const monthDiff = today.getMonth() - birthDate.getMonth();

//     // Adjust age if the birth month hasn't occurred yet this year
//     if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
//         age--;
//     }

//     return age;
// }

// function updateFieldsBasedOnAge(age) {

//     const ageField = document.querySelector('input[name="AGE"]');
//     if (ageField) {
//         ageField.value = age; // Set the value of the AGE field to the calculated age
//     }

//     // Update "OVER 40" field
//     const over40Field = document.querySelector('input[name="OVER 40"]');
//     if (over40Field) {
//         over40Field.value = age >= 40 ? "YES" : "NO";
//     }

//     // Update "OVER 44" field
//     const over44Field = document.querySelector('input[name="Over 44"]');
//     if (over44Field) {
//         over44Field.value = age >= 44 ? "YES" : "NO";
//     }

//     // Update fields based on age > 39.5
//     const lipidNeededField = document.querySelector('input[name="Lipid Needed"]');
//     const lipidPanelField = document.querySelector('input[name="LIPID PANEL"]');
//     const cholesterolField = document.querySelector('input[name="Cholesterol / HDL Cholesterol"]');
//     const ekgField = document.querySelector('input[name="EKG"]');
//     const ekgNeededField = document.querySelector('input[name="EKG NEEDED"]');

//     const valueForAge = age > 39.5 ? "NEEDED" : "N/A";

//     if (lipidNeededField) {
//         lipidNeededField.value = valueForAge;
//     }
//     if (lipidPanelField) {
//         lipidPanelField.value = valueForAge;
//     }
//     if (cholesterolField) {
//         cholesterolField.value = valueForAge;
//     }
//     if (ekgField) {
//         ekgField.value = valueForAge;
//     }
//     if (ekgNeededField) {
//         ekgNeededField.value = valueForAge;
//     }
// }


const validationRules = {
    "LAST NAME": { type: "alpha", allowSpecialCharacters: true, uppercase: true }, // Allow special characters in LAST NAME,
    "FIRST NAME": { type: "alpha", allowSpecialCharacters: true, uppercase: true }, // Allow special characters in FIRST NAME,
    "MIDDLE NAME": { type: "alpha", allowSpecialCharacters: true, uppercase: true }, // Allow special characters in MIDDLE NAME,
    "FULL NAME": { type: "alpha", allowSpecialCharacters: true, uppercase: true }, // Allow special characters in FULL NAME
    "FULL SSN": { type: "numeric", format: "xxx-xx-xxxx", maxLength: 11 }, // SSN field
    "DOD ID": { type: "numeric", maxLength: 10 },
    //"RANK": { type: "alphanumeric", uppercase: true, maxLength: 3 },
    //"MOS": { type: "alphanumeric", uppercase: true },
    "UIC": { type: "alphanumeric", uppercase: true, maxLength: 6},
    //"PULHES": { type: "numeric", maxLength: 5 },
    //"HRC": { type: "alphanumeric", uppercase: true },
    //"EventID": { type: "alphanumeric", uppercase: true },
    //"G6PD Status": { type: "alphanumeric", uppercase: true }
};

function validateInput(field, value) {
    const inputName = $(field).attr('name');
    const rules = validationRules[inputName];

    if (rules) {
        if (rules.uppercase) {
            value = value.toUpperCase();
            $(field).val(value);
        }

        //if (inputName === "FULL NAME" && rules.allowSpecialCharacters) {
        //    $(field).val(value);
        //}

        if (rules.type === "alpha") {
            // Remove lowercase letters and digits
            let cleanedValue = value.replace(/[a-z0-9]/g, '');

            if (cleanedValue !== value) {
                $(field).val(cleanedValue);
            }

            // Restrict to max length
            if (rules.maxLength && cleanedValue.length > rules.maxLength) {
                $(field).val(cleanedValue.slice(0, rules.maxLength));
                return false;
            } else {
                $(field).removeClass('is-invalid');
            }
        }


        // Check for numeric fields (e.g., LAST 4, DOD ID, AGE, PULHES)
        if (rules.type === "numeric") {
            // Allow only digits and restrict further input if max length is reached
            if (!/^\d*$/.test(value)) { // Allow only digits
                $(field).val(value.replace(/\D/g, '')); // Remove any non-digit characters
            }

            // Restrict input to max length for numeric fields
            if (rules.maxLength && value.length > rules.maxLength) {
                $(field).val(value.slice(0, rules.maxLength)); // Trim to max length
                return false;
            } else {
                $(field).removeClass('is-invalid');
            }
        }

        if (rules.type === "alphanumeric") {
            // Allow only letters and numbers
            let cleanedValue = value.replace(/[^a-z0-9]/gi, ''); // remove special chars

            if (rules.uppercase) {
                cleanedValue = cleanedValue.toUpperCase();
            }

            // Restrict to max length
            if (rules.maxLength && cleanedValue.length > rules.maxLength) {
                cleanedValue = cleanedValue.slice(0, rules.maxLength);
            }

            $(field).val(cleanedValue);
        }


        if (rules.format) {
            value = value.replace(/\D/g, ""); // Remove non-numeric characters

            if (rules.format === "xxx-xx-xxxx") {
                if (value.length > 3) {
                    value = value.substring(0, 3) + "-" + value.substring(3);
                }
                if (value.length > 6) {
                    value = value.substring(0, 6) + "-" + value.substring(6);
                }
                if (value.length > 11) {
                    value = value.substring(0, 11);
                }
            }

            $(field).val(value);
        }
    }

    return true;
}

$(document).on('change', '#checkedIn', function () {
    if ($(this).val() === 'Yes') {
        $('#checkedOut').prop('disabled', false);
    } else {
        $('#checkedOut').prop('disabled', true).val('No'); // Reset value to No when disabled
    }
});

function isValidDate(dateString) {
    const dateRegex = /^(0[1-9]|1[0-2])\/(0[1-9]|[12][0-9]|3[01])\/\d{4}$/;

    if (!dateRegex.test(dateString)) {
        return false; // Invalid format
    }

    const [month, day, year] = dateString.split('/').map(Number); // Convert to numbers

    const dateObj = new Date(year, month - 1, day); // Note: month is zero-based in JS Date

    return dateObj &&
        dateObj.getFullYear() === year &&
        dateObj.getMonth() + 1 === month &&
        dateObj.getDate() === day;
}

function formatDateToYYYYMMDD(dateString) {
    const [month, day, year] = dateString.split('/');
    return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
}

function formatDateToMMDDYYYY(dateString) {
    const [year, month, day] = dateString.split('-'); // Split the yyyy/mm/dd format
    return `${month}/${day}/${year}`; // Return as mm/dd/yyyy
}




function AdjustWidth() {
    setTimeout(function () {
        $('#previewTable').DataTable().columns.adjust().draw();
    }, 10);
}

/*let walkInSmCount = 0;*/
async function saveChangesButton() {

    const modalInputs = $('#editModal').find('input, select, textarea');
    const updatedData = {};

    if (window.isCheckInOutPage) {
        const checkedInDropdown = document.getElementById("checkedIn");
        const checkedOutDropdown = document.getElementById("checkedOut");

        if (checkedInDropdown.value === "Yes") {
            $("#checkedInBy").prop("required", true);
        } else {
            $("#checkedInBy").prop("required", false).val('');
        }

        // Checked Out handling
        if (checkedOutDropdown.value === "Yes") {
            $("#checkedOutBy").prop("required", true);
        } else {
            $("#checkedOutBy").prop("required", false).val('');
        }
    }
    let hasError = false;
    let firstInvalidField = null;

    // Clear previous highlights
    modalInputs.removeClass('highlight-error');

    modalInputs.each(function () {
        const key = $(this).attr('name');
        const value = $(this).val() || '';

        if (isDateField(key) && isValidDateOnSavingEditModal(value)) {
            updatedData[key] = formatDateToMMDDYYYY(value);
        } else {
            updatedData[key] = value;
        }

        // 🔹 Highlight required fields based on the required attribute
        if ($(this).prop('required') && value.trim() === '') {
            $(this).removeClass('valid-class').addClass('highlight-error');
            if (!hasError) {
                firstInvalidField = this; // store first invalid field
            }
            hasError = true;
        }
    });

    // If any required field is missing, do not proceed further
    if (hasError) {
        if (firstInvalidField) {
            $(firstInvalidField).focus()[0].scrollIntoView({
                behavior: "smooth",
                block: "center"
            });
        }
        return;
    }

    //let newRowData = new Array(79).fill('');
    const fullSsnValue = updatedData['FULL SSN'];
    const last4Index = keys.indexOf('LAST 4');

    if (isDuplicateDodId(updatedData, isAddingNewRow, keys)) {
        alert('This DOD ID already exists in this sheet.');
        return;
    }

    const shouldPrint = await showYesNoModal("Would you like to print Service Routing Sheet?");
    let smIdToIdentifyRecordForPrint = null;
    if (isAddingNewRow) {
        // Initialize a full row with empty values
        const fullRowData = Array(keys.length).fill('');

        // set sm id counter in index 0 (for edit mode)
        smIdCounter++;
        const smIdIndex = keys.indexOf('SM ID');// Find index of FULL SSN column
        fullRowData[smIdIndex] = smIdCounter.toString();
        smIdToIdentifyRecordForPrint = smIdCounter.toString();

        if (last4Index !== -1 && fullSsnValue) {
            const updatedLast4 = fullSsnValue.slice(-4);
            fullRowData[last4Index] = updatedLast4;
        }




        // Fill in only the known fields using tableToKeysIndexMap
        tableToKeysIndexMap.forEach((keyIndex, modalIndex) => {
            if (keyIndex !== -1) {
                const fieldKey = keys[keyIndex];
                fullRowData[keyIndex] = updatedData[fieldKey] || '';
            }
        });

        //First name, last name and middle name handling for FULL NAME
        const lastNameValue = (updatedData['LAST NAME'] || '').trim();
        const firstNameValue = (updatedData['FIRST NAME'] || '').trim();
        const middleNameValue = (updatedData['MIDDLE NAME'] || '').trim();
        const fullNameIndex = keys.indexOf('FULL NAME');

        // Filter out any empty parts and join with a space
        const fullName = [lastNameValue, firstNameValue, middleNameValue].filter(Boolean).join(' ');

        fullRowData[fullNameIndex] = fullName;

        const checkedInIndex = keys.indexOf('Checked In');
        const checkedOutIndex = keys.indexOf('Checked Out');

        fullRowData[checkedInIndex] = $('#checkedIn').val();
        fullRowData[checkedOutIndex] = $('#checkedOut').val();

        const checkedInByIndex = keys.indexOf('Checked In By');
        const checkedOutByIndex = keys.indexOf('Checked Out By');

        fullRowData[checkedInByIndex] = $('#checkedInBy').val();
        fullRowData[checkedOutByIndex] = $('#checkedOutBy').val();

        const checkedInTimeIndex = keys.indexOf('Checked In Time');

        if ($('#checkedIn').val() === "Yes") {
            fullRowData[checkedInTimeIndex] = formatDateTime24(new Date());
        } else {
            fullRowData[checkedInTimeIndex] = "";
        }

        const checkedOutTimeIndex = keys.indexOf('Checked Out Time');

        if ($('#checkedOut').val() === "Yes") {
            fullRowData[checkedOutTimeIndex] = formatDateTime24(new Date());
        } else {
            fullRowData[checkedOutTimeIndex] = "";
        }

        if (window.isCheckInOutPage) {
            const walkinSMIndex = keys.indexOf('Walk-In Service Member');
            fullRowData[walkinSMIndex] = 'Yes';
        }

        const table = $('#previewTable').DataTable();
        const barcodeIndex = keys.indexOf('Barcode');
        const barcodeValue = table.cell(0, barcodeIndex).data();

        let finalBarcodeValue = '';

        if (barcodeValue && barcodeValue.includes('-')) {
            finalBarcodeValue = barcodeValue.split('-')[0] + '-' + smIdCounter.toString().padStart(5, '0');
        } else {
            finalBarcodeValue = barcodeValue + '-' + smIdCounter.toString().padStart(5, '0');
        }

        fullRowData[barcodeIndex] = finalBarcodeValue;

        if (window.userType === "client") {
            const dtoObject = prepareObjectToAddRecordInDatabase(fullRowData, keys);

            const result = await addSingleRecordInDatabase('/ExcelFileUploader/InsertSingleRecord', dtoObject);

            if (result.success) {
                

                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: result.data.message || 'Record inserted successfully',
                    timer: 2000,
                    showConfirmButton: false
                });

                smIdToIdentifyRecordForPrint = result.data.data.smId;
                const selectedEventId = $('#eventIdDropdown').val();
                await fetchAndRenderEventData(selectedEventId, async () => {
                    await printSpecificRowIfNeeded(shouldPrint, smIdToIdentifyRecordForPrint);
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: result.status === 500 ? 'Server Error' : 'Network Error',
                    text: result.data?.message || result.error?.message || 'Something went wrong.'
                });
            }
        } else {
            $('#previewTable').DataTable().row.add(fullRowData).draw(false);
            await printSpecificRowIfNeeded(shouldPrint, smIdToIdentifyRecordForPrint);
        }

        //if (window.isCheckInOutPage) {
        //    //walkInServiceMemberCount++;
        //}
    }
    else {
        if (last4Index !== -1 && fullSsnValue) {
            const updatedLast4 = fullSsnValue.slice(-4);
            updatedData['LAST 4'] = updatedLast4;
        }

        updatedData['Checked In'] = $('#checkedIn').val();
        updatedData['Checked Out'] = $('#checkedOut').val();

        updatedData['Checked In By'] = $('#checkedInBy').val();
        updatedData['Checked Out By'] = $('#checkedOutBy').val();

        if ($('#checkedIn').val() === "Yes") {
            updatedData['Checked In Time'] = formatDateTime24(new Date());
        }
        else {
            updatedData['Checked In Time'] = "";
        }

        if ($('#checkedOut').val() === "Yes") {
            updatedData['Checked Out Time'] = formatDateTime24(new Date());

        }
        else {
            updatedData['Checked Out Time'] = "";
        }

        //keys.forEach((key, index) => {
        //    if (updatedData[key] !== undefined) {
        //        // Always update if key is 'Checked In By' or 'Checked Out By'
        //        if (key === 'ABO' || key === 'Checked In By' || key === 'Checked Out By' || key === 'Checked In Time' || key === 'Checked Out Time' || updatedData[key].trim() !== '') {
        //            currentRow.find('td').eq(index).text(updatedData[key]);
        //        }
        //    }
        //});

        if (window.userType === "client") {
            const dto = prepareObjectToUpdateRecordInDatabase(updatedData, columnMappingsForInsertionAndFetching);
            const result = await updateSingleRecordInDatabase('/ExcelFileUploader/UpdateSingleRecord', dto);

            if (result.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: result.data.message || 'Record updated successfully',
                    timer: 2000,
                    showConfirmButton: false
                });

                smIdToIdentifyRecordForPrint = dto.SmId;
                const selectedEventId = $('#eventIdDropdown').val();
                await fetchAndRenderEventData(selectedEventId, async () => {
                    await printSpecificRowIfNeeded(shouldPrint, smIdToIdentifyRecordForPrint);
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: result.status === 500 ? 'Server Error' : 'Network Error',
                    text: result.data?.message || result.error?.message || 'Something went wrong.'
                });
            }

        }
        else {
            keys.forEach((key, index) => {
                if (key === 'SM ID') {
                    smIdToIdentifyRecordForPrint = currentRow.find('td').eq(keys.indexOf('SM ID')).text().trim();
                }
                if (updatedData[key] !== undefined) {
                    const currentCellValue = currentRow.find('td').eq(index).text().trim();

                    // Skip update if key is Checked In Time or Checked Out Time and already has a value
                    if (
                        (key === 'Checked In Time' && currentCellValue !== '') || (key === 'Checked Out Time' && currentCellValue !== '')
                    ) {
                        return; // Skip this iteration
                    }

                    // Always update for these keys or if value is non-empty
                    if (
                        key === 'ABO' || key === 'Checked In By' || key === 'Checked Out By' || updatedData[key].trim() !== ''
                    ) {
                        currentRow.find('td').eq(index).text(updatedData[key]);
                    }
                }
            });

            await printSpecificRowIfNeeded(shouldPrint, smIdToIdentifyRecordForPrint);
        }
    }

    AdjustWidth();
    isAddingNewRow = false;

    $('#editModal').modal('hide');

    if (window.isCheckInOutPage && window.userType !== "client") {
        RenderUpdatedEventSummaryTable();

        //UpdateExcelFile();
    }
}

async function printSpecificRowIfNeeded(shouldPrint, smIdToIdentifyRecordForPrint) {
    if (!shouldPrint) return;

    const table = $('#previewTable').DataTable();
    const rows = table.rows().nodes().toArray(); // get all row DOM nodes

    for (let row of rows) {
        const $row = $(row);
        const data = table.row($row).data(); // get data for this row
        const smId = data[keys.indexOf('SM ID')];

        if (smId == smIdToIdentifyRecordForPrint) {
            const rowData = getRowData($row);
            await generatePDF([rowData], true);
            break; // stop looping after printing
        }
    }
}

function formatDateTime24(date) {
    const pad = (num) => num.toString().padStart(2, '0');

    const month = pad(date.getMonth() + 1);  // Months are zero-based
    const day = pad(date.getDate());
    const year = date.getFullYear();

    const hours = pad(date.getHours());       // 24-hour format by default
    const minutes = pad(date.getMinutes());
    const seconds = pad(date.getSeconds());

    return `${month}/${day}/${year} ${hours}:${minutes}:${seconds}`;
}

function isDuplicateDodId(updatedData, isAddingNewRow, keys) {
    const table = $('#previewTable').DataTable();
    const dodIdToCheck = updatedData['DOD ID'];
    const dodIdColumnIndex = keys.indexOf('DOD ID');
    const smIdColumnIndex = keys.indexOf('SM ID');

    let isDuplicate = false;

    table.rows().every(function () {
        const rowData = this.data(); // rowData is an array, not an object

        const dodId = rowData[dodIdColumnIndex];
        const smId = rowData[smIdColumnIndex];

        if (isAddingNewRow) {
            // For new rows, just compare DOD ID
            if (dodId === dodIdToCheck) {
                isDuplicate = true;
                return false; // break loop
            }
        } else {
            // For editing, skip same SM ID row
            if (smId != smIdEditing && dodId === dodIdToCheck) {
                isDuplicate = true;
                return false; // break loop
            }
        }
    });

    return isDuplicate;
}










function isDateField(fieldName) {
    const dateFields = ['DOB', 'PANO Needed', 'LIPID PANEL', 'EKG (Date)']; // Replace with actual date field names
    return dateFields.includes(fieldName);
}

function isValidDateOnSavingEditModal(value) {
    const date = new Date(value);
    return !isNaN(date.getTime()); // Returns true if it's a valid date
}

const { jsPDF } = window.jspdf;

// 📌 Print Single Row
$(document).on("click", ".btn-print", async function () {
    var row = $(this).closest("tr");
    var rowData = getRowData(row);
    await generatePDF([rowData], true);  // Print mode
});

$("#btnDownloadPDF").click(async function () {
    var allRowsData = [];

    $("#previewTable tbody tr").each(function () {
        var rowData = getRowData($(this));
        allRowsData.push(rowData);
    });

    if (allRowsData.length > 0) {
        allRowsData.sort((a, b) => a.fullName.localeCompare(b.fullName));
        await generatePDF(allRowsData, false); // Pass all rows and save PDF
    } else {
        alert("No data available for download.");
    }
});



function getRowData(row) {
    var table = row.closest("table"); // Get the table reference

    var fullNameIndex = getColumnIndex(table, "FULL NAME");
    var dodIdIndex = getColumnIndex(table, "DOD ID");
    var last4Index = getColumnIndex(table, "LAST 4");
    var bwxNeededIndex = getColumnIndex(table, "BWX Needed");
    var dentalNeededIndex = getColumnIndex(table, "Dental Needed");
    var panoNeededIndex = getColumnIndex(table, "PANO Needed");
    var visionNeededIndex = getColumnIndex(table, "VISION");
    var labNeededIndex = getColumnIndex(table, "Lab Needed");
    var barcodeIndex = getColumnIndex(table, "Barcode");
    var visionNeededIndex = getColumnIndex(table, "VISION Needed");
    var checkedInDateTimeIndex = getColumnIndex(table, "Checked In Time");
    var hearingNeededIndex = getColumnIndex(table, "HEARING Needed");
    var phaNeededIndex = getColumnIndex(table, "PHA Needed");
    var hivIndex = getColumnIndex(table, "HIV");
    var dnaIndex = getColumnIndex(table, "DNA");
    var g6pdIndex = getColumnIndex(table, "G6PD");
    var aboNeededIndex = getColumnIndex(table, "ABO Needed");
    var sickleIndex = getColumnIndex(table, "SICKLE");
    var lipidNeededIndex = getColumnIndex(table, "Lipid Needed");
    var ekgIndex = getColumnIndex(table, "EKG NEEDED");
    var pregnancyTestNeededIndex = getColumnIndex(table, "Pregnancy Test Needed");
    var immNeededIndex = getColumnIndex(table, "IMM Needed");
    var fluNeededIndex = getColumnIndex(table, "FLU Needed");
    var tetTdpIndex = getColumnIndex(table, "Tet/TDP Needed");
    var hepAIndex = getColumnIndex(table, "Hep A Needed");
    var mmrIndex = getColumnIndex(table, "MMR Needed");
    var hepBIndex = getColumnIndex(table, "Hep B Needed");
    var varicellaIndex = getColumnIndex(table, "Varicella Needed");

    return {
        fullName: fullNameIndex !== -1 ? row.find("td").eq(fullNameIndex).text().trim() : "N/A",
        dodId: dodIdIndex !== -1 ? row.find("td").eq(dodIdIndex).text().trim() : "N/A",
        last4: last4Index !== -1 ? row.find("td").eq(last4Index).text().trim() : "N/A",
        bwxNeeded: bwxNeededIndex !== -1 ? row.find("td").eq(bwxNeededIndex).text().trim() : "N/A",
        dentalNeeded: dentalNeededIndex !== -1 ? row.find("td").eq(dentalNeededIndex).text().trim() : "N/A",
        panoNeeded: panoNeededIndex !== -1 ? row.find("td").eq(panoNeededIndex).text().trim() : "N/A",
        visionNeeded: visionNeededIndex !== -1 ? row.find("td").eq(visionNeededIndex).text().trim() : "N/A",
        labNeeded: labNeededIndex !== -1 ? row.find("td").eq(labNeededIndex).text().trim() : "N/A",
        immNeeded: immNeededIndex !== -1 ? row.find("td").eq(immNeededIndex).text().trim() : "N/A",
        barcode: barcodeIndex !== -1 ? row.find("td").eq(barcodeIndex).text().trim() : "0",
        visionNeeded: visionNeededIndex !== -1 ? row.find("td").eq(visionNeededIndex).text().trim() : "N/A",
        checkedInDateTime: checkedInDateTimeIndex !== -1 ? row.find("td").eq(checkedInDateTimeIndex).text().trim() : "",
        hearingNeeded: hearingNeededIndex !== -1 ? row.find("td").eq(hearingNeededIndex).text().trim() : "N/A",
        phaNeeded: phaNeededIndex !== -1 ? row.find("td").eq(phaNeededIndex).text().trim() : "N/A",
        hiv: hivIndex !== -1 ? row.find("td").eq(hivIndex).text().trim() : "N/A",
        dna: dnaIndex !== -1 ? row.find("td").eq(dnaIndex).text().trim() : "N/A",
        g6pd: g6pdIndex !== -1 ? row.find("td").eq(g6pdIndex).text().trim() : "N/A",
        aboNeeded: aboNeededIndex !== -1 ? row.find("td").eq(aboNeededIndex).text().trim() : "N/A",
        sickle: sickleIndex !== -1 ? row.find("td").eq(sickleIndex).text().trim() : "N/A",
        lipidNeeded: lipidNeededIndex !== -1 ? row.find("td").eq(lipidNeededIndex).text().trim() : "N/A",
        ekg: ekgIndex !== -1 ? row.find("td").eq(ekgIndex).text().trim() : "N/A",
        pregnancyTestNeeded: pregnancyTestNeededIndex !== -1 ? row.find("td").eq(pregnancyTestNeededIndex).text().trim() : "N/A",
        immNeeded: immNeededIndex !== -1 ? row.find("td").eq(immNeededIndex).text().trim() : "N/A",
        fluNeeded: fluNeededIndex !== -1 ? row.find("td").eq(fluNeededIndex).text().trim() : "N/A",
        tetTdp: tetTdpIndex !== -1 ? row.find("td").eq(tetTdpIndex).text().trim() : "N/A",
        hepA: hepAIndex !== -1 ? row.find("td").eq(hepAIndex).text().trim() : "N/A",
        mmr: mmrIndex !== -1 ? row.find("td").eq(mmrIndex).text().trim() : "N/A",
        hepB: hepBIndex !== -1 ? row.find("td").eq(hepBIndex).text().trim() : "N/A",
        varicella: varicellaIndex !== -1 ? row.find("td").eq(varicellaIndex).text().trim() : "N/A",
    };
}

function getColumnIndex(table, columnName) {
    var index = -1;
    table.find("thead th").each(function (i) {
        if ($(this).text().trim() === columnName) {
            index = i;
        }
    });
    return index;
}

//document.getElementById('addRowButtonC').addEventListener('click', function () {
//    addRow();
//});

$(document).on('click', '.add-row-button', function () {
    addRow();
});

function addRow() {
    isAddingNewRow = true;
    const emptyData = {};
    populateModalForAdd(emptyData);
    $('#checkedIn').val('No');
    $('#checkedOut').val('No').prop('disabled', true);
    $("#checkedInBy").val('');
    $("#checkedOutBy").val('');
    document.getElementById("editModalLabel").innerText = "Add Walk-In Service Member";
    $('#editModal').modal('show');
}

function handleColumnsRelatedToDob(dob) {
    if (!dob) return; // Ensure DOB is provided
    let dobDate = new Date(dob);
    let today;

    if (window.isCheckInOutPage) {
        //var table = $('#previewTable').DataTable();
        //today = new Date(table.row(0).data()[65]);
        const table = $('#previewTable').DataTable();
        const firstRowData = table.row(0).data();
        if (!firstRowData) return;

        // Get "EventDate" using keys instead of headers/index
        const eventDateIndex = keys.indexOf("EventDate");
        if (eventDateIndex === -1) return;

        const eventDateValue = firstRowData[eventDateIndex];
        if (!eventDateValue) return;

        today = new Date(eventDateValue);
    }
    else {
        today = new Date(document.getElementById("calendarInput").value);
    }

    let exactAge = (today - dobDate) / (365.25 * 24 * 60 * 60 * 1000); // Age calculation
    let ageWithGrace = ((new Date(today.setDate(today.getDate() + 14))) - dobDate) / (365.25 * 24 * 60 * 60 * 1000);

    const ageField = document.querySelector('input[name="AGE"]');
    if (ageField) {
        ageField.value = Math.floor(exactAge); // Set the value of the AGE field to the calculated age
    }

    const over40Field = document.querySelector('input[name="OVER 40"]');
    if (over40Field) {
        over40Field.value = exactAge > 39.5 ? "YES" : "NO";
    }

    const over44Field = document.querySelector('input[name="Over 44"]');
    if (over44Field) {
        over44Field.value = ageWithGrace >= 45 ? "YES" : "NO";
    }

    const nearVisionField = document.querySelector('[name="NEAR VISION Needed"]');
    if (nearVisionField) {
        nearVisionField.value = ageWithGrace >= 45 ? "NEEDED" : "N/A";
        handleNearVisionNeededToVisionNeededLogic(nearVisionField.value);
    }

    const lipidNeededField = document.querySelector('[name="Lipid Needed"]');
    const lipidPanelField = document.querySelector('[name="LIPID PANEL"]');
    const cholesterolField = document.querySelector('[name="Cholesterol / HDL Cholesterol"]');
    const ekgField = document.querySelector('[name="EKG (Date)"]');
    const ekgNeededField = document.querySelector('[name="EKG NEEDED"]');
    const framinghamField = document.querySelector('[name="Framingham"]');


    if (isAddingNewRow || window.isCheckInOutPage) {
        const valueForAge = exactAge > 39.5 ? "NEEDED" : "N/A";

        if (lipidNeededField) {
            lipidNeededField.value = valueForAge;
        }
        if (lipidPanelField) {
            lipidPanelField.value = valueForAge;
        }
        if (cholesterolField) {
            cholesterolField.value = valueForAge;
        }
        if (ekgField) {
            ekgField.value = valueForAge;
        }
        if (ekgNeededField) {
            ekgNeededField.value = valueForAge;
        }
        if (framinghamField) {
            framinghamField.value = valueForAge; // Set empty value
        }
    }
    else {
        const valueForAge = exactAge > 39.5 ? "NEEDED" : "N/A";

        if (lipidNeededField) {
            lipidNeededField.value = valueForAge;
        }
        if (lipidPanelField) {
            if (exactAge > 39.5) {
                lipidPanelField.type = "date"; // Make it a calendar input
                lipidPanelField.value = "";
                lipidPanelField.readOnly = false;
            } else {
                lipidPanelField.type = "text"; // Make it a text input
                lipidPanelField.value = "N/A";
                lipidPanelField.readOnly = true;
            }
        }
        if (cholesterolField) {
            if (exactAge > 39.5) {
                cholesterolField.value = ""; // Set empty value
                cholesterolField.readOnly = false; // Make it editable
            } else {
                cholesterolField.value = "N/A"; // Set value to N/A
                cholesterolField.readOnly = true; // Make it readonly
            }
        }
        if (ekgField) {
            if (exactAge > 39.5) {
                ekgField.type = "date"; // Make it a calendar input
                ekgField.value = "";
                ekgField.readOnly = false;
            } else {
                ekgField.type = "text"; // Make it a text input
                ekgField.value = "N/A";
                ekgField.readOnly = true;
            }
        }
        if (ekgNeededField) {
            ekgNeededField.value = valueForAge;
        }
        if (framinghamField) {
            if (exactAge > 39.5) {
                framinghamField.type = "number";
                framinghamField.value = ""; // Set empty value
                framinghamField.readOnly = false; // Make it editable
            } else {
                framinghamField.type = "text";
                framinghamField.value = "N/A"; // Set value to N/A
                framinghamField.readOnly = true; // Make it readonly
            }
        }
    }
}

////////////////////////////////////////////////////////////Routing Sheet Methods Start////////////////////////////////////////////////////////////////////
async function generatePDF(dataArray = [], isPrintMode = false) {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();
    const dawsonLogoBase64 = await getImageBase64FromUrl("/images/Dawson-Logo.png");
    const malamaLogoBase64 = await getImageBase64FromUrl("/images/MALAMA-Horizontal-With-Tagline.png");

    /* dataArray.forEach((data, index) => {*/
    for (const [index, data] of dataArray.entries()) {
        if (index > 0) doc.addPage();  // New page for each row
        //font information
        let fontStyle = 'Times';
        let mainHeadingFontSize = 24;
        let headingFontSize = 14;
        let fieldsFontSize = 12;

        // x alignment
        let generalInformationFirstColumnX = 25;
        let generalInformationSecondColumnX = 115;
        let headingFirstColumnX = 35;
        let headingSecondColumnX = 115;
        let lineStartX = 35;
        let lineEndX = 175;
        let fieldsFirstColumnX = 45;
        let fieldsSecondColumnX = 115;
        let y = 41;
        let distanceInLines = 6;

        const headerImgWidth = 30;
        const headerImgHeight = 15;

        // Add Dawson logo at top-left
        doc.addImage(dawsonLogoBase64, 'PNG', 10, 10, headerImgWidth, headerImgHeight);

        // Main Heading Start
        doc.setFontSize(mainHeadingFontSize);
        doc.setFont(fontStyle, "bold");

        // Center text horizontally
        const pageWidth = doc.internal.pageSize.getWidth();
        const titleText = "Event Service Checklist";
        const textWidth = doc.getTextWidth(titleText);
        const centerX = (pageWidth - textWidth) / 2;
        const titleY = 22; // Vertically aligned with logo
        doc.text(titleText, centerX, titleY);
        // Main Heading End

        // General Information Start
        y = drawGeneralInformation(doc, fontStyle, data, generalInformationFirstColumnX, generalInformationSecondColumnX, headingFontSize, y, distanceInLines);
        // General Information End

        // Vitals Start
        y = drawVitalsSection(doc, data, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines);
        // Vitals End

        // Vision Start
        if ((data.visionNeeded || '').toLowerCase() === 'needed') {
            y = drawVisionSection(doc, fontStyle, headingFontSize, headingFirstColumnX, headingSecondColumnX, lineStartX, lineEndX, y, distanceInLines);
        }
        // Vision End

        // Labs Start
        if ((data.labNeeded || '').toLowerCase() === 'needed') {
            y = drawLabsSection(doc, data, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines);
        }

        // Labs End

        // Immunizations Start

        if ((data.immNeeded || '').toLowerCase() === 'needed') {
            y = drawImmunizationSection(doc, data, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines);
        }
        // Immunizations End

        //Dental Services Start

        if ((data.dentalNeeded || '').toLowerCase() === 'needed') {
            y = drawDentalServicesSection(doc, data, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines);

        }

        //Dental Services End

        //Audio Services Start
        if ((data.hearingNeeded || '').toLowerCase() === 'needed') {
            y = drawAudioServicesSection(doc, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines);
        }

        //Audio Services End

        //PHA Start
        if ((data.phaNeeded || '').toLowerCase() === 'needed') {
            y = drawPhaSection(doc, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines);
        }
        //PHA End

        y += 15;
        //doc.setFontSize(fontSize);
        doc.setFont(fontStyle, "bold");
        doc.text("CHECKOUT ___________", fieldsSecondColumnX, y);
        doc.setFont(fontStyle, "normal");
        y += 8;
        doc.setFontSize(10);
        doc.text("Attend only the stations/services listed on this form.", fieldsFirstColumnX, y);
        y += 6;
        doc.text("", 10, y);

        y += 10;



        const pageHeight = doc.internal.pageSize.getHeight();
        const imgWidth = 40;
        const imgHeight = 20;

        // Draw logo on bottom-left corner
        doc.addImage(malamaLogoBase64, 'PNG', 10, pageHeight - imgHeight - 10, imgWidth, imgHeight);

        // Generate barcode and add to center of footer
        if (data.barcode) {
            const barcodeBase64 = await generateBarcodeBase64(data.barcode);
            const barcodeWidth = 100;
            const barcodeHeight = 20;
            const barcodeX = (doc.internal.pageSize.getWidth() - barcodeWidth) / 2;
            const barcodeY = doc.internal.pageSize.getHeight() - barcodeHeight - 5; // 5 units above bottom

            doc.addImage(barcodeBase64, 'PNG', barcodeX, barcodeY, barcodeWidth, barcodeHeight);
        }

        doc.setFontSize(6);
        doc.text("Ver. 1 061025", pageWidth - 40, pageHeight - 5);
    };

    if (isPrintMode) {
        //window.open(doc.output('bloburl'), '_blank'); // Open in new tab to print
        const pdfUrl = doc.output('bloburl');
        window.open(pdfUrl, 'pdfPopupWindow', 'width=800,height=600,resizable=yes,scrollbars=yes');
    } else {
        doc.save('Service_Routing_Sheet.pdf');
    }
}

function generateBarcodeBase64(text) {
    return new Promise((resolve) => {
        const canvas = document.createElement("canvas");
        JsBarcode(canvas, text, {
            format: "CODE128",
            displayValue: true,
            height: 40,
            width: 2,
            margin: 0
        });
        resolve(canvas.toDataURL("image/png"));
    });
}

function getImageBase64FromUrl(url) {
    return new Promise((resolve, reject) => {
        const img = new Image();
        img.crossOrigin = "Anonymous"; // Important for local or cross-origin images

        img.onload = function () {
            const canvas = document.createElement("canvas");
            canvas.width = img.width;
            canvas.height = img.height;
            const ctx = canvas.getContext("2d");
            ctx.drawImage(img, 0, 0);
            const dataURL = canvas.toDataURL("image/png");
            resolve(dataURL);
        };

        img.onerror = function (err) {
            reject("Failed to load image: " + err);
        };

        img.src = url;
    });
}


function drawCheckbox(doc, x, y, fontSize, label) {
    const boxSize = 5;
    doc.rect(x, y, boxSize, boxSize);
    doc.text(label, x + boxSize + 3, y + boxSize);
}

function drawGeneralInformation(doc, fontStyle, data, colX1, colX2, fontSize, y, distanceInLines) {
    let formattedDate = "";
    let formattedTime = "";

    if (data.checkedInDateTime) {
        const datetime = new Date(data.checkedInDateTime);
        if (!isNaN(datetime)) {
            formattedDate = datetime.toLocaleDateString('en-US', {
                month: '2-digit',
                day: '2-digit',
                year: 'numeric'
            });

            formattedTime = datetime.toLocaleTimeString('en-GB', {
                hour: '2-digit',
                minute: '2-digit',
                hour12: false
            });
        }
    }

    doc.setFontSize(fontSize);
    doc.setFont(fontStyle, "normal");
    doc.text(`Name: ${data.fullName || "N/A"}`, colX1, 45);
    y += distanceInLines;
    doc.text(`DoD ID/Last 4: ${data.dodId || "________"}/${data.last4 || "____"}`, colX1, 51);
    y += distanceInLines;
    doc.text(`Time In: ${formattedTime}`, colX1, 57);
    y += distanceInLines;
    doc.text(`Date of Service: ${formattedDate}`, colX1, 63);

    return y;
}

function drawVitalsSection(doc, data, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines) {
    doc.setFont(fontStyle, "bold");
    y += distanceInLines;
    y += distanceInLines;
    doc.text("Vitals __________", headingFirstColumnX, y);

    y += distanceInLines;
    doc.setFont(fontStyle, "normal");
    drawCheckbox(doc, fieldsFirstColumnX, y, fieldsFontSize, 'Height/Weight');


    if ((data.dentalNeeded || '').toLowerCase() === 'needed' || (data.phaNeeded || '').toLowerCase() === 'needed') {
        drawCheckbox(doc, fieldsSecondColumnX, y, fieldsFontSize, 'Blood Pressure');
    }


    y += 8;
    doc.setLineDashPattern([1, 1], 0);
    doc.line(lineStartX, y, lineEndX, y);
    doc.setLineDashPattern([], 0);

    y += distanceInLines;
    return y;
}


function drawVisionSection(doc, fontStyle, headingFontSize, headingX1, headingX2, lineStartX, lineEndX, currentY, distanceInLines) {
    doc.setFont(fontStyle, "bold");
    doc.setFontSize(headingFontSize);

    doc.text("Vision Screening ___________", headingX1, currentY);
    doc.text("Optometrist ___________", headingX2, currentY);

    currentY += distanceInLines;
    doc.setLineDashPattern([1, 1], 0);
    doc.line(lineStartX, currentY, lineEndX, currentY);
    doc.setLineDashPattern([], 0);
    doc.setFont(fontStyle, "normal");
    return currentY; // return updated Y position
}

function drawLabsSection(doc, data, fontStyle, headingFontSize, fieldsFontSize, headingX, fieldX1, fieldX2, lineStartX, lineEndX, currentY, distanceInLines) {
    doc.setFont(fontStyle, "bold");
    currentY += distanceInLines;

    doc.setFontSize(headingFontSize);
    doc.text("Labs __________", headingX, currentY);

    doc.setFontSize(fieldsFontSize);
    doc.setFont(fontStyle, "normal");

    // Step 1: Build array of only the needed items
    const labs = [];
    if ((data.hiv || '').toLowerCase() === 'needed') labs.push("HIV");
    if ((data.dna || '').toLowerCase() === 'needed') labs.push("DNA");
    if ((data.g6pd || '').toLowerCase() === 'needed') labs.push("G6PD");
    if ((data.aboNeeded || '').toLowerCase() === 'needed') labs.push("ABO");
    if ((data.sickle || '').toLowerCase() === 'needed') labs.push("Sickle Cell Test");
    if ((data.pregnancyTestNeeded || '').toLowerCase() === 'needed') labs.push("Pregnancy");// it is pending because i am unable to find pregnancy column
    if ((data.lipidNeeded || '').toLowerCase() === 'needed') labs.push("Lipid Panel");
    if ((data.ekg || '').toLowerCase() === 'needed') labs.push("EKG");

    currentY += distanceInLines;
    // Step 2: Draw two per row
    for (let i = 0; i < labs.length; i += 2) {
        const label1 = labs[i];
        const label2 = labs[i + 1]; // May be undefined

        drawCheckbox(doc, fieldX1, currentY, fieldsFontSize, label1);
        if (label2) drawCheckbox(doc, fieldX2, currentY, fieldsFontSize, label2);

        currentY += distanceInLines;
    }

    // Final line
    currentY += 8;
    doc.setLineDashPattern([1, 1], 0);
    doc.line(lineStartX, currentY, lineEndX, currentY);
    doc.setLineDashPattern([], 0);

    return currentY;
}

function drawImmunizationSection(doc, data, fontStyle, headingFontSize, fieldsFontSize, headingX, fieldX1, fieldX2, lineStartX, lineEndX, currentY, distanceInLines) {
    currentY += distanceInLines;

    doc.setFontSize(headingFontSize);
    doc.setFont(fontStyle, "bold");
    doc.text("Immunizations __________", headingX, currentY);

    doc.setFontSize(fieldsFontSize);
    doc.setFont(fontStyle, "normal");

    const items = [];
    if ((data.fluNeeded || '').toLowerCase() === 'needed') items.push("Influenza");
    if ((data.tetTdp || '').toLowerCase() === 'needed') items.push("TDAP");
    if ((data.hepA || '').toLowerCase() === 'needed') items.push("Hepatitis A");
    if ((data.mmr || '').toLowerCase() === 'needed') items.push("MMR");
    if ((data.hepB || '').toLowerCase() === 'needed') items.push("Hepatitis B");
    if ((data.varicella || '').toLowerCase() === 'needed') items.push("Varicella");

    // Draw 2 per row
    for (let i = 0; i < items.length; i += 2) {
        const label1 = items[i];
        const label2 = items[i + 1];

        currentY += distanceInLines;
        drawCheckbox(doc, fieldX1, currentY, fieldsFontSize, label1);
        if (label2) drawCheckbox(doc, fieldX2, currentY, fieldsFontSize, label2);
    }

    // Final dotted line
    currentY += 8;
    doc.setLineDashPattern([1, 1], 0);
    doc.line(lineStartX, currentY, lineEndX, currentY);
    doc.setLineDashPattern([], 0);

    return currentY;
}

function drawDentalServicesSection(doc, data, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines) {
    y += distanceInLines;
    doc.setFontSize(headingFontSize);
    doc.setFont(fontStyle, "bold");
    doc.text("Dental Services __________", headingFirstColumnX, y);
    doc.setFontSize(fieldsFontSize);
    doc.setFont(fontStyle, "normal");

    y += distanceInLines;
    doc.setFontSize(fieldsFontSize);
    doc.text("Step 1 : Dental X-ray __________", fieldsFirstColumnX, y);
    doc.text("Dental Treatment __________", fieldsSecondColumnX, y);
    y += distanceInLines;

    if ((data.panoNeeded || '').toLowerCase() === 'needed') {
        doc.text("Panoramic X-Ray __________", fieldsFirstColumnX + 14, y);
        y += distanceInLines;
    }
    doc.text("Step 2 : Dental Exams __________", fieldsFirstColumnX, y);
    y += distanceInLines;
    doc.setLineDashPattern([1, 1], 0);
    doc.line(lineStartX, y, lineEndX, y);
    doc.setLineDashPattern([], 0);

    return y; // Return updated y to maintain flow in main method
}

function drawAudioServicesSection(doc, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines) {
    y += distanceInLines;
    doc.setFontSize(headingFontSize);
    doc.setFont(fontStyle, "bold");
    doc.text("Audio Services", headingFirstColumnX, y);
    doc.setFontSize(fieldsFontSize);
    doc.setFont(fontStyle, "normal");

    y += distanceInLines;
    doc.text("Hearing __________", fieldsFirstColumnX, y);
    doc.text("Audiologist __________", fieldsSecondColumnX, y);

    y += distanceInLines;
    doc.setLineDashPattern([1, 1], 0);
    doc.line(lineStartX, y, lineEndX, y);
    doc.setLineDashPattern([], 0);

    return y;
}

function drawPhaSection(doc, fontStyle, headingFontSize, fieldsFontSize, headingFirstColumnX, fieldsFirstColumnX, fieldsSecondColumnX, lineStartX, lineEndX, y, distanceInLines) {
    y += distanceInLines;
    doc.setFontSize(headingFontSize);
    doc.setFont(fontStyle, "bold");
    doc.text("PHA", headingFirstColumnX, y);
    doc.setFontSize(fieldsFontSize);
    doc.setFont(fontStyle, "normal");

    y += distanceInLines;
    doc.text("Record Review __________", fieldsFirstColumnX, y);
    doc.text("Provider __________", fieldsSecondColumnX, y);

    y += distanceInLines;
    doc.setLineDashPattern([1, 1], 0);
    doc.line(lineStartX, y, lineEndX, y);
    doc.setLineDashPattern([], 0);

    return y;
}

////////////////////////////////////////////////////////////Routing Sheet Methods End////////////////////////////////////////////////////////////////////


function showYesNoModal(message = "Are you sure?") {
    return new Promise((resolve) => {
        const modalElement = document.getElementById("yesNoModal");
        const modal = new bootstrap.Modal(modalElement);

        document.getElementById("yesNoModalMessage").textContent = message;

        // Handle YES
        const yesBtn = document.getElementById("yesBtn");
        const newYesBtn = yesBtn.cloneNode(true);
        newYesBtn.id = "yesBtn"; // restore ID
        yesBtn.parentNode.replaceChild(newYesBtn, yesBtn);

        newYesBtn.addEventListener("click", () => {
            resolve(true);
            modal.hide();
        });

        // Handle NO
        const noBtn = document.getElementById("noBtn");
        const newNoBtn = noBtn.cloneNode(true);
        newNoBtn.id = "noBtn"; // restore ID
        noBtn.parentNode.replaceChild(newNoBtn, noBtn);

        newNoBtn.addEventListener("click", () => {
            resolve(false);
            modal.hide();
        });

        modal.show();
    });
}

const columnMappingsForInsertionAndFetching = [
    { key: "SmId", label: "SM ID", type: "int" },
    { key: "FullName", label: "FULL NAME", type: "string" },
    { key: "FullSsn", label: "FULL SSN", type: "string" },
    { key: "Last4", label: "LAST 4", type: "string" },
    { key: "DodId", label: "DOD ID", type: "string" },
    { key: "Rank", label: "RANK", type: "string" },
    { key: "Age", label: "AGE", type: "int" },
    { key: "Sex", label: "SEX", type: "string" },
    { key: "Mos", label: "MOS", type: "string" },
    { key: "Agr", label: "AGR", type: "string" },
    { key: "Uic", label: "UIC", type: "string" },
    { key: "Mrc", label: "MRC", type: "string" },
    { key: "Dob", label: "DOB", type: "string" },
    { key: "Over40", label: "OVER 40", type: "string" },
    { key: "DentalDue", label: "Dental Due", type: "string" },
    { key: "DentalExam", label: "Dental Exam", type: "string" },
    { key: "DentalNeeded", label: "Dental Needed", type: "string" },
    { key: "PanoNeeded", label: "PANO Needed", type: "string" },
    { key: "BwxNeeded", label: "BWX Needed", type: "string" },
    { key: "Drc", label: "DRC", type: "string" },
    { key: "PhaDate", label: "PHA Date", type: "string" },
    { key: "PhaDue", label: "PHA Due", type: "string" },
    { key: "Pha", label: "PHA Needed", type: "string" },
    { key: "Pulhes", label: "PULHES", type: "string" },
    { key: "VisionDate", label: "Vision Date", type: "string" },
    { key: "Vision", label: "VISION Needed", type: "string" },
    { key: "NearVision", label: "NEAR VISION Needed", type: "string" },
    { key: "Vrc", label: "VRC", type: "string" },
    { key: "Vision2pg", label: "Vision 2PG", type: "string" },
    { key: "Vision1mi", label: "Vision Mask Insert", type: "string" },
    { key: "HearingDate", label: "Hearing Date", type: "string" },
    { key: "Hearing", label: "HEARING Needed", type: "string" },
    { key: "Hrc", label: "HRC", type: "string" },
    { key: "HearingProfile", label: "Hearing Profile", type: "string" },
    { key: "Quest", label: "Lab Requisition", type: "string" },
    { key: "LabNeeded", label: "Lab Needed", type: "string" },
    { key: "Abo", label: "ABO", type: "string" },
    { key: "AboNeeded", label: "ABO Needed", type: "string" },
    { key: "Dna", label: "DNA", type: "string" },
    { key: "SickleDate", label: "Sickle Date", type: "string" },
    { key: "Sickle", label: "SICKLE", type: "string" },
    { key: "G6pd", label: "G6PD", type: "string" },
    { key: "G6pdDate", label: "G6PD Date", type: "string" },
    { key: "G6pdStatus", label: "G6PD Status", type: "string" },
    { key: "HivNextTestDate", label: "HIV NEXT TEST DATE", type: "string" },
    { key: "Hiv", label: "HIV", type: "string" },
    { key: "LipidNeeded", label: "Lipid Needed", type: "string" },
    { key: "LipidPanel", label: "LIPID PANEL", type: "string" },
    { key: "CholesterolHdlCholesterol", label: "Cholesterol / HDL Cholesterol", type: "string" },
    { key: "Framingham", label: "Framingham", type: "string" },
    { key: "Ekg", label: "EKG (Date)", type: "string" },
    { key: "EkgNeeded", label: "EKG NEEDED", type: "string" },
    { key: "PregnancyTestNeeded", label: "Pregnancy Test Needed", type: "string" },
    { key: "Imm", label: "IMM Needed", type: "string" },
    { key: "HepB", label: "Hep B Needed", type: "string" },
    { key: "HepA", label: "Hep A Needed", type: "string" },
    { key: "Flu", label: "FLU Needed", type: "string" },
    { key: "TetTdp", label: "Tet/TDP Needed", type: "string" },
    { key: "Mmr", label: "MMR Needed", type: "string" },
    { key: "Varicella", label: "Varicella Needed", type: "string" },
    { key: "TaskForce", label: "TaskForce", type: "string" },
    { key: "Notes", label: "Notes", type: "string" },
    { key: "Over44", label: "Over 44", type: "string" },
    { key: "EventDate", label: "EventDate", type: "string" },
    { key: "EventEndDate", label: "Event End Date", type: "string" },
    { key: "EventId", label: "EventID", type: "string" },
    { key: "VisionWin", label: "Vision Win", type: "int" },
    { key: "DentalWin", label: "Dental Win", type: "int" },
    { key: "PhaWin", label: "PHA Win", type: "int" },
    { key: "HivWin", label: "HIV Win", type: "int" },
    { key: "HearingWin", label: "Hearing WIN", type: "int" },
    { key: "Barcode", label: "Barcode", type: "string" },
    { key: "VitalsStatus", label: "Vitals Status", type: "string" },
    { key: "HepBNeededStatus", label: "Hep B Needed Status", type: "string" },
    { key: "HepBReason", label: "Hep B Reason", type: "string" },
    { key: "FluNeededStatus", label: "FLU Needed Status", type: "string" },
    { key: "FluReason", label: "FLU Reason", type: "string" },
    { key: "MmrStatus", label: "MMR Status", type: "string" },
    { key: "MmrReason", label: "MMR Reason", type: "string" },
    { key: "HepANeededStatus", label: "Hep A Needed Status", type: "string" },
    { key: "HepAReason", label: "Hep A Reason", type: "string" },
    { key: "TetTdpNeededStatus", label: "Tet/TDP Needed Status", type: "string" },
    { key: "TetTdpReason", label: "Tet/TDP Reason", type: "string" },
    { key: "VaricellaNeededStatus", label: "Varicella Needed Status", type: "string" },
    { key: "VaricellaReason", label: "Varicella Reason", type: "string" },
    { key: "SickleStatus", label: "SICKLE Status", type: "string" },
    { key: "SickleReason", label: "Sickle Reason", type: "string" },
    { key: "HivStatus", label: "HIV Status", type: "string" },
    { key: "HivBarcode", label: "HIV Barcode", type: "string" },
    { key: "HivReason", label: "HIV Reason", type: "string" },
    { key: "AboStatus", label: "ABO Status", type: "string" },
    { key: "AboReason", label: "ABO Reason", type: "string" },
    { key: "DnaStatus", label: "DNA Status", type: "string" },
    { key: "DnaReason", label: "DNA Reason", type: "string" },
    { key: "G6pdCheckoutStatus", label: "G6PD Checkout Status", type: "string" },
    { key: "G6pdReason", label: "G6PD Reason", type: "string" },
    { key: "LipidStatus", label: "Lipid Status", type: "string" },
    { key: "LipidReason", label: "Lipid Reason", type: "string" },
    { key: "EkgNeededStatus", label: "EKG Needed Status", type: "string" },
    { key: "EkgNeededReason", label: "EKG Needed Reason", type: "string" },
    { key: "PhaStatus", label: "PHA Status", type: "string" },
    { key: "PhaFollowUp", label: "PHA Follow Up", type: "string" },
    { key: "HearingStatus", label: "Hearing Status", type: "string" },
    { key: "AudiologistServiceCompleted", label: "Audiologist Service Completed", type: "string" },
    { key: "VisionStatus", label: "Vision Status", type: "string" },
    { key: "OptometristServiceCompleted", label: "Optometrist Service Completed", type: "string" },
    { key: "DentalXrayStatus", label: "Dental X-Ray Status", type: "string" },
    { key: "PanoramicXray", label: "Panoramic X-Ray", type: "string" },
    { key: "DentalExamStatus", label: "Dental Exam Status", type: "string" },
    { key: "ClassDentalExam", label: "Class (Dental Exam)", type: "string" },
    { key: "DentalTreatment", label: "Dental Treatment", type: "string" },
    { key: "FinalDentalClass", label: "Final Dental Class", type: "string" },
    { key: "DentalTreatmentReason", label: "Dental Treatment Reason", type: "string" },
    { key: "CheckIn", label: "Checked In", type: "string" },
    { key: "CheckOut", label: "Checked Out", type: "string" },
    { key: "CheckInBy", label: "Checked In By", type: "string" },
    { key: "CheckOutBy", label: "Checked Out By", type: "string" },
    { key: "CheckInTime", label: "Checked In Time", type: "date" },
    { key: "CheckOutTime", label: "Checked Out Time", type: "date" },
    { key: "WalkInServiceMember", label: "Walk-In Service Member", type: "string" }
];


function submitData() {
    $('#loader').removeClass('d-none');
    let tableRows = [];

    $('#previewTable tbody tr').each(function () {
        let $cells = $(this).find('td');
        let row = {};

        columnMappingsForInsertionAndFetching.forEach(mapping => {
            let value = getCellValue($cells, mapping.label);
            switch (mapping.type) {
                case "int":
                    row[mapping.key] = parseInt(value) || 0;
                    break;
                case "float":
                    row[mapping.key] = parseFloat(value) || 0.0;
                    break;
                case "date":
                    // Handle empty string or invalid date
                    if (!value) {
                        row[mapping.key] = null;
                    } else {
                        let parsedDate = new Date(value);

                        // If invalid date, fallback to null
                        if (isNaN(parsedDate.getTime())) {
                            row[mapping.key] = null;
                        } else {
                            // Format as ISO string (PostgreSQL-compatible)
                            row[mapping.key] = parsedDate.toISOString(); // e.g., "2025-07-14T10:30:00.000Z"
                        }
                    }
                    break;
                default: // string or unknown
                    row[mapping.key] = value;
                    break;
            }
        });

        row["isDeleted"] = false; // always false when inserting
        tableRows.push(row);
    });

    let eventId = null;

    if (window.isCheckInOutPage === true) {
        const firstRow = $('#previewTable tbody tr:first');
        const $cells = firstRow.find('td');
        eventId = getCellValue($cells, "EventID");

    }
    else {
        eventId = document.getElementById('eventId').value;
    }

    $.ajax({
        url: '/Home/CheckForExistingDataAgainstEventId',
        type: 'POST',
        data: JSON.stringify(eventId),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                submitDataToDatabase(tableRows, eventId);
            } else {
                Swal.fire({
                    title: 'Data Exists!',
                    text: 'Data already exists for this Event ID. Do you want to overwrite it?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes, overwrite',
                    cancelButtonText: 'No, cancel'
                }).then((result) => {
                    if (result.isConfirmed) {
                        submitDataToDatabase(tableRows, eventId);
                    } else {
                        Swal.fire('Cancelled', 'No changes were made.', 'info');
                    }
                });
            }
        },
        error: function (xhr, status, error) {
            alert("Error: " + error);
        },
        complete: function () {
            $('#loader').addClass('d-none');
        }
    });
}

function submitDataToDatabase(tableRows, eventId) {
    const dataToSend = JSON.stringify({
        Entities: tableRows,
        EventId: eventId
    });
    console.log(dataToSend);
    $.ajax({
        url: '/Home/SubmitDataInDatabase',
        type: 'POST',
        data: dataToSend,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            Swal.fire({
                title: response.success ? "Success!" : "Error!",
                text: response.message,
                icon: response.success ? "success" : "error",
                confirmButtonText: "OK"
            }).then(() => {
                if (response.success) {
                    clearPreview();
                }
            });
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: "Error!",
                text: "Something went wrong: " + error,
                icon: "error",
                confirmButtonText: "OK"
            });
        },
        complete: function () {
            $('#loader').addClass('d-none');
        }
    });
}


function clearPreview() {
    if ($.fn.DataTable.isDataTable('#previewTable')) {
        $('#previewTable').DataTable().clear().destroy(); // Clear data and destroy the instance
    }

    const tableHead = document.querySelector('#previewTable thead');
    const tableBody = document.querySelector('#previewTable tbody');

    tableHead.innerHTML = '';
    tableBody.innerHTML = '';

    if (window.isCheckInOutPage === false) {
        document.getElementById('taskforceInfoContainer').innerHTML = '';
        document.getElementById('fileValidationErrorContainer').innerHTML = '';
        document.getElementById('generateExcelButton').classList.add('d-none');
    }
    else {
        $('#eventSummaryTable').addClass('d-none');
        $('.card.p-3.mb-4').addClass('d-none');
        $('#excelFile').val('');
    }

    document.getElementById('submitDataButton').classList.add('d-none');
    /*document.getElementById('addRowButton').classList.add('d-none');*/
    smIdCounter = 0;
    uploadCounter = 0;
}



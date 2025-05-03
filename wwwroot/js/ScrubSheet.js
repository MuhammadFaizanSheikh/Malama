const keys = [
    "SM ID", "FULL NAME", "NAME", "FULL SSN", "LAST 4", "DOD ID", "RANK", "AGE", "SEX", "MOS",
    "AGR", "UIC", "MRC", "DOB", "OVER 40", "Dental Due", "Dental Exam", "Dental Needed",
    "PANO Needed", "BWX Needed", "DRC", "PHA Date", "PHA Due", "PHA Needed", "PULHES", "Vision Date",
    "VISION Needed", "NEAR VISION Needed", "VRC", "Vision 2PG", "Vision Mask Insert", "Hearing Date", "HEARING Needed",
    "HRC", "Hearing Profile", "Lab Requisition", "Lab Needed", "ABO", "ABO Needed", "DNA", "Sickle Date",
    "SICKLE", "G6PD", "G6PD Date", "G6PD Status", "HIV NEXT TEST DATE", "HIV", "Lipid Needed",
    "LIPID PANEL", "Cholesterol / HDL Cholesterol", "Framingham", "EKG", "EKG NEEDED", "hcg",
    "IMM Needed", "Hep B Needed", "Hep A Needed", "FLU Needed", "Tet/TDP Needed", "MMR Needed", "Varicella Needed", "TaskForce", "Notes", "Over 44",
    "EventDate", "Event End Date", "EventID", "Vision_Win", "Dental_Win", "PHA_Win", "HIV_Win", "Hearing_WIN"
];

const tableToKeysIndexMap = [
    keys.indexOf("FULL NAME"),      // FULL NAME
    keys.indexOf("AGE"),            // AGE
    keys.indexOf("UIC"),            // UIC
    keys.indexOf("FULL SSN"),         // LAST 4
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
    keys.indexOf("Vision_Win"),     // Vision_Win
    keys.indexOf("PHA_Win"),        // PHA_Win
    keys.indexOf("Hearing_WIN"),    // Hearing_WIN
    keys.indexOf("Dental_Win"),     // Dental_Win
    keys.indexOf("HIV_Win"),        // HIV_Win
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
    keys.indexOf("EKG"),            // EKG
    keys.indexOf("LIPID PANEL"),    // LIPID PANEL
    keys.indexOf("Framingham"),     // Framingham
    keys.indexOf("EKG NEEDED"),     // EKG NEEDED
    keys.indexOf("hcg"),            // hcg
    // Immunization information
    keys.indexOf("IMM Needed"),            // IMM
    keys.indexOf("Hep B Needed"),          // Hep B
    keys.indexOf("FLU Needed"),            // FLU
    keys.indexOf("MMR Needed"),            // MMR
    keys.indexOf("Hep A Needed"),          // Hep A
    keys.indexOf("Tet/TDP Needed"),        // Tet/TDP
    keys.indexOf("Varicella Needed")       // Varicella
];





const categories = {
    "Personal Information": [
        "FULL NAME", "AGE", "UIC", "FULL SSN", "SEX", "MRC", "DOD ID", "MOS", "DOB", "RANK", "AGR", "OVER 40"
    ],
    "Taskforce Information": [
        "TaskForce", "EventID", "Over 44", "EventDate", "Event End Date", "Vision_Win", "PHA_Win",
        "Hearing_WIN", "Dental_Win", "HIV_Win", "Notes"
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
        , "EKG", "LIPID PANEL", "Framingham", "EKG NEEDED", "hcg"
    ],
    "Immunization Information": [
        "IMM Needed", "Hep B Needed", "FLU Needed", "MMR Needed", "Hep A Needed", "Tet/TDP Needed", "Varicella Needed"
    ]
};



let currentRow;
let isAddingNewRow = false;

function editRow(button) {
    alert('asdasd')
    currentRow = $(button).closest('tr');  // Get the row clicked for editing
    const rowData = {};

    // Correct mapping: use tableToKeysIndexMap to map the table column index to the keys array
    tableToKeysIndexMap.forEach((tableColIndex, keysIndex) => {
        const key = keys[tableColIndex];  // Get the correct key from the keys array
        rowData[key] = currentRow.find('td').eq(tableColIndex).text().trim() || '';  // Get the value from the correct table column
    });

    populateModal(rowData);  // Pass the mapped data to populateModal
    $('#editModal').modal('show');  // Show the modal
}

const calendarIndexes = [13];
const dropdownIndexes = [8, 10, 12, 19, 20, 23, 27, 28, 29, 30, 37, 38, 39, 41, 42, 46, 47, 53, 55, 56, 57, 58, 59, 60];
// const readOnlyIndexes = [7, 14, 15, 16, 17, 18, 19, 22, 21, 43, 23, 25, 26, 31, 32, 35, 36, 40, 45, 48, 49, 51, 52, 61, 63, 64, 65, 66, 67, 68, 69, 70, 71];
const readOnlyIndexes = [6, 7, 8, 9, 10, 11, 12, 14, 61, 66, 63, 64, 65, 70, 67, 69, 71, 68, 15, 17, 16, 21, 24, 22, 25, 26, 31, 32, 33, 34, 40, 45, 35, 36, 43, 44, 52, 54];
const multilineTextbox = [62];

// Define specific dropdown options for certain fields
const dropdownOptionsMapping = {
    "SEX": [
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
    "hcg": [
        { value: "", label: "" },
        { value: "Needed", label: "Needed" },
        { value: "N/A", label: "N/A" }
    ]
};

const modalContent = $('#modalBodyContent');

function populateModal(data) {
    modalContent.empty(); // Clear previous content

    const fieldsPerRow = 5; // Set to 5 fields per row now

    for (const [categoryName, categoryKeys] of Object.entries(categories)) {
        modalContent.append(`<h5 class="category-header">${categoryName}</h5><hr/>`);

        let rowHtml = '<div class="row">';
        let inputCount = 0;
        categoryKeys.forEach((key, index) => {
            const value = data[key] || '';
            let inputHtml = '';
            let keyIndex = keys.indexOf(key);
            let readOnly = readOnlyIndexes.includes(keys.indexOf(key)) ? 'readonly' : '';
            let textColor = 'style="color: black;"'; // Set text color to black

            if (keyIndex === 18 || keyIndex === 48 || keyIndex === 51) {
                if (value.trim().toLowerCase() === "n/a") {
                    inputHtml = `
                                                    <div class="form-group col-lg-2">
                                                                <label>${key}</label>
                                                                            <input type="text" class="form-control" name="${key}" value="${value}" readonly ${textColor} />
                                                            </div>`;

                } else {
                    let dateValue = isValidDate(value) ? formatDateToYYYYMMDD(value) : '';
                    inputHtml = `
                                                            <div class="form-group col-lg-2">
                                                                <label>${key}</label>
                                                                <input type="date" class="form-control" name="${key}" value="${dateValue}" placeholder="mm/dd/yyyy" ${readOnly} ${textColor} />
                                                            </div>`;
                }
            }
            else if (keyIndex === 49) {
                if (value.trim().toLowerCase() === "n/a") {
                    inputHtml = `<div class="form-group col-lg-2">
                                                                            <label>${key}</label>
                                                                                        <input type="text" class="form-control" name="${key}" value="${value}" readonly ${textColor} />
                                                                        </div>`;

                } else {
                    inputHtml = `<div class="form-group col-lg-2">
                                                                                        <label>${key}</label>
                                                                                                    <input type="text" class="form-control" name="${key}" value="${value.toLowerCase() === 'needed' ? '' : value}" ${readOnly} ${textColor} />
                                                                                    </div>`;
                }
            }
            else if (keyIndex === 50) {
                if (value.trim().toLowerCase() === "n/a") {
                    inputHtml = `<div class="form-group col-lg-2">
                                                                                        <label>${key}</label>
                                                                                                                <input type="text" class="form-control decimal-input" name="${key}" value="${value}" readonly ${textColor} />
                                                                                    </div>`;

                } else {
                    inputHtml = `<div class="form-group col-lg-2">
                                                                                        <label>${key}</label>
                                                                                            <input type="number" class="form-control decimal-input" name="${key}"
                                                                                            value="${value.toLowerCase() === 'needed' ? '' : value}"
                                                                                            step="0.1" inputmode="decimal" ${readOnly} ${textColor} />

                                                                                    </div>`;
                }
            }
            else if (multilineTextbox.includes(keys.indexOf(key))) {
                inputHtml = `
                            <div class="form-group col-lg-12">
                                <label>${key}</label>
                                <textarea class="form-control" name="${key}" rows="4" ${readOnly} ${textColor}>${value}</textarea>
                            </div>
                        `;
            }
            else if (calendarIndexes.includes(keys.indexOf(key))) {
                let dateValue = isValidDate(value) ? formatDateToYYYYMMDD(value) : '';
                inputHtml = `
                                    <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <input type="date" class="form-control" name="${key}" value="${dateValue}" placeholder="mm/dd/yyyy" ${readOnly} ${textColor} />
                                    </div>
                                `;
            }
            // Dropdown field
            else if (dropdownIndexes.includes(keys.indexOf(key))) {
                const dropdownOptions = dropdownOptionsMapping[key] || [
                    { value: "NEEDED", label: "NEEDED" },
                    { value: "N/A", label: "N/A" }
                ];

                // Build dropdown options dynamically
                let optionsHtml = dropdownOptions.map(option =>
                    `<option value="${option.value}" ${value === option.value ? 'selected' : ''}>${option.label}</option>`
                ).join('');

                let disabled = readOnlyIndexes.includes(keys.indexOf(key)) ? 'disabled' : '';

                inputHtml = `
                                    <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <select class="form-control" name="${key}" ${disabled} ${textColor}>

                                            ${optionsHtml}
                                        </select>
                                    </div>
                                `;
            }
            // Default text field
            else {
                inputHtml = `
                                    <div class="form-group col-lg-2">
                                        <label>${key}</label>
                                        <input type="text" class="form-control" name="${key}" value="${value}" ${readOnly} ${textColor} />
                                    </div>
                                `;
            }

            rowHtml += inputHtml;
            inputCount++;

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
            inputField.on('change', checkQuestField);
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
}

//****************************************************This can be needed in future*************************************************/
function checkQuestField() {
    const aboNeededValue = modalContent.find('select[name="ABO Needed"]').val();
    const g6pdValue = modalContent.find('select[name="G6PD"]').val();
    const sickleValue = modalContent.find('select[name="SICKLE"]').val();
    const lipidNeededValue = modalContent.find('select[name="Lipid Needed"]').val();

    const questField = modalContent.find('input[name="Lab Requisition"]');

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

    const labNeededField = modalContent.find('input[name="Lab Needed"]');

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



function handleColumnsRelatedToDob(dob) {
    if (!dob) return; // Ensure DOB is provided
    let dobDate = new Date(dob);
    let today = new Date(document.getElementById("calendarInput").value);
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

    const nearVisionField = document.querySelector('select[name="NEAR VISION Needed"]');
    if (nearVisionField) {
        nearVisionField.value = ageWithGrace >= 45 ? "NEEDED" : "N/A";
    }

    const lipidNeededField = document.querySelector('select[name="Lipid Needed"]');
    const lipidPanelField = document.querySelector('input[name="LIPID PANEL"]');
    const cholesterolField = document.querySelector('input[name="Cholesterol / HDL Cholesterol"]');
    const ekgField = document.querySelector('input[name="EKG"]');
    const ekgNeededField = document.querySelector('input[name="EKG NEEDED"]');
    const framinghamField = document.querySelector('input[name="Framingham"]');

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
    "FULL NAME": { type: "alphanumeric", allowSpecialCharacters: true, uppercase: true }, // Allow special characters in FULL NAME
    "FULL SSN": { type: "numeric", format: "xxx-xx-xxxx", maxLength: 11 }, // SSN field
    "DOD ID": { type: "numeric", maxLength: 10 },
    //"RANK": { type: "alphanumeric", uppercase: true, maxLength: 3 },
    //"MOS": { type: "alphanumeric", uppercase: true },
    //"UIC": { type: "alphanumeric", uppercase: true },
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

        if (inputName === "FULL NAME" && rules.allowSpecialCharacters) {
            $(field).val(value);
        }

        if (rules.type === "alphanumeric" && inputName !== "FULL NAME") {
            if (!/^[A-Z0-9]*$/.test(value)) {
                $(field).val(value.replace(/[^A-Z0-9]/g, '')); // Remove non-alphanumeric characters
            }

            // Restrict to max length for alphanumeric fields
            if (rules.maxLength && value.length > rules.maxLength) {
                $(field).val(value.slice(0, rules.maxLength)); // Trim to max length
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
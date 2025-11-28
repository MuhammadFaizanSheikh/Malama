const qualificationsAttributesAndTypes = {
    "Nurse Practitioner": {
        attributes: ["Behavioral Health", "EKG Reading"],
        types: ["APRN", "ARNP", "NP"]
    },
    "Physician Assistant": {
        attributes: ["Behavioral Health", "EKG Reading"],
        types: ["PA-C", "PA"]
    },
    "Registered Nurse": {
        attributes: ["Phlebotomy", "EKG", "Imms", "Cold Chain Cert"],
        types: ["RN"]
    },
    "Licensed Vocational Nurse": {
        attributes: ["Phlebotomy", "EKG", "Imms", "Cold Chain Cert"],
        types: ["LVN"]
    },
    "Licensed Practical Nurse": {
        attributes: ["Phlebotomy", "EKG", "Imms", "Cold Chain Cert"],
        types: ["LPN"]
    },
    "Medical Assistant": {
        attributes: ["Phlebotomy", "EKG", "Imms", "Cold Chain Cert"],
        types: ["CMA", "RMA", "CCMA", "CMAA"]
    },
    "Audiologist": {
        attributes: [],
        types: ["Au.D", "CCC-A", "F-AAA", "ABA"]
    },
    "Audio tech": {
        attributes: [],
        types: ["CAOHC"]
    },
    "Dentist": {
        attributes: ["Exam", "Treatment", "Oral Surgery", "Endo", "General"],
        types: ["DDS", "DMD"]
    },
    "Dental Assistant": {
        attributes: ["Exam Assist", "Treatment Assist", "X-ray"],
        types: ["CDA", "RDA", "RHS"]
    },
    "Registered Dental Hygienist": {
        attributes: [],
        types: ["RDH"]
    },
    "Medical Doctor": {
        attributes: ["Behavioral Health", "EKG Reading"],
        types: ["MD", "DO"]   // you didn't provide MD types — keep empty
    },
    "Optometrist": {
        attributes: [],
        types: ["O.D."]
    },
    "Phlebotomist": {
        attributes: ["Phlebotomy", "EKG", "Imms", "Cold Chain Cert"],
        types: ["LPT", "CPT1", "CPT2"]
    },
    "Admin (CAC'd)": {
        attributes: ["CAC", "Data Entry", "DOEHRS Entry", "Record Review"],
        types: []  // empty from your list
    },
    "Admin": {
        attributes: [],
        types: []  // empty from your list
    }
};
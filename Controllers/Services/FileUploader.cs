using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.Repositories.Services;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.RegularExpressions;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class FileUploader : IFileUploader
    {
        List<string> dateHeaderNames = new List<string> { "DOB", "Date of Next Exam", "PHA Date", "Next PHA Date", "Date of Vision Screen", "Audiogram Date", "Sickle Cell Date", "G6PD Date", "Next Test Date" };
        private string DateFormat = "MM/dd/yyyy";

        private readonly IFileUploaderRepository<FileDataDto> fileUploaderRepository;

        public FileUploader(IFileUploaderRepository<FileDataDto> fileUploaderRepository)
        {
            this.fileUploaderRepository = fileUploaderRepository;
        }

        public List<List<Dictionary<string, object>>> UploadAndPreview(List<IFormFile> files, IFormFile G6PDFile, DateTime parsedEventDate, DateTime? parsedLastEventDate, string eventId, int lastDentalExam, int vision, int dental, int pha, int hiv, int hearing)
        {
            var processingSequence = GetProcessingSequence();

            var validationErrors = CheckFieldsBeforeUploading(files, processingSequence);

            DateTime visionDate = parsedEventDate.AddDays(vision);
            DateTime dentalDate = parsedEventDate.AddDays(dental);
            DateTime phaDate = parsedEventDate.AddDays(pha);
            DateTime hivDate = parsedEventDate.AddDays(hiv);
            DateTime hearingDate = parsedEventDate.AddDays(hearing);
            var fileConfigurations = GetFileConfiguration();
            var fileConfigG6PD = fileConfigurations["G6PDReport.xlsx"];
            var G6PDDataTable = GetDataFromFile(G6PDFile, fileConfigG6PD.Headers, fileConfigG6PD.SelectedColumns, fileConfigG6PD.stoppingKeyword, isG6PD: true, out string taskForceValue, out long totalRecordValueInG6pdFile);
            CheckG6PDTotalAssignedCountWithTotalRow(G6PDDataTable, totalRecordValueInG6pdFile);
            ProcessExcelFiles(files, G6PDDataTable, processingSequence, fileConfigurations, parsedLastEventDate, visionDate, dentalDate, phaDate, hivDate, hearingDate, parsedEventDate, lastDentalExam, vision, dental, pha, hiv, hearing, eventId);
            MergeSomeMoreColumns(G6PDDataTable, taskForceValue);
            var headerMapping = GetHeaderMapping();
            string[] predefinedColumnOrder = GetPredefinedColumnHeader();
            var preparedDataTable = RenameReorderAndProcess(G6PDDataTable, headerMapping, predefinedColumnOrder);
            var mergedData = DataTableToJson(preparedDataTable);

            return new List<List<Dictionary<string, object>>>
            {
                validationErrors,
                mergedData
            };
        }

        public ResponseDto AddRecordsBulk(IEnumerable<FileDataDto> data, string eventId)
        {
            bool isRecordUpdate = false;

            var dataAgainstEventId = fileUploaderRepository.FindByEventId(eventId);

            if (dataAgainstEventId != null && dataAgainstEventId.Any())
            {
                isRecordUpdate = true;

                foreach (var item in dataAgainstEventId)
                {
                    item.isDeleted = true;
                    item.UpdatedBy = 1;
                    item.UpdatedOn = DateTime.Now;
                }

                fileUploaderRepository.UpdateRange(dataAgainstEventId);
                fileUploaderRepository.Save();
            }


            var Records = data.Select(dto => new FileDataDto
            {
                SmId = dto.SmId,
                FullName = dto.FullName,
                Name = dto.Name,
                FullSsn = dto.FullSsn,
                Last4 = dto.Last4,
                DodId = dto.DodId,
                Rank = dto.Rank,
                Age = dto.Age,
                Sex = dto.Sex,
                Mos = dto.Mos,
                Agr = dto.Agr,
                Uic = dto.Uic,
                Mrc = dto.Mrc,
                Dob = dto.Dob,
                Over40 = dto.Over40,
                DentalDue = dto.DentalDue,
                DentalExam = dto.DentalExam,
                DentalNeeded = dto.DentalNeeded,
                PanoNeeded = dto.PanoNeeded,
                BwxNeeded = dto.BwxNeeded,
                Drc = dto.Drc,
                PhaDate = dto.PhaDate,
                PhaDue = dto.PhaDue,
                Pha = dto.Pha,
                Pulhes = dto.Pulhes,
                VisionDate = dto.VisionDate,
                Vision = dto.Vision,
                NearVision = dto.NearVision,
                Vrc = dto.Vrc,
                Vision2pg = dto.Vision2pg,
                Vision1mi = dto.Vision1mi,
                HearingDate = dto.HearingDate,
                Hearing = dto.Hearing,
                Hrc = dto.Hrc,
                HearingProfile = dto.HearingProfile,
                Quest = dto.Quest,
                LabNeeded = dto.LabNeeded,
                Abo = dto.Abo,
                AboNeeded = dto.AboNeeded,
                Dna = dto.Dna,
                SickleDate = dto.SickleDate,
                Sickle = dto.Sickle,
                G6pd = dto.G6pd,
                G6pdDate = dto.G6pdDate,
                G6pdStatus = dto.G6pdStatus,
                HivNextTestDate = dto.HivNextTestDate,
                Hiv = dto.Hiv,
                LipidNeeded = dto.LipidNeeded,
                LipidPanel = dto.LipidPanel,
                CholesterolHdlCholesterol = dto.CholesterolHdlCholesterol,
                Framingham = dto.Framingham,
                Ekg = dto.Ekg,
                EkgNeeded = dto.EkgNeeded,
                Hcg = dto.Hcg,
                Imm = dto.Imm,
                HepB = dto.HepB,
                HepA = dto.HepA,
                Flu = dto.Flu,
                TetTdp = dto.TetTdp,
                Mmr = dto.Mmr,
                Varicella = dto.Varicella,
                TaskForce = dto.TaskForce,
                Notes = dto.Notes,
                Over44 = dto.Over44,
                EventDate = dto.EventDate,
                EventEndDate = dto.EventEndDate,
                EventId = dto.EventId,
                VisionWin = dto.VisionWin,
                DentalWin = dto.DentalWin,
                PhaWin = dto.PhaWin,
                HivWin = dto.HivWin,
                HearingWin = dto.HearingWin,
                isDeleted = false,
                AddedBy = 1,
                AddedOn = DateTime.Now
            }).ToList();

            fileUploaderRepository.AddRange(Records);
            fileUploaderRepository.Save();

            return new ResponseDto
            {
                Success = true,
                Message = Messages.DataInsertSuccesfully
            };
        }

        public ResponseDto CheckForExistingDataAgainstEventId(string eventId)
        {
            var dataAgainstEventId = fileUploaderRepository.FindByEventId(eventId);

            if (dataAgainstEventId.Any())
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = ""
                };
            }

            return new ResponseDto
            {
                Success = true,
                Message = ""
            };
        }

        public List<Dictionary<string, object>> CheckFieldsBeforeUploading(List<IFormFile> files, List<string>? processingSequence)
        {
            var validationErrors = new List<Dictionary<string, object>>();
            var fileConfigurations = GetFileConfigurationForChecking();

            foreach (var fileName in processingSequence)
            {
                IFormFile file = null; // Assume file is fetched correctly based on your logic
                if (fileName.StartsWith("Taskforce_Roster", StringComparison.OrdinalIgnoreCase))
                {
                    file = files.FirstOrDefault(f => Regex.IsMatch(f.FileName, @"^Taskforce_Roster.*\.xlsx$", RegexOptions.IgnoreCase));
                }
                else
                {
                    file = files.FirstOrDefault(f => f.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                }
                if (file == null) continue;

                var fileConfig = fileConfigurations[fileName];
                DataTable fileData = GetDataFromFile(file, fileConfig.Headers, fileConfig.SelectedColumns, fileConfig.stoppingKeyword, isG6PD: false, out string taskForceValue, out long totalRecordValueInG6pdFile);

                // Perform validations based on the report type
                ValidateMissingFields(fileName, fileData, fileConfig.SelectedColumns, validationErrors);

            }

            return validationErrors;
        }

        private void ValidateMissingFields(string fileName, DataTable fileData, string[] selectedColumns, List<Dictionary<string, object>> validationErrors)
        {
            var dodIdTracker = new HashSet<string>(); // To track DOD ID values for duplicates

            for (int rowIndex = 0; rowIndex < fileData.Rows.Count; rowIndex++)
            {
                var row = fileData.Rows[rowIndex];

                // Check for missing fields
                foreach (var columnName in selectedColumns)
                {
                    if (row[columnName] == null || string.IsNullOrWhiteSpace(row[columnName].ToString()))
                    {
                        validationErrors.Add(new Dictionary<string, object>
                {
                    { "FileName", fileName },
                    { "Row", rowIndex + 1 }, // 1-based index
                    { "Column", columnName },
                    { "Error", "Missing" }
                });
                    }
                }

                // Check for duplicate DOD ID in G6PDReport.xlsx
                if (fileName.Equals("G6PDReport.xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    var dodIdValue = row["DOD ID"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(dodIdValue))
                    {
                        if (dodIdTracker.Contains(dodIdValue))
                        {
                            validationErrors.Add(new Dictionary<string, object>
                    {
                        { "FileName", fileName },
                        { "Row", rowIndex + 1 }, // 1-based index
                        { "Column", "DOD ID" },
                        { "Error", "Duplicate" }
                    });
                        }
                        else
                        {
                            dodIdTracker.Add(dodIdValue);
                        }
                    }
                }
            }
        }


        private void CheckG6PDTotalAssignedCountWithTotalRow(DataTable G6PDDataTable, long totalRecordValueInG6pdFile)
        {
            if (G6PDDataTable.Rows.Count != totalRecordValueInG6pdFile)
            {
                throw new Exception("Total assigned value in G6PD file rows count mismatched!");
            }
        }

        private DataTable RenameReorderAndProcess(DataTable dataTable, Dictionary<string, string> headerMapping, string[] outputHeaders)
        {
            // Create a new DataTable to hold the renamed, reordered columns and processed values
            DataTable reorderedTable = new DataTable();

            // Add columns to the new DataTable based on the outputHeaders array
            foreach (var newHeader in outputHeaders)
            {
                reorderedTable.Columns.Add(newHeader);
            }

            // Loop through each row in the original dataTable
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new row for the reordered table
                DataRow newRow = reorderedTable.NewRow();

                // Copy data from the original row to the new row based on the header mapping
                foreach (var column in headerMapping)
                {
                    if (dataTable.Columns.Contains(column.Key))
                    {
                        var originalValue = row[column.Key];

                        // Check if the value is null or DBNull and replace with an empty string if necessary
                        if (originalValue == null || originalValue is DBNull)
                        {
                            newRow[column.Value] = string.Empty;
                        }
                        // If the value is a non-string object, convert it to a string
                        else if (originalValue.GetType().IsClass && originalValue.GetType() != typeof(string))
                        {
                            newRow[column.Value] = originalValue.ToString();
                        }
                        // Otherwise, just copy the original value
                        else
                        {
                            newRow[column.Value] = originalValue;
                        }
                    }
                }

                // Add the newly reordered and processed row to the reordered DataTable
                reorderedTable.Rows.Add(newRow);
            }

            return reorderedTable;
        }

        private void SetFileFlags(string fileName, out bool isVisionFile, out bool isDentalFile, out bool isPhaFile, out bool isHivFile, out bool isHearingFile, out bool isTaskForceFile, out bool isBloodFile, out bool isUmrFile, out bool isSickleFile, out bool isImmunizationFile)
        {
            isVisionFile = false;
            isDentalFile = false;
            isPhaFile = false;
            isHivFile = false;
            isHearingFile = false;
            isTaskForceFile = false;
            isBloodFile = false;
            isUmrFile = false;
            isSickleFile = false;
            isImmunizationFile = false;

            switch (fileName)
            {
                case "VisionReadinessReport.xlsx":
                    isVisionFile = true;
                    break;

                case "DentalReadinessReport.xlsx":
                    isDentalFile = true;
                    break;

                case "Pha_Report.xlsx":
                    isPhaFile = true;
                    break;

                case "HIVReadinessSummaryReport.xlsx":
                    isHivFile = true;
                    break;

                case "HearingReadinessReport.xlsx":
                    isHearingFile = true;
                    break;
                case "Taskforce_Roster.xlsx":
                    isTaskForceFile = true;
                    break;
                case "BloodTypeReport.xlsx":
                    isBloodFile = true;
                    break;
                case "UMR.xlsx":
                    isUmrFile = true;
                    break;
                case "SickleCellReport.xlsx":
                    isSickleFile = true;
                    break;
                case "ImmunizationProfileReport.xlsx":
                    isImmunizationFile = true;
                    break;

                default:
                    // Handle cases where the file name does not match any known reports
                    break;
            }
        }
        private void ProcessExcelFiles(List<IFormFile> files, DataTable? G6PDDataTable, List<string>? processingSequence, Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)> fileConfigurations, DateTime? parsedLastEventDate, DateTime visionDate, DateTime dentalDate, DateTime phaDate, DateTime hivDate, DateTime hearingDate, DateTime parsedEventDate, int lastDentalExam, int vision, int dental, int pha, int hiv, int hearing, string eventId)
        {
            foreach (var fileName in processingSequence)
            {
                if (fileName != "G6PDReport.xlsx")
                {
                    IFormFile file = null;

                    if (fileName.StartsWith("Taskforce_Roster", StringComparison.OrdinalIgnoreCase))
                    {
                        file = files.FirstOrDefault(f => Regex.IsMatch(f.FileName, @"^Taskforce_Roster.*\.xlsx$", RegexOptions.IgnoreCase));
                    }
                    else
                    {
                        file = files.FirstOrDefault(f => f.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                    }

                    if (file == null) continue;

                    var fileConfig = fileConfigurations[fileName];
                    DataTable fileData = GetDataFromFile(file, fileConfig.Headers, fileConfig.SelectedColumns, fileConfig.stoppingKeyword, isG6PD: false, out string taskForceValue, out long totalRecordValueInG6pdFile);
                    bool isVisionFile = false; bool isDentalFile = false; bool isPhaFile = false; bool isHivFile = false; bool isHearingFile = false; bool isTaskForceFile = false; bool isBloodFile = false; bool isUmrFile = false; bool isSickleFile = false; bool isImmunizationFile = false;
                    SetFileFlags(fileName, out isVisionFile, out isDentalFile, out isPhaFile, out isHivFile, out isHearingFile, out isTaskForceFile, out isBloodFile, out isUmrFile, out isSickleFile, out isImmunizationFile);
                    MergeDataTables(G6PDDataTable, fileData, fileConfig.SelectedColumns, lastDentalExam, parsedLastEventDate, visionDate, dentalDate, phaDate, hivDate, hearingDate, isVisionFile, isDentalFile, isPhaFile, isHivFile, isHearingFile, isTaskForceFile, isBloodFile, isUmrFile, isSickleFile, isImmunizationFile, parsedEventDate, vision, dental, pha, hiv, hearing, eventId);
                }
            }
        }
        private string[] GetPredefinedColumnHeader()
        {
            return new string[]
                {
                //"SM ID",
                "FULL NAME",
                "NAME",
                "FULL SSN",
                "LAST 4",
                "DOD ID",
                "RANK",
                "AGE",
                "SEX",
                "MOS",
                "AGR",
                "UIC",
                "MRC",
                "DOB",
                "OVER 40",
                "Dental Due",
                "Dental Exam",
                "Dental Needed",
                "PANO Needed",
                "BWX Needed",
                "DRC",
                "PHA Date",
                "PHA Due",
                "PHA",
                "PULHES",
                "Vision Date",
                "VISION",
                "NEAR VISION",
                "VRC",
                "Vision 2PG",
                "Vision 1MI",
                "Hearing Date",
                "HEARING",
                "HRC",
                "Hearing Profile",
                "QUEST",
                "Lab Needed",
                "ABO",
                "ABO Needed",
                "DNA",
                "Sickle Date",
                "SICKLE",
                "G6PD",
                "G6PD Date",
                "G6PD Status",
                "HIV NEXT TEST DATE",
                "HIV",
                "Lipid Needed",
                "LIPID PANEL",
                "Cholesterol / HDL Cholesterol",
                "Framingham",
                "EKG",
                "EKG NEEDED",
                "hcg",
                "IMM",
                "Hep B",
                "Hep A",
                "FLU",
                "Tet/TDP",
                "MMR",
                "Varicella",
                "TaskForce",
                "Notes",
                "Over 44",
                "EventDate",
                "Event End Date",
                "EventID",
                "Vision_Win",
                "Dental_Win",
                "PHA_Win",
                "HIV_Win",
                "Hearing_WIN"
                };
        }

        private Dictionary<string, string>? GetHeaderMapping()
        {
            return new Dictionary<string, string>
            {
                 //{ "SM ID", "SM ID" },
                  { "ONLY_NAME", "NAME" },
                { "Name", "FULL NAME" },
                { "FULL SSN", "FULL SSN" },
                { "SSN", "LAST 4" },
                { "DOD ID", "DOD ID" },
                { "Rank", "RANK" },
                { "Age", "AGE" },
                { "DOB", "DOB" },
                { "OVER 40", "OVER 40" },
                { "Sex", "SEX" },
                { "MOS", "MOS" },
                { "AGR", "AGR" },
                { "UIC", "UIC" },
                { "MRC", "MRC" },
                { "Date of Next Exam", "Dental Due" },
                { "Dental Exam", "Dental Exam" },
                { "Dental Needed", "Dental Needed" },
                { "PANO Needed", "PANO Needed" },
                { "BWX Needed", "BWX Needed" },
                { "DRC", "DRC" },
                { "PHA Date", "PHA Date" },
                { "Next PHA Date", "PHA Due" },
                { "PHA", "PHA" },
                { "PULHES Code", "PULHES" },
                { "Date of Vision Screen", "Vision Date" },
                { "VISION", "VISION" },
                { "NEAR VISION", "NEAR VISION" },
                { "VRC", "VRC" },
                { "2PG", "Vision 2PG" },
                { "1MI", "Vision 1MI" },
                { "Audiogram Date", "Hearing Date" },
                { "HEARING", "HEARING" },
                { "HRC", "HRC" },
                { "Hearing Profile", "Hearing Profile" },
                { "QUEST", "QUEST" },
                { "Lab Needed", "Lab Needed" },

                { "Blood Type", "ABO" },
                { "ABO Needed", "ABO Needed" },
                { "DNA", "DNA" },
                { "Sickle Cell Date", "Sickle Date" },
                { "SICKLE", "SICKLE" },
                { "G6PD", "G6PD" },
                { "G6PD Date", "G6PD Date" },
                { "G6PD Status", "G6PD Status" },
                { "Next Test Date", "HIV NEXT TEST DATE" },
                { "HIV", "HIV" },
                { "Lipid Needed", "Lipid Needed" },
                { "LIPID PANEL", "LIPID PANEL" },
                { "Cholesterol / HDL Cholesterol", "Cholesterol / HDL Cholesterol" },
                { "Framingham", "Framingham" },
                { "EKG", "EKG" },
                { "EKG NEEDED", "EKG NEEDED" },
                { "hcg", "hcg" },
                { "IMM", "IMM" },
                { "Hep B", "Hep B" },
                { "Hep A", "Hep A" },
                { "FLU", "FLU" },
                { "Tet/TDP", "Tet/TDP" },
                { "MMR", "MMR" },
                { "Varicella", "Varicella" },
                { "TaskForce","TaskForce"},
                { "Over 44", "Over 44" },
                { "Notes", "Notes" },
                { "EventDate", "EventDate" },
                { "Event End Date", "Event End Date" },
                { "EventID", "EventID" },
                { "Vision_Win", "Vision_Win" },
                { "Dental_Win", "Dental_Win" },
                { "PHA_Win", "PHA_Win" },
                { "HIV_Win", "HIV_Win" },
                { "Hearing_WIN", "Hearing_WIN" },
            };
        }

        private Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)> GetFileConfiguration()
        {
            return new Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)>
            {
                { "G6PDReport.xlsx", (new string[] { "Name","Rank","IMR Exclusion","SSN","DOD ID","Age","ACO/MOS","UIC","Location","CMP","G6PD Date","G6PD Status" }, new string[] { "DOD ID", "Name","SSN","G6PD Date","G6PD Status" }, "") },
                { "Taskforce_Roster.xlsx", (new string[] { "Name","Rank","SSN","Gender","MOS","MPC","UIC","DOB","Type" }, new string[] { "Name", "SSN", "DOB" }, "") },
                { "ImmunizationProfileReport.xlsx", (new string[] { "Name","Rank","SSN","CMP","UIC","AOC/MOS","HB3","HPA","MMR","TDP","TDA","VAR" }, new string[] { "Name", "SSN", "HB3","HPA","MMR","TDP","TDA","VAR" }, "Immunization Profile Report") },
                { "Pha_Report.xlsx", (new string[] { "Cbt Dfrmnt","Name","Rank","IMR Exclusion","SSN","Age","AOC/MOS","UIC","Location","CMP","PHA Date","Next PHA Date","PHA Completion Status","PULHES Code" }, new string[] { "Name", "SSN", "PHA Date","Next PHA Date","PULHES Code" }, "") },
                { "DentalReadinessReport.xlsx", (new string[] { "Cbt","Dfrmnt","DRC","Reason","Date of Next Exam","Panx","Name","DOD ID","Rank","IMR Exclusion","AOC/MOS","UIC","Station","CMP" }, new string[] { "DOD ID", "DRC","Reason","Date of Next Exam","Panx" }, "" ) },
                { "VisionReadinessReport.xlsx", (new string[] { "CBT Dfrmnt","VRC","Date of Vision Screen","2PG","1MI","MCEP","MCEPI","MRCL","Waiver","Name","DOD ID","Age","Rank","AOC/MOS","UIC","Station","Compo" }, new string[] { "DOD ID", "VRC","Date of Vision Screen","2PG","1MI" }, "") },
                { "SickleCellReport.xlsx", (new string[] { "Name","Rank","IMR Exclusion","SSN","DOD ID","Age","Sex","MOS","UIC","Location","CMP","Sickle Cell Status","Sickle Cell Date" }, new string[] { "DOD ID", "Sickle Cell Date" }, "") },
                { "HIVReadinessSummaryReport.xlsx", (new string[] { "Cbt Dfrmnt","HIV Test Current","Name","SSN","DOD ID","Rank","IMR Exclusion","AOC/MOS","UIC","CMP","AGR","Test Date","Next Test Date" }, new string[] { "DOD ID", "Next Test Date" }, "") },
                { "UMR.xlsx", (new string[] {  "Name", "DOD ID", "Rank", "IMR Exclusion", "Age", "Sex", "MOS", "CMP", "AGR", "UIC", "Location", "MRC", "Deployable", "MR2-DNA", "MR2-HIV", "MR2-IMM", "MR2-IME", "MR2-Temp>=8<=30", "MR2-HRC4", "MR2-VRC4", "MR3-Temp>=31 (DL1)", "MR3-DRC3 (DL2)","MR3-PRG (DL3)", "MR3-MAR2 (DL4)", "MR3-MEB (DL5)", "MR3-ND-PEB (DL6)","MR3-PERM (DL7)", "Dental Exam (DRC4)", "MR4-PHA" }, new string[] { "Name","DOD ID","Rank","Age","Sex","MOS","CMP","AGR","UIC","Location","MRC","MR2-DNA", "MR2-IMM" }, "") },
                { "HearingReadinessReport.xlsx", (new string[] { "Cbt Dfrmnt/Separation","HRC","Status","Audiogram Date","Earplugs Issued","Hearing Protection Type","Hearing Aid w/ Battery","Name","SSN","DOD ID","Rank","AOC/MOS","UIC","Location","CMP" }, new string[] { "DOD ID", "HRC","Status","Audiogram Date" }, "") },
                { "BloodTypeReport.xlsx", (new string[] { "R/G","Name","Rank","IMR Exclusion Code","SSN","DOD ID","Age","Sex","MOS","UIC","Location","CMP","Blood Type" }, new string[] { "DOD ID", "Blood Type" }, "") }

            };
        }

        private Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)> GetFileConfigurationForChecking()
        {
            return new Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)>
            {
                { "G6PDReport.xlsx", (new string[] { "Name","Rank","IMR Exclusion","SSN","DOD ID","Age","ACO/MOS","UIC","Location","CMP","G6PD Date","G6PD Status" }, new string[] { "DOD ID", "Name","SSN" }, "") },
                { "Taskforce_Roster.xlsx", (new string[] { "Name","Rank","SSN","Gender","MOS","MPC","UIC","DOB","Type" }, new string[] { "Name", "SSN", "DOB", "Rank", "Gender" }, "") },
                { "ImmunizationProfileReport.xlsx", (new string[] { "Name","Rank","SSN","CMP","UIC","AOC/MOS","HB3","HPA","MMR","TDP","TDA","VAR" }, new string[] { "Name", "SSN" }, "Immunization Profile Report") },
                { "Pha_Report.xlsx", (new string[] { "Cbt Dfrmnt","Name","Rank","IMR Exclusion","SSN","Age","AOC/MOS","UIC","Location","CMP","PHA Date","Next PHA Date","PHA Completion Status","PULHES Code" }, new string[] { "Name","SSN"  }, "") },
                { "DentalReadinessReport.xlsx", (new string[] { "Cbt","Dfrmnt","DRC","Reason","Date of Next Exam","Panx","Name","DOD ID","Rank","IMR Exclusion","AOC/MOS","UIC","Station","CMP" }, new string[] { "DOD ID", "DRC", "Name" }, "" ) },
                { "VisionReadinessReport.xlsx", (new string[] { "CBT Dfrmnt","VRC","Date of Vision Screen","2PG","1MI","MCEP","MCEPI","MRCL","Waiver","Name","DOD ID","Age","Rank","AOC/MOS","UIC","Station","Compo" }, new string[] { "DOD ID", "VRC","Name" }, "") },
                { "SickleCellReport.xlsx", (new string[] { "Name","Rank","IMR Exclusion","SSN","DOD ID","Age","Sex","MOS","UIC","Location","CMP","Sickle Cell Status","Sickle Cell Date" }, new string[] {  "DOD ID", "Name","SSN" }, "") },
                { "HIVReadinessSummaryReport.xlsx", (new string[] { "Cbt Dfrmnt","HIV Test Current","Name","SSN","DOD ID","Rank","IMR Exclusion","AOC/MOS","UIC","CMP","AGR","Test Date","Next Test Date" }, new string[] { "DOD ID", "Name","SSN" }, "") },
                { "UMR.xlsx", (new string[] {  "Name", "DOD ID", "Rank", "IMR Exclusion", "Age", "Sex", "MOS", "CMP", "AGR", "UIC", "Location", "MRC", "Deployable", "MR2-DNA", "MR2-HIV", "MR2-IMM", "MR2-IME", "MR2-Temp>=8<=30", "MR2-HRC4", "MR2-VRC4", "MR3-Temp>=31 (DL1)", "MR3-DRC3 (DL2)","MR3-PRG (DL3)", "MR3-MAR2 (DL4)", "MR3-MEB (DL5)", "MR3-ND-PEB (DL6)","MR3-PERM (DL7)", "Dental Exam (DRC4)", "MR4-PHA" }, new string[] { "Name","DOD ID","Rank","Sex","MOS","AGR", "UIC" }, "") },
                { "HearingReadinessReport.xlsx", (new string[] { "Cbt Dfrmnt/Separation","HRC","Status","Audiogram Date","Earplugs Issued","Hearing Protection Type","Hearing Aid w/ Battery","Name","SSN","DOD ID","Rank","AOC/MOS","UIC","Location","CMP" }, new string[] { "DOD ID", "Name","SSN", "HRC" }, "") },
                { "BloodTypeReport.xlsx", (new string[] { "R/G","Name","Rank","IMR Exclusion Code","SSN","DOD ID","Age","Sex","MOS","UIC","Location","CMP","Blood Type" }, new string[] { "DOD ID", "Name","SSN" }, "") }
            };
        }

        private List<string>? GetProcessingSequence()
        {
            return new List<string>
            {
                "G6PDReport.xlsx",
                "Taskforce_Roster.xlsx",
                "DentalReadinessReport.xlsx",
                "ImmunizationProfileReport.xlsx",
                "VisionReadinessReport.xlsx",
                "SickleCellReport.xlsx",
                "HIVReadinessSummaryReport.xlsx",
                "UMR.xlsx",
                "HearingReadinessReport.xlsx",
                "BloodTypeReport.xlsx",
                "Pha_Report.xlsx"
            };
        }

        private void MergeSomeMoreColumns(DataTable parentTable, string taskForceValue)
        {
            if (!parentTable.Columns.Contains("TaskForce"))
            {
                parentTable.Columns.Add("TaskForce", typeof(string));
            }

            if (!parentTable.Columns.Contains("QUEST"))
            {
                parentTable.Columns.Add("QUEST", typeof(string));
            }

            if (!parentTable.Columns.Contains("Lab Needed"))
            {
                parentTable.Columns.Add("Lab Needed", typeof(string));
            }

            foreach (DataRow parentRow in parentTable.Rows)
            {
                parentRow["TaskForce"] = taskForceValue;

                bool commonNeeded =
                    (parentTable.Columns.Contains("ABO Needed") && parentRow["ABO Needed"]?.ToString().Trim() == "NEEDED") ||
                    (parentTable.Columns.Contains("G6PD") && parentRow["G6PD"]?.ToString().Trim() == "NEEDED") ||
                    (parentTable.Columns.Contains("SICKLE") && parentRow["SICKLE"]?.ToString().Trim() == "NEEDED") ||
                    (parentTable.Columns.Contains("LIPID NEEDED") && parentRow["LIPID NEEDED"]?.ToString().Trim() == "NEEDED");

                bool additionalLabNeeded =
                    (parentTable.Columns.Contains("HIV") && parentRow["HIV"]?.ToString().Trim() == "NEEDED") ||
                    (parentTable.Columns.Contains("DNA") && parentRow["DNA"]?.ToString().Trim() == "NEEDED");

                if (commonNeeded)
                {
                    parentRow["QUEST"] = "NEEDED";
                }
                else
                {
                    parentRow["QUEST"] = "N/A";
                }

                if (commonNeeded || additionalLabNeeded)
                {
                    parentRow["Lab Needed"] = "NEEDED";
                }
                else
                {
                    parentRow["Lab Needed"] = "N/A";
                }
            }

        }

        private List<Dictionary<string, object>> DataTableToJson(DataTable table)
        {
            var result = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn column in table.Columns)
                {
                    dict[column.ColumnName] = row[column];
                }
                result.Add(dict);
            }

            return result;
        }

        private DataTable GetDataFromFile(IFormFile file, string[] headers, string[] selectedColumns, string stopKeyword, bool isG6PD, out string taskForceValue, out long totalRecordValueInG6pdFile)
        {
            DataTable dataTable = new DataTable();
            taskForceValue = string.Empty;
            totalRecordValueInG6pdFile = 0;

            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Get the first sheet

                IRow headerRow = null;
                bool headerFound = false;
                int headerRowIndex = 0;

                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row != null)
                    {
                        for (int j = 0; j < row.LastCellNum; j++)
                        {
                            string cellValue = row.GetCell(j)?.ToString();

                            if (isG6PD)
                            {
                                if (string.IsNullOrWhiteSpace(taskForceValue))
                                {
                                    if (!string.IsNullOrEmpty(cellValue) && NormalizeWhiteSpace(cellValue).Contains("G6PD Report by Taskforce:"))
                                    {
                                        taskForceValue = ExtractTaskForceValue(cellValue);
                                    }
                                }
                            }

                            if (headers.Contains(cellValue))
                            {
                                headerRow = row;
                                headerRowIndex = i;
                                headerFound = true;
                                break;
                            }
                        }
                    }

                    if (headerFound) break;
                }

                if (headerRow == null)
                    throw new Exception("Header row not found!");

                if (isG6PD)
                {
                    if (totalRecordValueInG6pdFile == 0)
                    {
                        totalRecordValueInG6pdFile = GetTotalAssignedRecordFromG6pdFile(sheet);
                    }
                }

                // Add selected columns to the DataTable
                foreach (var column in selectedColumns)
                {
                    dataTable.Columns.Add(column);
                }

                // Extract data from rows after the header
                for (int i = headerRowIndex + 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null || IsStopRow(row, stopKeyword)) break; // Stop if blank or stop keyword

                    DataRow dataRow = dataTable.NewRow();

                    for (int j = 0; j < row.LastCellNum; j++)
                    {
                        ICell cell = row.GetCell(j); // Get cell safely
                        if (cell == null) continue; // Skip if cell is null

                        string headerValue = headerRow.GetCell(j)?.ToString();
                        if (selectedColumns.Contains(headerValue))
                        {
                            // Check if the current header is in the list of date headers
                            if (dateHeaderNames.Contains(headerValue))
                            {
                                // Handle date formatting for date columns stored as string
                                string cellValue = cell.ToString();
                                if (DateTime.TryParse(cellValue, out DateTime parsedDate))
                                {
                                    dataRow[headerValue] = parsedDate.ToString(DateFormat); // Format as MM/dd/yyyy
                                }
                                else
                                {
                                    // If parsing fails, store the original value or handle it differently
                                    dataRow[headerValue] = cellValue;
                                }
                            }
                            else
                            {
                                // Otherwise, handle as a normal string value
                                dataRow[headerValue] = cell?.ToString();
                            }
                        }
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        private bool IsPossibleDate(string value)
        {
            // You can add more checks here based on what date formats you expect
            return DateTime.TryParse(value, out _);
        }

        private string NormalizeWhiteSpace(string input)
        {
            return string.Join(" ", input.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private string ExtractTaskForceValue(string cellValue)
        {
            int startIndex = cellValue.IndexOf("Taskforce:") + "Taskforce:".Length;
            int endIndex = cellValue.IndexOf(",", startIndex);

            if (startIndex > -1 && endIndex > -1)
            {
                return cellValue.Substring(startIndex, endIndex - startIndex).Trim();
            }

            return string.Empty; // Return an empty string if not found
        }

        private long GetTotalAssignedRecordFromG6pdFile(ISheet sheet)
        {
            string targetKey = "Total Assigned";

            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                IRow currentRow = sheet.GetRow(row);

                if (currentRow != null)
                {
                    for (int col = 0; col < currentRow.LastCellNum; col++)
                    {
                        ICell keyCell = currentRow.GetCell(col);

                        if (keyCell != null && keyCell.ToString() == targetKey)
                        {
                            IRow nextRow = sheet.GetRow(row + 1);
                            if (nextRow != null)
                            {
                                ICell valueCell = nextRow.GetCell(col);
                                if (valueCell != null)
                                {
                                    try
                                    {
                                        return Convert.ToInt64(valueCell.ToString());
                                    }
                                    catch (FormatException)
                                    {
                                        throw new Exception("Total assigned record not found in G6PD file or in different format!");
                                    }
                                    catch (OverflowException)
                                    {
                                        throw new Exception("Total assigned record not found in G6PD file or in different format!");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }

        private bool IsStopRow(IRow row, string stopKeyword)
        {
            bool isBlankRow = row == null || row.Cells.All(cell =>
        cell == null ||
        cell.CellType == CellType.Blank ||
        (cell.CellType == CellType.String && string.IsNullOrWhiteSpace(cell.StringCellValue)) ||
        (cell.CellType == CellType.Formula && string.IsNullOrWhiteSpace(cell.StringCellValue))
    );

            bool hasStopKeyword = !string.IsNullOrEmpty(stopKeyword) &&
                                  row.Cells.Any(c => c.CellType == CellType.String &&
                                                     c.StringCellValue.IndexOf(stopKeyword, StringComparison.OrdinalIgnoreCase) >= 0);

            return isBlankRow || hasStopKeyword;
        }

        private void MergeDataTables(DataTable parentTable, DataTable childTable, string[] mergeColumns, int lastDentalExamDate, DateTime? lastEventDate, DateTime visionDate, DateTime dentalDate, DateTime phaDate,
                             DateTime hivDate, DateTime hearingDate, bool isVisionFile, bool isDentalFile, bool isPhaFile, bool isHivFile,
                             bool isHearingFile, bool isTaskForceFile, bool isBloodFile, bool isUmrFile,
                             bool isSickleFile, bool isImmunizationFile, DateTime eventDate, int vision, int dental, int pha, int hiv, int hearing, string eventId)
        {

            AddCalculatedFieldColumns(parentTable);

            //int rowNumber = 1;

            foreach (DataRow parentRow in parentTable.Rows)
            {
                //parentRow["SM ID"] = rowNumber++;
                parentRow["ONLY_NAME"] = string.Empty;

                DataRow matchingChildRow = GetMatchingChildRow(parentTable, childTable, parentRow);

                if (matchingChildRow != null)
                {
                    MergeRows(parentTable, childTable, mergeColumns, matchingChildRow, parentRow, isTaskForceFile);


                    if (isVisionFile)
                    {
                        HandleVisionFile(parentRow, visionDate);
                    }

                    if (isDentalFile)
                    {
                        HandleDentalFile(parentRow, dentalDate, lastDentalExamDate);
                    }

                    if (isPhaFile)
                    {
                        HandlePhaFile(parentRow, phaDate);
                    }

                    if (isHivFile)
                    {
                        HandleHivFile(parentRow, hivDate);
                    }

                    if (isHearingFile)
                    {
                        HandleHearingFile(parentRow, hearingDate);
                    }

                    if (isTaskForceFile)
                    {
                        HandleTaskForceFile(parentRow);
                    }

                    if (isBloodFile)
                    {
                        HandleBloodFile(parentRow);
                    }

                    if (isUmrFile)
                    {
                        HandleUmrFile(parentRow);
                    }

                    if (isSickleFile)
                    {
                        HandleSickleFile(parentRow);
                    }

                    if (isImmunizationFile)
                    {
                        HandleImmunizationFile(parentRow, lastEventDate);
                    }

                    HandleOtherFields(parentRow, eventDate, lastEventDate, vision, dental, pha, hiv, hearing, eventId);
                }
            }
        }

        private DataRow GetMatchingChildRow(DataTable parentTable, DataTable childTable, DataRow parentRow)
        {
            DataRow matchingChildRow = null;

            if (parentTable.Columns.Contains("DOD ID") && childTable.Columns.Contains("DOD ID"))
            {
                matchingChildRow = childTable.Rows.Cast<DataRow>()
                    .FirstOrDefault(row => row["DOD ID"]?.ToString() == parentRow["DOD ID"]?.ToString());
            }

            if (matchingChildRow == null && parentTable.Columns.Contains("Name") && parentTable.Columns.Contains("SSN") && childTable.Columns.Contains("Name") && childTable.Columns.Contains("SSN"))
            {
                matchingChildRow = childTable.Rows.Cast<DataRow>()
                    .FirstOrDefault(row => row["Name"]?.ToString() == parentRow["Name"]?.ToString() &&
                    row["SSN"]?.ToString()?.Substring(Math.Max(0, row["SSN"].ToString().Length - 4)) ==
                    parentRow["SSN"]?.ToString()?.Substring(Math.Max(0, parentRow["SSN"].ToString().Length - 4)));
            }

            return matchingChildRow;
        }

        private void MergeRows(DataTable parentTable, DataTable childTable, string[] mergeColumns, DataRow matchingChildRow, DataRow parentRow, bool isTaskForceFile)
        {
            foreach (var column in mergeColumns)
            {
                if (childTable.Columns.Contains(column))
                {
                    if (isTaskForceFile && column == "SSN")
                    {
                        parentRow["FULL SSN"] = matchingChildRow[column];
                    }
                    else if (column != "DOD ID" && column != "Name" && column != "SSN")
                    {
                        if (parentTable.Columns.Contains(column))
                        {
                            if (parentRow[column] == DBNull.Value || string.IsNullOrEmpty(parentRow[column].ToString()))
                            {
                                parentRow[column] = matchingChildRow[column];
                            }
                        }
                        else
                        {
                            parentTable.Columns.Add(column, childTable.Columns[column].DataType);
                            parentRow[column] = matchingChildRow[column];
                        }
                    }
                }
            }
        }

        private void AddCalculatedFieldColumns(DataTable parentTable)
        {
            //if (!parentTable.Columns.Contains("SM ID"))
            //    parentTable.Columns.Add("SM ID", typeof(int));

            // Add "NAME" column if it doesn't already exist (initially blank)
            if (!parentTable.Columns.Contains("ONLY_NAME"))
                parentTable.Columns.Add("ONLY_NAME", typeof(string));

            if (!parentTable.Columns.Contains("FULL SSN"))
                parentTable.Columns.Add("FULL SSN", typeof(string));

            if (!parentTable.Columns.Contains("VISION"))
                parentTable.Columns.Add("VISION", typeof(string));

            if (!parentTable.Columns.Contains("Dental Needed"))
                parentTable.Columns.Add("Dental Needed", typeof(string));

            if (!parentTable.Columns.Contains("Dental Exam"))
                parentTable.Columns.Add("Dental Exam", typeof(string));

            if (!parentTable.Columns.Contains("PHA"))
                parentTable.Columns.Add("PHA", typeof(string));

            if (!parentTable.Columns.Contains("HIV"))
                parentTable.Columns.Add("HIV", typeof(string));

            if (!parentTable.Columns.Contains("HEARING"))
                parentTable.Columns.Add("HEARING", typeof(string));

            if (!parentTable.Columns.Contains("PANO NEEDED"))
                parentTable.Columns.Add("PANO NEEDED", typeof(string));

            if (!parentTable.Columns.Contains("BWX Needed"))
                parentTable.Columns.Add("BWX Needed", typeof(string));

            if (!parentTable.Columns.Contains("Hearing Profile"))
                parentTable.Columns.Add("Hearing Profile", typeof(string));

            if (!parentTable.Columns.Contains("ABO Needed"))
                parentTable.Columns.Add("ABO Needed", typeof(string));

            if (!parentTable.Columns.Contains("DNA"))
                parentTable.Columns.Add("DNA", typeof(string));

            if (!parentTable.Columns.Contains("SICKLE"))
                parentTable.Columns.Add("SICKLE", typeof(string));

            if (!parentTable.Columns.Contains("G6PD"))
                parentTable.Columns.Add("G6PD", typeof(string));

            if (!parentTable.Columns.Contains("Lipid Needed"))
                parentTable.Columns.Add("Lipid Needed", typeof(string));

            if (!parentTable.Columns.Contains("LIPID PANEL"))
                parentTable.Columns.Add("LIPID PANEL", typeof(string));

            if (!parentTable.Columns.Contains("Cholesterol / HDL Cholesterol"))
                parentTable.Columns.Add("Cholesterol / HDL Cholesterol", typeof(string));

            if (!parentTable.Columns.Contains("Framingham"))
                parentTable.Columns.Add("Framingham", typeof(string));

            if (!parentTable.Columns.Contains("EKG"))
                parentTable.Columns.Add("EKG", typeof(string));

            if (!parentTable.Columns.Contains("EKG NEEDED"))
                parentTable.Columns.Add("EKG NEEDED", typeof(string));

            if (!parentTable.Columns.Contains("hcg"))
                parentTable.Columns.Add("hcg", typeof(string));

            if (!parentTable.Columns.Contains("IMM"))
                parentTable.Columns.Add("IMM", typeof(string));

            if (!parentTable.Columns.Contains("Hep B"))
                parentTable.Columns.Add("Hep B", typeof(string));

            if (!parentTable.Columns.Contains("Hep A"))
                parentTable.Columns.Add("Hep A", typeof(string));

            if (!parentTable.Columns.Contains("FLU"))
                parentTable.Columns.Add("FLU", typeof(string));

            if (!parentTable.Columns.Contains("Tet/TDP"))
                parentTable.Columns.Add("Tet/TDP", typeof(string));

            if (!parentTable.Columns.Contains("MMR"))
                parentTable.Columns.Add("MMR", typeof(string));

            if (!parentTable.Columns.Contains("Varicella"))
                parentTable.Columns.Add("Varicella", typeof(string));

            if (!parentTable.Columns.Contains("Over 44"))
                parentTable.Columns.Add("Over 44", typeof(string));

            if (!parentTable.Columns.Contains("Notes"))
                parentTable.Columns.Add("Notes", typeof(string));

            if (!parentTable.Columns.Contains("EventDate"))
                parentTable.Columns.Add("EventDate", typeof(string));

            if (!parentTable.Columns.Contains("Event End Date"))
                parentTable.Columns.Add("Event End Date", typeof(string));

            if (!parentTable.Columns.Contains("EventID"))
                parentTable.Columns.Add("EventID", typeof(string));

            if (!parentTable.Columns.Contains("Vision_Win"))
                parentTable.Columns.Add("Vision_Win", typeof(string));

            if (!parentTable.Columns.Contains("Dental_Win"))
                parentTable.Columns.Add("Dental_Win", typeof(string));

            if (!parentTable.Columns.Contains("PHA_Win"))
                parentTable.Columns.Add("PHA_Win", typeof(string));

            if (!parentTable.Columns.Contains("HIV_Win"))
                parentTable.Columns.Add("HIV_Win", typeof(string));

            if (!parentTable.Columns.Contains("Hearing_WIN"))
                parentTable.Columns.Add("Hearing_WIN", typeof(string));
        }

        private void HandleVisionFile(DataRow parentRow, DateTime visionDate)
        {
            if (parentRow.Table.Columns.Contains("Date of Vision Screen") && DateTime.TryParse(parentRow["Date of Vision Screen"]?.ToString(), out DateTime visionScreenDate))
            {
                parentRow["VISION"] = visionDate >= visionScreenDate ? "NEEDED" : "N/A";
            }
            else
            {
                parentRow["VISION"] = "N/A";
            }
        }

        private void HandleDentalFile(DataRow parentRow, DateTime dentalDate, int lastDentalExamDate)
        {
            if (parentRow.Table.Columns.Contains("Date of Next Exam") && DateTime.TryParse(parentRow["Date of Next Exam"]?.ToString(), out DateTime dentalExamDate))
            {
                parentRow["Dental Needed"] = dentalDate >= dentalExamDate ? "NEEDED" : "N/A";
                parentRow["BWX Needed"] = dentalDate >= dentalExamDate ? "NEEDED" : "N/A";
                parentRow["Dental Exam"] = dentalExamDate.AddDays(-lastDentalExamDate).ToString(DateFormat);
            }
            else
            {
                parentRow["Dental Needed"] = "N/A";
                parentRow["BWX Needed"] = "N/A";
                parentRow["Dental Exam"] = "";
            }

            if (parentRow.Table.Columns.Contains("PANX"))
            {
                string panxValue = parentRow["PANX"]?.ToString();
                parentRow["PANO NEEDED"] = (panxValue == "N" || string.IsNullOrEmpty(panxValue)) ? "NEEDED" : "N/A";
            }
            else
            {
                parentRow["PANO NEEDED"] = "N/A";
            }
        }

        private void HandlePhaFile(DataRow parentRow, DateTime phaDate)
        {
            if (parentRow.Table.Columns.Contains("Next PHA Date") && DateTime.TryParse(parentRow["Next PHA Date"]?.ToString(), out DateTime phaExamDate))
            {
                parentRow["PHA"] = phaDate >= phaExamDate ? "NEEDED" : "N/A";
            }
            else
            {
                parentRow["PHA"] = "N/A";
            }

            if (parentRow.Table.Columns.Contains("PULHES Code"))
            {
                string pulhesCode = parentRow["PULHES Code"]?.ToString();
                parentRow["Hearing Profile"] = !string.IsNullOrEmpty(pulhesCode) && pulhesCode.Length >= 4 ? pulhesCode[3].ToString() : "";
            }
        }

        private void HandleHivFile(DataRow parentRow, DateTime hivDate)
        {
            if (parentRow.Table.Columns.Contains("Next Test Date"))
            {
                string nextTestDateValue = parentRow["Next Test Date"]?.ToString();

                if (string.IsNullOrEmpty(nextTestDateValue))
                {
                    parentRow["HIV"] = "NEEDED";
                }
                else if (DateTime.TryParse(nextTestDateValue, out DateTime hivTestDate))
                {
                    parentRow["HIV"] = hivDate >= hivTestDate ? "NEEDED" : "N/A";
                }
                else
                {
                    parentRow["HIV"] = "N/A";
                }
            }
            else
            {
                parentRow["HIV"] = "N/A";
            }
        }

        private void HandleHearingFile(DataRow parentRow, DateTime hearingDate)
        {
            if (parentRow.Table.Columns.Contains("Audiogram Date") && DateTime.TryParse(parentRow["Audiogram Date"]?.ToString(), out DateTime hearingTestDate))
            {
                parentRow["HEARING"] = hearingDate >= hearingTestDate ? "NEEDED" : "N/A";
            }
            else
            {
                parentRow["HEARING"] = "N/A";
            }
        }

        private void HandleTaskForceFile(DataRow parentRow)
        {
            if (parentRow.Table.Columns.Contains("DOB") && DateTime.TryParse(parentRow["DOB"]?.ToString(), out DateTime dobTaskForce))
            {
                // Calculate exact age
                var today = DateTime.Now;
                int age = today.Year - dobTaskForce.Year;
                if (dobTaskForce > today.AddYears(-age)) age--; // Adjust if birthday hasn't occurred this year

                // Calculate whether age is greater than 39.5 years
                double exactAge = (today - dobTaskForce).TotalDays / 365.25; // Using 365.25 to account for leap years

                // Over 40 logic
                if (age > 40)
                {
                    if (!parentRow.Table.Columns.Contains("OVER 40"))
                    {
                        parentRow.Table.Columns.Add("OVER 40", typeof(string));
                    }

                    parentRow["OVER 40"] = "YES";  // Mark the user as over 40
                }
                else
                {
                    if (!parentRow.Table.Columns.Contains("OVER 40"))
                    {
                        parentRow.Table.Columns.Add("OVER 40", typeof(string));
                    }

                    parentRow["OVER 40"] = "NO"; // Mark the user as under 40
                }

                // Near Vision logic
                if (age > 45)
                {
                    if (!parentRow.Table.Columns.Contains("NEAR VISION"))
                    {
                        parentRow.Table.Columns.Add("NEAR VISION", typeof(string));
                    }

                    parentRow["NEAR VISION"] = "NEEDED";
                }
                else
                {
                    if (!parentRow.Table.Columns.Contains("NEAR VISION"))
                    {
                        parentRow.Table.Columns.Add("NEAR VISION", typeof(string));
                    }

                    parentRow["NEAR VISION"] = "N/A";
                }

                // Over 44 logic
                if (age > 44)
                {
                    if (!parentRow.Table.Columns.Contains("OVER 44"))
                    {
                        parentRow.Table.Columns.Add("OVER 44", typeof(string));
                    }

                    parentRow["OVER 44"] = "YES";
                }
                else
                {
                    if (!parentRow.Table.Columns.Contains("OVER 44"))
                    {
                        parentRow.Table.Columns.Add("OVER 44", typeof(string));
                    }

                    parentRow["OVER 44"] = "NO";
                }

                // Additional logic for exact age > 39.5
                if (exactAge > 39.5)  // Check if exact age is greater than 39.5
                {
                    // Add columns if not already present and set "NEEDED"
                    if (!parentRow.Table.Columns.Contains("LIPID NEEDED"))
                    {
                        parentRow.Table.Columns.Add("LIPID NEEDED", typeof(string));
                    }
                    parentRow["LIPID NEEDED"] = "NEEDED";

                    if (!parentRow.Table.Columns.Contains("LIPID PANEL"))
                    {
                        parentRow.Table.Columns.Add("LIPID PANEL", typeof(string));
                    }
                    parentRow["LIPID PANEL"] = "NEEDED";

                    if (!parentRow.Table.Columns.Contains("Cholesterol / HDL Cholesterol"))
                    {
                        parentRow.Table.Columns.Add("Cholesterol / HDL Cholesterol", typeof(string));
                    }
                    parentRow["Cholesterol / HDL Cholesterol"] = "NEEDED";

                    if (!parentRow.Table.Columns.Contains("EKG"))
                    {
                        parentRow.Table.Columns.Add("EKG", typeof(string));
                    }
                    parentRow["EKG"] = "NEEDED";

                    if (!parentRow.Table.Columns.Contains("EKG NEEDED"))
                    {
                        parentRow.Table.Columns.Add("EKG NEEDED", typeof(string));
                    }
                    parentRow["EKG NEEDED"] = "NEEDED";
                }
                else
                {
                    // Set columns to "N/A" if age is less than 39.5
                    if (!parentRow.Table.Columns.Contains("LIPID NEEDED"))
                    {
                        parentRow.Table.Columns.Add("LIPID NEEDED", typeof(string));
                    }
                    parentRow["LIPID NEEDED"] = "N/A";

                    if (!parentRow.Table.Columns.Contains("LIPID PANEL"))
                    {
                        parentRow.Table.Columns.Add("LIPID PANEL", typeof(string));
                    }
                    parentRow["LIPID PANEL"] = "N/A";

                    if (!parentRow.Table.Columns.Contains("Cholesterol / HDL Cholesterol"))
                    {
                        parentRow.Table.Columns.Add("Cholesterol / HDL Cholesterol", typeof(string));
                    }
                    parentRow["Cholesterol / HDL Cholesterol"] = "N/A";

                    if (!parentRow.Table.Columns.Contains("EKG"))
                    {
                        parentRow.Table.Columns.Add("EKG", typeof(string));
                    }
                    parentRow["EKG"] = "N/A";

                    if (!parentRow.Table.Columns.Contains("EKG NEEDED"))
                    {
                        parentRow.Table.Columns.Add("EKG NEEDED", typeof(string));
                    }
                    parentRow["EKG NEEDED"] = "N/A";
                }
            }
        }

        private void HandleBloodFile(DataRow parentRow)
        {
            if (parentRow.Table.Columns.Contains("Blood Type"))
            {
                parentRow["ABO Needed"] = string.IsNullOrEmpty(parentRow["Blood Type"]?.ToString()) ? "NEEDED" : "N/A";
            }
            else
            {
                parentRow["ABO Needed"] = "N/A"; // Default to "Yes" if the "Blood Type" column doesn't exist
            }
        }

        private void HandleUmrFile(DataRow parentRow)
        {
            if (parentRow.Table.Columns.Contains("MR2-DNA"))
            {
                parentRow["DNA"] = parentRow["MR2-DNA"]?.ToString() == "N" ? "NEEDED" : "N/A";
            }
            else
            {
                parentRow["DNA"] = "N/A";
            }

            if (parentRow.Table.Columns.Contains("SEX") && parentRow["SEX"] != null)
            {
                string sex = parentRow["SEX"].ToString().Trim();

                // Ensure "HCG" column exists
                if (!parentRow.Table.Columns.Contains("HCG"))
                {
                    parentRow.Table.Columns.Add("HCG", typeof(string));
                }

                if (sex == "M")
                {
                    parentRow["HCG"] = "N/A";
                }
                else if (sex == "F")
                {
                    parentRow["HCG"] = "";
                }
            }

            if (parentRow.Table.Columns.Contains("MR2-IMM") && parentRow["MR2-IMM"] != null)
            {
                string mr2ImmValue = parentRow["MR2-IMM"].ToString().Trim();

                // Ensure "IMM" column exists
                if (!parentRow.Table.Columns.Contains("IMM"))
                {
                    parentRow.Table.Columns.Add("IMM", typeof(string));
                }

                if (string.IsNullOrEmpty(mr2ImmValue))
                {
                    parentRow["IMM"] = "N/A";
                }
                else
                {
                    parentRow["IMM"] = "NEEDED";
                }
            }

        }

        private void HandleSickleFile(DataRow parentRow)
        {
            if (parentRow.Table.Columns.Contains("Sickle Cell Date"))
            {
                parentRow["SICKLE"] = string.IsNullOrEmpty(parentRow["Sickle Cell Date"]?.ToString()) ? "NEEDED" : "N/A";
            }
            else
            {
                parentRow["SICKLE"] = "N/A"; // Default to "Needed" if the "Sickle Cell Date" column doesn't exist
            }
        }

        private void HandleImmunizationFile(DataRow parentRow, DateTime? lastEventDate)
        {
            if (parentRow.Table.Columns.Contains("HB3"))
            {
                if (string.IsNullOrEmpty(parentRow["HB3"]?.ToString()) ||
                    parentRow["HB3"].ToString().StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(parentRow["HB3"]?.ToString(), out DateTime hb3Date) && hb3Date < lastEventDate.Value))
                {
                    parentRow["Hep B"] = "NEEDED";
                }
                else
                {
                    parentRow["Hep B"] = "N/A";
                }
            }
            else
            {
                parentRow["Hep B"] = "N/A";
            }

            if (parentRow.Table.Columns.Contains("HPA"))
            {
                if (string.IsNullOrEmpty(parentRow["HPA"]?.ToString()) ||
                    parentRow["HPA"].ToString().StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(parentRow["HPA"]?.ToString(), out DateTime HPADate) && HPADate < lastEventDate.Value))
                {
                    parentRow["Hep A"] = "NEEDED";
                }
                else
                {
                    parentRow["Hep A"] = "N/A";
                }
            }
            else
            {
                parentRow["Hep A"] = "N/A";
            }

            if (parentRow.Table.Columns.Contains("INJ"))
            {
                if (string.IsNullOrEmpty(parentRow["INJ"]?.ToString()) ||
                    parentRow["INJ"].ToString().StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(parentRow["INJ"]?.ToString(), out DateTime INJDate) && INJDate < lastEventDate.Value))
                {
                    parentRow["FLU"] = "NEEDED";
                }
                else
                {
                    parentRow["FLU"] = "N/A";
                }
            }
            else
            {
                parentRow["FLU"] = "N/A";
            }

            if (parentRow.Table.Columns.Contains("TDP"))
            {
                if (string.IsNullOrEmpty(parentRow["TDP"]?.ToString()) ||
                    parentRow["TDP"].ToString().StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(parentRow["TDP"]?.ToString(), out DateTime TDPDate) && TDPDate < lastEventDate.Value))
                {
                    parentRow["Tet/TDP"] = "NEEDED";
                }
                else
                {
                    parentRow["Tet/TDP"] = "N/A";
                }
            }
            else
            {
                parentRow["Tet/TDP"] = "N/A";
            }

            if (parentRow.Table.Columns.Contains("MMR"))
            {
                if (string.IsNullOrEmpty(parentRow["MMR"]?.ToString()) ||
                    parentRow["MMR"].ToString().StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(parentRow["MMR"]?.ToString(), out DateTime MMRDate) && MMRDate < lastEventDate.Value))
                {
                    parentRow["MMR"] = "NEEDED";
                }
                else
                {
                    parentRow["MMR"] = "N/A";
                }
            }
            else
            {
                parentRow["MMR"] = "N/A";
            }

            if (parentRow.Table.Columns.Contains("VAR"))
            {
                if (string.IsNullOrEmpty(parentRow["VAR"]?.ToString()) ||
                    parentRow["VAR"].ToString().StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(parentRow["VAR"]?.ToString(), out DateTime VARDate) && VARDate < lastEventDate.Value))
                {
                    parentRow["Varicella"] = "NEEDED";
                }
                else
                {
                    parentRow["Varicella"] = "N/A";
                }
            }
            else
            {
                parentRow["Varicella"] = "N/A";
            }
        }

        private void HandleOtherFields(DataRow parentRow, DateTime eventDate, DateTime? lastEventDate, int vision, int dental, int pha, int hiv, int hearing, string eventId)
        {
            if (parentRow.Table.Columns.Contains("G6PD Status"))
            {
                parentRow["G6PD"] = string.IsNullOrEmpty(parentRow["G6PD Status"]?.ToString()) ? "NEEDED" : "N/A";
            }
            else
            {
                parentRow["G6PD"] = "N/A"; // Default to "NEEDED" if the "G6PD date" column doesn't exist
            }

            if (parentRow.Table.Columns.Contains("EventDate"))
            {
                parentRow["EventDate"] = eventDate.ToString(DateFormat);
            }
            else
            {
                parentRow["EventDate"] = "";
            }


            if (parentRow.Table.Columns.Contains("Event End Date"))
            {
                parentRow["Event End Date"] = lastEventDate.HasValue ? lastEventDate.Value.ToString(DateFormat) : "";
            }
            else
            {
                parentRow["Event End Date"] = "";
            }

            if (parentRow.Table.Columns.Contains("Vision_Win"))
            {
                parentRow["Vision_Win"] = vision;
            }
            else
            {
                parentRow["Vision_Win"] = "";
            }

            if (parentRow.Table.Columns.Contains("Dental_Win"))
            {
                parentRow["Dental_Win"] = dental;
            }
            else
            {
                parentRow["Dental_Win"] = "";
            }

            if (parentRow.Table.Columns.Contains("PHA_Win"))
            {
                parentRow["PHA_Win"] = pha;
            }
            else
            {
                parentRow["PHA_Win"] = "";
            }

            if (parentRow.Table.Columns.Contains("HIV_Win"))
            {
                parentRow["HIV_Win"] = hiv;
            }
            else
            {
                parentRow["HIV_Win"] = "";
            }

            if (parentRow.Table.Columns.Contains("Hearing_WIN"))
            {
                parentRow["Hearing_WIN"] = hearing;
            }
            else
            {
                parentRow["Hearing_WIN"] = "";
            }

            if (parentRow.Table.Columns.Contains("EventID"))
            {
                parentRow["EventID"] = eventId;
            }
            else
            {
                parentRow["EventID"] = "";
            }
        }
    }
}

using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NPOI.HSSF.Record;
using System.Reflection;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class VitalStationService : IVitalStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IFileUploader _fileUploader;
        private readonly ILogger<VitalStationService> _logger;
        private readonly IEventManagementService _eventManagementService;
        private readonly IContractService _contractService;
        private const string CLASSNAME = "VitalStationService";


        public VitalStationService(ILogger<VitalStationService> logger, IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IPdfGeneratorService pdfGenerator, IEventManagementService eventManagementService, IFileUploader fileUploader, IContractService contractService)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _fileUploader = fileUploader;
            _eventManagementService = eventManagementService;
            _contractService = contractService;
            _logger = logger;
        }

        public async Task<LabStation?> GetByIdAsync(long id)
        {
            try
            {
                return await _unitOfWork.LabStation.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                // optional: log error here
                throw new Exception($"Service error in GetByIdAsync: {ex.Message}", ex);
            }
        }

        //public async Task<LabStation> GetByIdWithParentAsync(long id)
        //{
        //    return await _unitOfWork.LabStation.GetWithInclude(x => x.Id == id, x => x.FileData).FirstOrDefaultAsync();
        //}

        public async Task<VitalStationVM> GetVitalStationByServiceMemberChildIdAsync(long serviceMemberChildId)
        {
            const string methodName = "GetVitalStationByServiceMemberChildIdAsync";

            try
            {
                var result = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.Id == serviceMemberChildId,
                        c => c.ServiceMembersParent.EventManagement,
                        c => c.VitalStationRecord,
                        c => c.VitalStationRecord.BloodPressureReadings
                    )
                    .Select(c => new VitalStationVM
                    {
                        EventId = c.ServiceMembersParent.EventManagement.Id,
                        EventID = c.ServiceMembersParent.EventManagement.EventID,

                        ServiceMember = new ServiceMembersChildDto
                        {
                            FullName = c.FullName,
                            DodId = c.DodId,
                            Barcode = c.Barcode,
                            Dob = c.Dob,
                            Age = c.Age,
                            Sex = c.Sex
                        },

                        VitalStationDto = c.VitalStationRecord == null
                            ? null
                            : new VitalStationDto
                            {
                                Id = c.VitalStationRecord.Id,
                                ServiceMembersChildId = c.VitalStationRecord.ServiceMembersChildId,
                                Height = c.VitalStationRecord.Height,
                                Weight = c.VitalStationRecord.Weight,
                                FinalBpStatus = c.VitalStationRecord.FinalBpStatus,
                                TotalReadingsTaken = c.VitalStationRecord.TotalReadingsTaken,
                                Status = c.VitalStationRecord.Status,

                                IsNextReadingRequired =
                                    c.VitalStationRecord.Status != "Completed"
                                    && c.VitalStationRecord.TotalReadingsTaken < 3,

                                NextReadingNumber = c.VitalStationRecord.TotalReadingsTaken + 1,

                                NextReadingAfterMinutes = 15,

                                Message = c.VitalStationRecord.Status == "Completed"
                                    ? "Vitals completed"
                                    : $"Reading {c.VitalStationRecord.TotalReadingsTaken + 1} required after 15 minutes",

                                BloodPressureReadings = c.VitalStationRecord.BloodPressureReadings
                                    .OrderBy(x => x.ReadingNumber)
                                    .Select(r => new VitalStationBpReadingDto
                                    {
                                        Id = r.Id,
                                        ReadingNumber = r.ReadingNumber,
                                        Systolic = r.Systolic,
                                        Diastolic = r.Diastolic,
                                        ReadingStatus = r.ReadingStatus,
                                        IsRetakeRequired = r.IsRetakeRequired,
                                        ReadingTakenAt = r.ReadingTakenAt,
                                        Remarks = r.Remarks
                                    })
                                    .ToList()
                            },

                        // You can extend VM if needed later
                    })
                    .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching VitalStation for ChildId: {Id}", serviceMemberChildId);
                throw;
            }
        }

        public async Task AddAsync(VitalStationDto model, string userName)
        {
            model.AddedOn = DateTime.Now;
            model.AddedBy = userName;

            SetGivenDateTimes(model); // Set Completed date-times

            await _unitOfWork.LabStation.AddAsync(model);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(LabStation model, string userName)
        {
            string methodName = nameof(UpdateAsync);

            try
            {
                var existing = await _unitOfWork.LabStation.GetByIdAsync(model.Id);

                if (existing == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Lab Station record with Id={Id} not found by user {User}",
                        CLASSNAME, methodName, model.Id, userName);

                    throw new KeyNotFoundException($"Lab Station record with Id={model.Id} not found.");
                }

                MapToEntity(model, existing, userName);

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Lab Station record with Id={Id} successfully updated by user {User}",
                    CLASSNAME, methodName, model.Id, userName);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogError(knfEx,
                    "{ClassName}, {MethodName}, KeyNotFoundException occurred while updating Lab Station record Id={Id} by user {User}",
                    CLASSNAME, methodName, model.Id, userName);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while updating Lab Station record Id={Id} by user {User}",
                    CLASSNAME, methodName, model.Id, userName);

                throw;
            }
        }

        private void MapToEntity(LabStation source, LabStation target, string userName)
        {
            //G6pd

            if (source.G6pdNeeded.IsNullOrEmpty())
            {
                target.G6pdGivenDateTime = null;
                target.G6pdReason = null;
            }
            else if (source.G6pdNeeded == "Not Completed")
            {
                target.G6pdGivenDateTime = null;
                target.G6pdReason = source.G6pdReason;
            }
            else
            {
                if (source.G6pdNeeded != target.G6pdNeeded)
                {
                    target.G6pdGivenDateTime = DateTime.Now;
                }
            }

            //ABO 

            if (source.AboNeeded.IsNullOrEmpty())
            {
                target.AboGivenDateTime = null;
                target.AboGrouping = null;
                target.AboRhFactor = null;
                target.AboReason = null;
            }
            else if (source.AboNeeded == "Not Completed")
            {
                target.AboGivenDateTime = null;
                target.AboGrouping = null;
                target.AboRhFactor = null;
                target.AboReason = source.AboReason;
            }
            else
            {
                if (source.AboNeeded != target.AboNeeded)
                {
                    target.AboGivenDateTime = DateTime.Now;
                }

                target.AboReason = null;
                target.AboGrouping = source.AboGrouping;
                target.AboRhFactor = source.AboRhFactor;
            }

            //Lipid

            if (source.LipidPanelNeeded.IsNullOrEmpty())
            {
                target.LipidPanelGivenDateTime = null;
                target.LipidPanelReason = null;
                target.LipidPanelRapidTesting = false;
                target.TotalCholesterol = null;
                target.TotalCholesterol_LessThan100 = false;
                target.TotalCholesterol_GreaterThan400 = false;
                target.HdlCholesterol = null;
                target.HdlCholesterol_LessThan20 = false;
                target.HdlCholesterol_GreaterThan120 = false;
                target.Triglycerides = null;
                target.Triglycerides_LessThan50 = false;
                target.Triglycerides_GreaterThan500 = false;
                target.Glucose = null;
                target.Glucose_LessThan20 = false;
                target.Glucose_GreaterThan600 = false;
                target.LdlCholesterol = null;
                target.TotalCholesterolHdlRatio = null;
                target.NonHdlCholesterol = null;
                target.LdlHdlLipoprotiens = null;
                target.A1C = null;
            }
            else if (source.LipidPanelNeeded == "Not Completed")
            {
                target.LipidPanelReason = source.LipidPanelReason;
                target.LipidPanelGivenDateTime = null;
                target.LipidPanelRapidTesting = false;
                target.TotalCholesterol = null;
                target.TotalCholesterol_LessThan100 = false;
                target.TotalCholesterol_GreaterThan400 = false;
                target.HdlCholesterol = null;
                target.HdlCholesterol_LessThan20 = false;
                target.HdlCholesterol_GreaterThan120 = false;
                target.Triglycerides = null;
                target.Triglycerides_LessThan50 = false;
                target.Triglycerides_GreaterThan500 = false;
                target.Glucose = null;
                target.Glucose_LessThan20 = false;
                target.Glucose_GreaterThan600 = false;
                target.LdlCholesterol = null;
                target.TotalCholesterolHdlRatio = null;
                target.NonHdlCholesterol = null;
                target.LdlHdlLipoprotiens = null;
                target.A1C = null;
            }
            else
            {
                if (source.LipidPanelNeeded != target.LipidPanelNeeded)
                {
                    target.LipidPanelGivenDateTime = DateTime.Now;
                }

                if (source.LipidPanelRapidTesting)
                {
                    target.LipidPanelRapidTesting = source.LipidPanelRapidTesting;
                    target.TotalCholesterol = source.TotalCholesterol;
                    target.TotalCholesterol_LessThan100 = source.TotalCholesterol_LessThan100;
                    target.TotalCholesterol_GreaterThan400 = source.TotalCholesterol_GreaterThan400;
                    target.HdlCholesterol = source.HdlCholesterol;
                    target.HdlCholesterol_LessThan20 = source.HdlCholesterol_LessThan20;
                    target.HdlCholesterol_GreaterThan120 = source.HdlCholesterol_GreaterThan120;
                    target.Triglycerides = source.Triglycerides;
                    target.Triglycerides_LessThan50 = source.Triglycerides_LessThan50;
                    target.Triglycerides_GreaterThan500 = source.Triglycerides_GreaterThan500;
                    target.Glucose = source.Glucose;
                    target.Glucose_LessThan20 = source.Glucose_LessThan20;
                    target.Glucose_GreaterThan600 = source.Glucose_GreaterThan600;
                    target.LdlCholesterol = source.LdlCholesterol;
                    target.TotalCholesterolHdlRatio = source.TotalCholesterolHdlRatio;
                    target.NonHdlCholesterol = source.NonHdlCholesterol;
                    target.LdlHdlLipoprotiens = source.LdlHdlLipoprotiens;
                    target.A1C = source.A1C;
                    target.LipidPanelReason = null;
                }
                else
                {
                    target.LipidPanelRapidTesting = false;
                    target.TotalCholesterol = null;
                    target.TotalCholesterol_LessThan100 = false;
                    target.TotalCholesterol_GreaterThan400 = false;
                    target.HdlCholesterol = null;
                    target.HdlCholesterol_LessThan20 = false;
                    target.HdlCholesterol_GreaterThan120 = false;
                    target.Triglycerides = null;
                    target.Triglycerides_LessThan50 = false;
                    target.Triglycerides_GreaterThan500 = false;
                    target.Glucose = null;
                    target.Glucose_LessThan20 = false;
                    target.Glucose_GreaterThan600 = false;
                    target.LdlCholesterol = null;
                    target.TotalCholesterolHdlRatio = null;
                    target.NonHdlCholesterol = null;
                    target.LdlHdlLipoprotiens = null;
                    target.A1C = null;
                }
            }

            //HIV

            if (source.HivNeeded.IsNullOrEmpty())
            {
                target.HivGivenDateTime = null;
                target.HivReason = null;
                target.HivBarcodeCarebill = null;
            }
            else if (source.HivNeeded == "Not Completed")
            {
                target.HivGivenDateTime = null;
                target.HivBarcodeCarebill = null;
                target.HivReason = source.HivReason;
            }
            else
            {
                if (source.HivNeeded != target.HivNeeded)
                {
                    target.HivGivenDateTime = DateTime.Now;
                }

                target.HivReason = null;
                target.HivBarcodeCarebill = source.HivBarcodeCarebill;
            }

            //Pregnancy

            if (source.PregnancyTestNeeded.IsNullOrEmpty())
            {
                target.PregnancyTestGivenDateTime = null;
                target.PregnancyTestResult = null;
                target.PregnancyTestReason = null;
            }
            else if (source.PregnancyTestNeeded == "Not Completed")
            {
                target.PregnancyTestGivenDateTime = null;
                target.PregnancyTestResult = null;
                target.PregnancyTestReason = source.PregnancyTestReason;
            }
            else
            {
                if (source.PregnancyTestNeeded != target.PregnancyTestNeeded)
                {
                    target.PregnancyTestGivenDateTime = DateTime.Now;
                }

                target.PregnancyTestResult = source.PregnancyTestResult;
                target.PregnancyTestReason = null;
            }

            //SickleCell

            if (source.SickleCellNeeded.IsNullOrEmpty())
            {
                target.SickleCellGivenDateTime = null;
                target.SickleCellReason = null;
            }
            else if (source.SickleCellNeeded == "Not Completed")
            {
                target.SickleCellGivenDateTime = null;
                target.SickleCellReason = source.SickleCellReason;
            }
            else
            {
                if (source.SickleCellNeeded != target.SickleCellNeeded)
                {
                    target.SickleCellGivenDateTime = DateTime.Now;
                }
            }

            //DNA

            if (source.DnaNeeded.IsNullOrEmpty())
            {
                target.DnaGivenDateTime = null;
                target.DnaReason = null;
                target.DnaSerialNo = null;
            }
            else if (source.DnaNeeded == "Not Completed")
            {
                target.DnaGivenDateTime = null;
                target.DnaSerialNo = null;
                target.DnaReason = source.DnaReason;
            }
            else
            {
                if (source.DnaNeeded != target.DnaNeeded)
                {
                    target.DnaGivenDateTime = DateTime.Now;
                }

                target.DnaReason = null;
                target.DnaSerialNo = source.DnaSerialNo;
            }

            target.AreYouFasting = source.AreYouFasting;
            target.AnyComplicationInBloodDrawn = source.AnyComplicationInBloodDrawn;
            target.AllergicToLatex = source.AllergicToLatex;
            target.FeelAlright = source.FeelAlright;
            target.G6pdNeeded = source.G6pdNeeded;
            target.AboNeeded = source.AboNeeded;
            target.LipidPanelNeeded = source.LipidPanelNeeded;
            target.HivNeeded = source.HivNeeded;
            target.PregnancyTestNeeded = source.PregnancyTestNeeded;
            target.SickleCellNeeded = source.SickleCellNeeded;
            target.DnaNeeded = source.DnaNeeded;
            target.FedExTrackingNo = source.FedExTrackingNo;
            target.Status = source.Status;
            target.UpdatedOn = DateTime.Now;
            target.UpdatedBy = userName;

            
        }

        public void SetGivenDateTimes(LabStation model)
        {
            if (model == null) return;

            model.G6pdGivenDateTime = model.G6pdNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.AboGivenDateTime = model.AboNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.LipidPanelGivenDateTime = model.LipidPanelNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.HivGivenDateTime = model.HivNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.PregnancyTestGivenDateTime = model.PregnancyTestNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.SickleCellGivenDateTime = model.SickleCellNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.DnaGivenDateTime = model.DnaNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
        }
    }
}

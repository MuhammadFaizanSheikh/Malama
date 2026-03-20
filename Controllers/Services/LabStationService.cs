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
    public class LabStationService : ILabStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IFileUploader _fileUploader;
        private readonly IPdfGeneratorService _pdfGenerator;
        private readonly ILogger<LabStationService> _logger;
        private readonly IEventManagementService _eventManagementService;
        private readonly IContractService _contractService;
        private const string CLASSNAME = "LabStationService";


        public LabStationService(ILogger<LabStationService> logger, IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IPdfGeneratorService pdfGenerator, IEventManagementService eventManagementService, IFileUploader fileUploader, IContractService contractService)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _fileUploader = fileUploader;
            _eventManagementService = eventManagementService;
            _contractService = contractService;
            _pdfGenerator = pdfGenerator;
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

        public async Task<(LabStation LabStation, string EventId)> GetLabStationByIdWithEventIdAsync(long labStationId)
        {
            const string methodName = "GetLabStationByIdWithEventIdAsync";

            try
            {
                _logger.LogDebug("{ClassName}, {MethodName}, Fetching LabStation with Id: {Id}", labStationId);

                // Start from ServiceMembersChild but only project what we need
                var result = await _unitOfWork.ServiceMembersChild
                    .GetWithInclude(
                        c => c.LabStationRecord != null && c.LabStationRecord.Id == labStationId, // Defensive null check
                        c => c.LabStationRecord,
                        c => c.ServiceMembersParent.EventManagement
                    )
                    .Select(c => new
                    {
                        LabStationRecord = c.LabStationRecord,
                        EventId = c.ServiceMembersParent.EventManagement.EventID
                    })
                    .FirstOrDefaultAsync();

                if (result == null || result.LabStationRecord == null)
                {
                    _logger.LogInformation("No LabStationRecord found with Id: {Id}", labStationId);
                    return (null, null);
                }

                return (result.LabStationRecord, result.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching LabStationRecord with Id: {Id}", labStationId);
                throw; // Let controller handle displaying generic error
            }
        }

        public async Task AddAsync(LabStation model, string userName)
        {
            model.AddedOn = DateTime.Now;
            model.AddedBy = userName;

            SetGivenDateTimes(model); // Set Completed date-times

            await _unitOfWork.LabStation.AddAsync(model);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(LabStation model, string userName)
        {
            var existing = await _unitOfWork.LabStation
                .GetWithInclude(x => x.Id == model.Id)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                throw new Exception($"LabStation record with Id={model.Id} not found.");
            }

            // map all fields from model → existing
            MapToEntity(model, existing, userName);
            //SetGivenDateTimes(existing);

            await _unitOfWork.LabStation.UpdateAsync(existing);
            await _unitOfWork.SaveAsync();
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

            target.AreYouFasting = source.AreYouFasting;
            target.AnyComplicationInBloodDrawn = source.AnyComplicationInBloodDrawn;
            target.AllergicToLatex = source.AllergicToLatex;
            target.FeelAlright = source.FeelAlright;
            target.G6pdNeeded = source.G6pdNeeded;
            target.AboNeeded = source.AboNeeded;
            target.LipidPanelNeeded = source.LipidPanelNeeded;
            target.HivNeeded = source.HivNeeded;
            target.PregnancyTestNeeded = source.PregnancyTestNeeded;
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
        }

        public async Task<byte[]> GetLabDataAgainstEventIdAndGenerateHivPdf(string eventId)
        {
            const string methodName = "GetLabDataAgainstEventIdAndGenerateHivPdf";
            _logger.LogInformation("{ClassName}, {MethodName}, Started for EventId={EventId}",
                CLASSNAME, methodName, eventId);

            try
            {
                if (string.IsNullOrWhiteSpace(eventId))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, EventId is null or empty",
                        CLASSNAME, methodName);

                    throw new ArgumentException("EventId is required.");
                }

                // 🔹 Fetch HIV lab data
                var serviceMembersChild = await _fileUploader
                    .GetEventDataByEventIdForLabHivReport(eventId);

                if (serviceMembersChild == null || !serviceMembersChild.Any())
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, No HIV lab records found for EventId={EventId}",
                        CLASSNAME, methodName, eventId);

                    return Array.Empty<byte>();
                }

                // 🔹 Fetch Event details
                var eventDto = await _eventManagementService
                    .GetEventManagementByEventIdWithoutInclude(eventId);

                if (eventDto == null)
                {
                    _logger.LogError("{ClassName}, {MethodName}, EventManagement returned null for EventId={EventId}",
                        CLASSNAME, methodName, eventId);

                    throw new KeyNotFoundException($"Event {eventId} not found.");
                }


                var contractDetail = await _contractService.GetContractById(eventDto.ContractId, string.Empty, false);

                if (!contractDetail.Success)
                {
                    _logger.LogError("{ClassName}, {MethodName}, {Message}",
                        CLASSNAME, methodName, contractDetail.Message);

                    throw new KeyNotFoundException($"{contractDetail.Message}");
                }

                // 🔹 Generate PDF
                var pdfBytes = await _pdfGenerator
                    .GenerateHivSignInSheetPdfAsync(serviceMembersChild, eventDto, contractDetail.Data as ContractDetails);

                _logger.LogInformation("{ClassName}, {MethodName}, PDF generation completed for EventId={EventId}",
                    CLASSNAME, methodName, eventId);

                return pdfBytes;
            }
            catch (KeyNotFoundException)
            {
                // Known, meaningful exception → rethrow
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Unexpected error for EventId={EventId}",
                    CLASSNAME, methodName, eventId);

                throw new ApplicationException(
                    "Failed to generate HIV Sign-In Sheet PDF.", ex);
            }
        }

    }
}

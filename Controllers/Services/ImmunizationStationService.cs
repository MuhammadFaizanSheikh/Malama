using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NPOI.HSSF.Record;
using NPOI.POIFS.Properties;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class ImmunizationStationService : IImmunizationStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IFileUploader _fileUploader;
        private readonly IImmunizationVaccineInfoService _immunizationVaccineInfoService;
        private readonly ILogger<ImmunizationStationService> _logger;
        private const string CLASSNAME = "ImmunizationStationService";

        public ImmunizationStationService(ILogger<ImmunizationStationService> logger, IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IFileUploader fileUploader, IImmunizationVaccineInfoService immunizationVaccineInfoService)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _fileUploader = fileUploader;
            _immunizationVaccineInfoService = immunizationVaccineInfoService;
            _logger = logger;
        }

        public async Task<ImmunizationStation?> GetByIdAsync(long id)
        {
            try
            {
                return await _unitOfWork.ImmunizationStation.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                // optional: log error here
                throw new Exception($"Service error in GetByIdAsync: {ex.Message}", ex);
            }
        }

        public async Task<(ImmunizationStation Immunization, long EventId)> GetImmunizationByIdWithEventIdAsync(long immunizationId)
        {
            const string methodName = "GetImmunizationByIdWithEventIdAsync";

            try
            {
                if (immunizationId <= 0)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, GetImmunizationByIdWithEventIdAsync called with invalid Id: {Id}", immunizationId);
                    return (null, 0);
                }

                _logger.LogDebug("{ClassName}, {MethodName}, Fetching ImmunizationRecord with Id: {Id} and its EventId", immunizationId);

                var result = await _unitOfWork.ServiceMembersChild
                .GetWithInclude(
                    c => c.ImmunizationRecord != null && c.ImmunizationRecord.Id == immunizationId,
                    c => c.ImmunizationRecord,                      // forward navigation
                    c => c.ServiceMembersParent.EventManagement
                )
                .Include(c => c.ImmunizationRecord)               // ensure tracking
                    .ThenInclude(ir => ir.ServiceMembersChild)   // include back navigation
                .Select(c => new
                {
                    Immunization = c.ImmunizationRecord,
                    EventId = c.ServiceMembersParent.EventManagement.Id
                })
                .FirstOrDefaultAsync();

                if (result == null || result.Immunization == null)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, No ImmunizationRecord found with Id: {Id}", immunizationId);
                    return (null, 0);
                }

                return (result.Immunization, result.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error fetching ImmunizationRecord with Id: {Id}", immunizationId);
                throw; // Let controller handle displaying generic error
            }
        }

        public async Task<ResponseDto> GetImmunizationManufacturer(long eventId)
        {
            return await _immunizationVaccineInfoService.GetManufacturerByEventIdAsync(eventId);
        }

        public async Task AddAsync(ImmunizationStation model, string userName)
        {
            model.AddedOn = DateTime.Now;
            model.AddedBy = userName;

            SetGivenDateTimes(model);

            await _unitOfWork.ImmunizationStation.AddAsync(model);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ImmunizationStation model, string userName)
        {
            var existing = await _unitOfWork.ImmunizationStation.GetByIdAsync(model.Id);

            if (existing == null)
            {
                throw new Exception($"Immunization record with Id={model.Id} not found.");
            }

            // map all fields from model → existing
            MapToEntity(model, existing, userName);

            await _unitOfWork.ImmunizationStation.UpdateAsync(existing);
            await _unitOfWork.SaveAsync();
        }

        private void MapToEntity(ImmunizationStation source, ImmunizationStation target, string userName)
        {
            // ========== Questions ==========
            target.IsSickToday = source.IsSickToday;
            target.IsSickTodayReason = source.IsSickTodayReason;
            target.HasAllergiesToMedicationFoodVaccineOrLatex = source.HasAllergiesToMedicationFoodVaccineOrLatex;
            target.HasAllergiesReason = source.HasAllergiesReason;
            target.HadSeriousReactionAfterVaccination = source.HadSeriousReactionAfterVaccination;
            target.SeriousReactionReason = source.SeriousReactionReason;
            target.HasLongTermHealthProblem = source.HasLongTermHealthProblem;
            target.LongTermHealthProblemReason = source.LongTermHealthProblemReason;
            target.HasCancerOrImmuneSystemProblem = source.HasCancerOrImmuneSystemProblem;
            target.CancerOrImmuneSystemReason = source.CancerOrImmuneSystemReason;
            target.TookImmuneSuppressingMedicationRecently = source.TookImmuneSuppressingMedicationRecently;
            target.ImmuneSuppressionReason = source.ImmuneSuppressionReason;
            target.HadSeizureOrNervousSystemProblem = source.HadSeizureOrNervousSystemProblem;
            target.SeizureReason = source.SeizureReason;
            target.HadBloodTransfusionOrAntiviralInPastYear = source.HadBloodTransfusionOrAntiviralInPastYear;
            target.BloodTransfusionReason = source.BloodTransfusionReason;
            target.IsPregnantOrCouldBePregnant = source.IsPregnantOrCouldBePregnant;
            target.PregnancyCheckboxSelected = source.PregnancyCheckboxSelected;
            target.ReceivedVaccineInPast4Weeks = source.ReceivedVaccineInPast4Weeks;
            target.ReceivedVaccineReason = source.ReceivedVaccineReason;

            // ========== Hepatitis B ==========
            if (source.HepBNeeded.IsNullOrEmpty())
            {
                target.HepBGivenDateTime = null;
                target.HepBReason = null;
                target.HepBReasonExcusedComments = null;
                target.HepBVaccineInfoId = null;
                target.HepBVaccineLotEntryId = null;
                target.HepBExpirationDate = null;
                target.HepBType = null;
                target.HepBBodyPart = null;
                target.HepBBodyPartOther = null;
                target.HepBSite = null;
                target.HepBStaffName = null;
            }
            else if (source.HepBNeeded == "Not Completed")
            {
                target.HepBGivenDateTime = null;
                target.HepBVaccineInfoId = null;
                target.HepBVaccineLotEntryId = null;
                target.HepBExpirationDate = null;
                target.HepBType = null;
                target.HepBBodyPart = null;
                target.HepBBodyPartOther = null;
                target.HepBSite = null;
                target.HepBStaffName = null;
                target.HepBReason = source.HepBReason;
                target.HepBReasonExcusedComments = source.HepBReasonExcusedComments;
            }
            else
            {
                if (source.HepBNeeded != target.HepBNeeded)
                {
                    target.HepBGivenDateTime = DateTime.Now;
                }

                target.HepBReason = null;
                target.HepBReasonExcusedComments = null;
                target.HepBVaccineInfoId = source.HepBVaccineInfoId;
                target.HepBVaccineLotEntryId = source.HepBVaccineLotEntryId;
                target.HepBExpirationDate = source.HepBExpirationDate;
                target.HepBType = source.HepBType;
                target.HepBBodyPart = source.HepBBodyPart;
                target.HepBBodyPartOther = source.HepBBodyPartOther;
                target.HepBSite = source.HepBSite;
                target.HepBStaffName = source.HepBStaffName;
            }

            // ========== Influenza ==========

            if (source.FluNeeded.IsNullOrEmpty())
            {
                target.FluGivenDateTime = null;
                target.FluReason = null;
                target.FluReasonExcusedComments = null;
                target.FluVaccineInfoId = null;
                target.FluVaccineLotEntryId = null;
                target.FluExpirationDate = null;
                target.FluType = null;
                target.FluBodyPart = null;
                target.FluBodyPartOther = null;
                target.FluSite = null;
                target.FluStaffName = null;
            }
            else if (source.FluNeeded == "Not Completed")
            {
                target.FluGivenDateTime = null;
                target.FluVaccineInfoId = null;
                target.FluVaccineLotEntryId = null;
                target.FluExpirationDate = null;
                target.FluType = null;
                target.FluBodyPart = null;
                target.FluBodyPartOther = null;
                target.FluSite = null;
                target.FluStaffName = null;
                target.FluReason = source.FluReason;
                target.FluReasonExcusedComments = source.FluReasonExcusedComments;
            }
            else
            {
                if (source.FluNeeded != target.FluNeeded)
                {
                    target.FluGivenDateTime = DateTime.Now;
                }

                target.FluReason = null;
                target.FluReasonExcusedComments = null;
                target.FluVaccineInfoId = source.FluVaccineInfoId;
                target.FluVaccineLotEntryId = source.FluVaccineLotEntryId;
                target.FluExpirationDate = source.FluExpirationDate;
                target.FluType = source.FluType;
                target.FluBodyPart = source.FluBodyPart;
                target.FluBodyPartOther = source.FluBodyPartOther;
                target.FluSite = source.FluSite;
                target.FluStaffName = source.FluStaffName;
            }

            // ========== MMR ==========

            if (source.MMRNeeded.IsNullOrEmpty())
            {
                target.MMRGivenDateTime = null;
                target.MMRReason = null;
                target.MMRReasonExcusedComments = null;
                target.MMRVaccineInfoId = null;
                target.MMRVaccineLotEntryId = null;
                target.MMRExpirationDate = null;
                target.MMRType = null;
                target.MMRBodyPart = null;
                target.MMRBodyPartOther = null;
                target.MMRSite = null;
                target.MMRStaffName = null;
            }
            else if (source.MMRNeeded == "Not Completed")
            {
                target.MMRGivenDateTime = null;
                target.MMRVaccineInfoId = null;
                target.MMRVaccineLotEntryId = null;
                target.MMRExpirationDate = null;
                target.MMRType = null;
                target.MMRBodyPart = null;
                target.MMRBodyPartOther = null;
                target.MMRSite = null;
                target.MMRStaffName = null;
                target.MMRReason = source.MMRReason;
                target.MMRReasonExcusedComments = source.MMRReasonExcusedComments;
            }
            else
            {
                if (source.MMRNeeded != target.MMRNeeded)
                {
                    target.MMRGivenDateTime = DateTime.Now;
                }

                target.MMRReason = null;
                target.MMRReasonExcusedComments = null;
                target.MMRVaccineInfoId = source.MMRVaccineInfoId;
                target.MMRVaccineLotEntryId = source.MMRVaccineLotEntryId;
                target.MMRExpirationDate = source.MMRExpirationDate;
                target.MMRType = source.MMRType;
                target.MMRBodyPart = source.MMRBodyPart;
                target.MMRBodyPartOther = source.MMRBodyPartOther;
                target.MMRSite = source.MMRSite;
                target.MMRStaffName = source.MMRStaffName;
            }

            

            // ========== Hepatitis A ==========

            if (source.HepANeeded.IsNullOrEmpty())
            {
                target.HepAGivenDateTime = null;
                target.HepAReason = null;
                target.HepAReasonExcusedComments = null;
                target.HepAVaccineInfoId = null;
                target.HepAVaccineLotEntryId = null;
                target.HepAExpirationDate = null;
                target.HepAType = null;
                target.HepABodyPart = null;
                target.HepABodyPartOther = null;
                target.HepASite = null;
                target.HepAStaffName = null;
            }
            else if (source.HepANeeded == "Not Completed")
            {
                target.HepAGivenDateTime = null;
                target.HepAVaccineInfoId = null;
                target.HepAVaccineLotEntryId = null;
                target.HepAExpirationDate = null;
                target.HepAType = null;
                target.HepABodyPart = null;
                target.HepABodyPartOther = null;
                target.HepASite = null;
                target.HepAStaffName = null;
                target.HepAReason = source.HepAReason;
                target.HepAReasonExcusedComments = source.HepAReasonExcusedComments;
            }
            else
            {
                if (source.HepANeeded != target.HepANeeded)
                {
                    target.HepAGivenDateTime = DateTime.Now;
                }

                target.HepAReason = null;
                target.HepAReasonExcusedComments = null;
                target.HepAVaccineInfoId = source.HepAVaccineInfoId;
                target.HepAVaccineLotEntryId = source.HepAVaccineLotEntryId;
                target.HepAExpirationDate = source.HepAExpirationDate;
                target.HepAType = source.HepAType;
                target.HepABodyPart = source.HepABodyPart;
                target.HepABodyPartOther = source.HepABodyPartOther;
                target.HepASite = source.HepASite;
                target.HepAStaffName = source.HepAStaffName;
            }

            // ========== Tetanus / Tdap ==========

            if (source.TetTdpNeeded.IsNullOrEmpty())
            {
                target.TetTdpGivenDateTime = null;
                target.TetTdpReason = null;
                target.TetTdpReasonExcusedComments = null;
                target.TetTdpVaccineInfoId = null;
                target.TetTdpVaccineLotEntryId = null;
                target.TetTdpExpirationDate = null;
                target.TetTdpType = null;
                target.TetTdpBodyPart = null;
                target.TetTdpBodyPartOther = null;
                target.TetTdpSite = null;
                target.TetTdpStaffName = null;
            }
            else if (source.TetTdpNeeded == "Not Completed")
            {
                target.TetTdpGivenDateTime = null;
                target.TetTdpVaccineInfoId = null;
                target.TetTdpVaccineLotEntryId = null;
                target.TetTdpExpirationDate = null;
                target.TetTdpType = null;
                target.TetTdpBodyPart = null;
                target.TetTdpBodyPartOther = null;
                target.TetTdpSite = null;
                target.TetTdpStaffName = null;
                target.TetTdpReason = source.TetTdpReason;
                target.TetTdpReasonExcusedComments = source.TetTdpReasonExcusedComments;
            }
            else
            {
                if (source.TetTdpNeeded != target.TetTdpNeeded)
                {
                    target.TetTdpGivenDateTime = DateTime.Now;
                }

                target.TetTdpReason = null;
                target.TetTdpReasonExcusedComments = null;
                target.TetTdpVaccineInfoId = source.TetTdpVaccineInfoId;
                target.TetTdpVaccineLotEntryId = source.TetTdpVaccineLotEntryId;
                target.TetTdpExpirationDate = source.TetTdpExpirationDate;
                target.TetTdpType = source.TetTdpType;
                target.TetTdpBodyPart = source.TetTdpBodyPart;
                target.TetTdpBodyPartOther = source.TetTdpBodyPartOther;
                target.TetTdpSite = source.TetTdpSite;
                target.TetTdpStaffName = source.TetTdpStaffName;
            }
            
            // ========== Varicella ==========

            if (source.VaricellaNeeded.IsNullOrEmpty())
            {
                target.VaricellaGivenDateTime = null;
                target.VaricellaReason = null;
                target.VaricellaReasonExcusedComments = null;
                target.VaricellaVaccineInfoId = null;
                target.VaricellaVaccineLotEntryId = null;
                target.VaricellaExpirationDate = null;
                target.VaricellaType = null;
                target.VaricellaBodyPart = null;
                target.VaricellaBodyPartOther = null;
                target.VaricellaSite = null;
                target.VaricellaStaffName = null;
            }
            else if (source.VaricellaNeeded == "Not Completed")
            {
                target.VaricellaGivenDateTime = null;
                target.VaricellaVaccineInfoId = null;
                target.VaricellaVaccineLotEntryId = null;
                target.VaricellaExpirationDate = null;
                target.VaricellaType = null;
                target.VaricellaBodyPart = null;
                target.VaricellaBodyPartOther = null;
                target.VaricellaSite = null;
                target.VaricellaStaffName = null;
                target.VaricellaReason = source.VaricellaReason;
                target.VaricellaReasonExcusedComments = source.VaricellaReasonExcusedComments;
            }
            else
            {
                if (source.VaricellaNeeded != target.VaricellaNeeded)
                {
                    target.VaricellaGivenDateTime = DateTime.Now;
                }

                target.VaricellaReason = null;
                target.VaricellaReasonExcusedComments = null;
                target.VaricellaVaccineInfoId = source.VaricellaVaccineInfoId;
                target.VaricellaVaccineLotEntryId = source.VaricellaVaccineLotEntryId;
                target.VaricellaExpirationDate = source.VaricellaExpirationDate;
                target.VaricellaType = source.VaricellaType;
                target.VaricellaBodyPart = source.VaricellaBodyPart;
                target.VaricellaBodyPartOther = source.VaricellaBodyPartOther;
                target.VaricellaSite = source.VaricellaSite;
                target.VaricellaStaffName = source.VaricellaStaffName;
            }

            target.HepBNeeded = source.HepBNeeded;
            target.FluNeeded = source.FluNeeded;
            target.MMRNeeded = source.MMRNeeded;
            target.HepANeeded = source.HepANeeded;
            target.TetTdpNeeded = source.TetTdpNeeded;
            target.VaricellaNeeded = source.VaricellaNeeded;

            // ========== Metadata ==========
            target.Status = source.Status;
            target.UpdatedOn = DateTime.Now;
            target.UpdatedBy = userName;
        }

        public void SetGivenDateTimes(ImmunizationStation model)
        {
            if (model == null) return;

            model.FluGivenDateTime = model.FluNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.HepBGivenDateTime = model.HepBNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.HepAGivenDateTime = model.HepANeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.MMRGivenDateTime = model.MMRNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.TetTdpGivenDateTime = model.TetTdpNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
            model.VaricellaGivenDateTime = model.VaricellaNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
        }
    }
}

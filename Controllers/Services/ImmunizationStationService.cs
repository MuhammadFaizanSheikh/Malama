using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class ImmunizationStationService : IImmunizationStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IFileUploader _fileUploader;
        private readonly IImmunizationVaccineInfoService _immunizationVaccineInfoService;

        public ImmunizationStationService(IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IFileUploader fileUploader, IImmunizationVaccineInfoService immunizationVaccineInfoService)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _fileUploader = fileUploader;
            _immunizationVaccineInfoService = immunizationVaccineInfoService;
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

        public async Task<ImmunizationStation> GetByIdWithParentAsync(long id)
        {
            return await _unitOfWork.ImmunizationStation.GetWithInclude(x => x.Id == id, x => x.FileData).FirstOrDefaultAsync();
        }

        public async Task<ResponseDto> GetImmunizationManufacturer(string eventId)
        {
            return await _immunizationVaccineInfoService.GetManufacturerByEventIdAsync(eventId);
        }

        public async Task AddAsync(ImmunizationStation model, string userName)
        {
            model.AddedOn = DateTime.Now;
            model.AddedBy = userName;
            model.FluGivenDateTime = DateTime.Now;
            model.HepBGivenDateTime = DateTime.Now;
            model.HepAGivenDateTime = DateTime.Now;
            model.MMRGivenDateTime = DateTime.Now;
            model.TetTdpGivenDateTime = DateTime.Now;
            model.VaricellaGivenDateTime = DateTime.Now;


            await _unitOfWork.ImmunizationStation.AddAsync(model);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ImmunizationStation model, string userName)
        {
            var existing = await _unitOfWork.ImmunizationStation
                .GetWithInclude(x => x.Id == model.Id, x => x.FileData)
                .FirstOrDefaultAsync();

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
            target.HepBNeeded = source.HepBNeeded;
            target.HepBReason = source.HepBReason;
            target.HepBManufacturer = source.HepBManufacturer;
            target.HepBLotNo = source.HepBLotNo;
            target.HepBExpirationDate = source.HepBExpirationDate;
            target.HepBType = source.HepBType;
            target.HepBBodyPart = source.HepBBodyPart;
            target.HepBBodyPartOther = source.HepBBodyPartOther;
            target.HepBSite = source.HepBSite;
            target.HepBStaffName = source.HepBStaffName;

            // ========== Influenza ==========
            target.FluNeeded = source.FluNeeded;
            target.FluReason = source.FluReason;
            target.FluManufacturer = source.FluManufacturer;
            target.FluLotNo = source.FluLotNo;
            target.FluExpirationDate = source.FluExpirationDate;
            target.FluType = source.FluType;
            target.FluBodyPart = source.FluBodyPart;
            target.FluBodyPartOther = source.FluBodyPartOther;
            target.FluSite = source.FluSite;
            target.FluStaffName = source.FluStaffName;

            // ========== MMR ==========
            target.MMRNeeded = source.MMRNeeded;
            target.MMRReason = source.MMRReason;
            target.MMRManufacturer = source.MMRManufacturer;
            target.MMRLotNo = source.MMRLotNo;
            target.MMRExpirationDate = source.MMRExpirationDate;
            target.MMRType = source.MMRType;
            target.MMRBodyPart = source.MMRBodyPart;
            target.MMRBodyPartOther = source.MMRBodyPartOther;
            target.MMRSite = source.MMRSite;
            target.MMRStaffName = source.MMRStaffName;

            // ========== Hepatitis A ==========
            target.HepANeeded = source.HepANeeded;
            target.HepAReason = source.HepAReason;
            target.HepAManufacturer = source.HepAManufacturer;
            target.HepALotNo = source.HepALotNo;
            target.HepAExpirationDate = source.HepAExpirationDate;
            target.HepAType = source.HepAType;
            target.HepABodyPart = source.HepABodyPart;
            target.HepABodyPartOther = source.HepABodyPartOther;
            target.HepASite = source.HepASite;
            target.HepAStaffName = source.HepAStaffName;

            // ========== Tetanus / Tdap ==========
            target.TetTdpNeeded = source.TetTdpNeeded;
            target.TetTdpReason = source.TetTdpReason;
            target.TetTdpManufacturer = source.TetTdpManufacturer;
            target.TetTdpLotNo = source.TetTdpLotNo;
            target.TetTdpExpirationDate = source.TetTdpExpirationDate;
            target.TetTdpType = source.TetTdpType;
            target.TetTdpBodyPart = source.TetTdpBodyPart;
            target.TetTdpBodyPartOther = source.TetTdpBodyPartOther;
            target.TetTdpSite = source.TetTdpSite;
            target.TetTdpStaffName = source.TetTdpStaffName;

            // ========== Varicella ==========
            target.VaricellaNeeded = source.VaricellaNeeded;
            target.VaricellaReason = source.VaricellaReason;
            target.VaricellaManufacturer = source.VaricellaManufacturer;
            target.VaricellaLotNo = source.VaricellaLotNo;
            target.VaricellaExpirationDate = source.VaricellaExpirationDate;
            target.VaricellaType = source.VaricellaType;
            target.VaricellaBodyPart = source.VaricellaBodyPart;
            target.VaricellaBodyPartOther = source.VaricellaBodyPartOther;
            target.VaricellaSite = source.VaricellaSite;
            target.VaricellaStaffName = source.VaricellaStaffName;

            // ========== Metadata ==========
            target.Status = source.Status;
            target.UpdatedOn = DateTime.Now;
            target.UpdatedBy = userName;
        }


    }
}

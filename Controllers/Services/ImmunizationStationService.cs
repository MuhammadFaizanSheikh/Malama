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

        public ImmunizationStationService(IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IFileUploader fileUploader)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _fileUploader = fileUploader;
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

        public async Task AddAsync(ImmunizationStation model)
        {
            // Ensure parent exists
            //var parent = await _fileUploader.GetByIdAsync(model.FileDataId);
            //if (parent == null)
            //{
            //    throw new Exception($"Parent FileData with Id={model.FileDataId} not found.");
            //}

            //model.FileData = parent;
            model.CompletedOn = DateTime.SpecifyKind(model.CompletedOn ?? DateTime.Now, DateTimeKind.Unspecified);


            await _unitOfWork.ImmunizationStation.AddAsync(model);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ImmunizationStation model)
        {
            var existing = await _unitOfWork.ImmunizationStation
                .GetWithInclude(x => x.Id == model.Id, x => x.FileData)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                throw new Exception($"Immunization record with Id={model.Id} not found.");
            }

            // map all fields from model → existing
            MapToEntity(model, existing);

            await _unitOfWork.ImmunizationStation.UpdateAsync(existing);
            await _unitOfWork.SaveAsync();
        }

        private void MapToEntity(ImmunizationStation source, ImmunizationStation target)
        {
            // Questions
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

            // Vaccines
            target.HepBNeeded = source.HepBNeeded;
            target.HepBReason = source.HepBReason;
            target.HepBManufacturer = source.HepBManufacturer;
            target.HepBLotNo = source.HepBLotNo;
            target.HepBExpirationDate = source.HepBExpirationDate;

            target.FluNeeded = source.FluNeeded;
            target.FluReason = source.FluReason;
            target.FluManufacturer = source.FluManufacturer;
            target.FluLotNo = source.FluLotNo;
            target.FluExpirationDate = source.FluExpirationDate;

            target.MMRNeeded = source.MMRNeeded;
            target.MMRReason = source.MMRReason;
            target.MMRManufacturer = source.MMRManufacturer;
            target.MMRLotNo = source.MMRLotNo;
            target.MMRExpirationDate = source.MMRExpirationDate;

            target.HepANeeded = source.HepANeeded;
            target.HepAReason = source.HepAReason;
            target.HepAManufacturer = source.HepAManufacturer;
            target.HepALotNo = source.HepALotNo;
            target.HepAExpirationDate = source.HepAExpirationDate;

            target.TetTdpNeeded = source.TetTdpNeeded;
            target.TetTdpReason = source.TetTdpReason;
            target.TetTdpManufacturer = source.TetTdpManufacturer;
            target.TetTdpLotNo = source.TetTdpLotNo;
            target.TetTdpExpirationDate = source.TetTdpExpirationDate;

            target.VaricellaNeeded = source.VaricellaNeeded;
            target.VaricellaReason = source.VaricellaReason;
            target.VaricellaManufacturer = source.VaricellaManufacturer;
            target.VaricellaLotNo = source.VaricellaLotNo;
            target.VaricellaExpirationDate = source.VaricellaExpirationDate;

            // Metadata
            target.Status = source.Status;
            target.CompletedOn = DateTime.SpecifyKind(source.CompletedOn ?? DateTime.Now, DateTimeKind.Unspecified);
            target.CompletedBy = source.CompletedBy;
        }

    }
}

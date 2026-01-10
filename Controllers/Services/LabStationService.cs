using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class LabStationService : ILabStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IFileUploader _fileUploader;

        public LabStationService(IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IFileUploader fileUploader)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _fileUploader = fileUploader;
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

        public async Task<LabStation> GetByIdWithParentAsync(long id)
        {
            return await _unitOfWork.LabStation.GetWithInclude(x => x.Id == id, x => x.FileData).FirstOrDefaultAsync();
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
                .GetWithInclude(x => x.Id == model.Id, x => x.FileData)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                throw new Exception($"LabStation record with Id={model.Id} not found.");
            }

            // map all fields from model → existing
            MapToEntity(model, existing, userName);
            SetGivenDateTimes(existing);

            await _unitOfWork.LabStation.UpdateAsync(existing);
            await _unitOfWork.SaveAsync();
        }

        private void MapToEntity(LabStation source, LabStation target, string userName)
        {
            target.AreYouFasting = source.AreYouFasting;
            target.G6pdNeeded = source.G6pdNeeded;
            target.G6pdReason = source.G6pdReason;
            target.AboNeeded = source.AboNeeded;
            target.AboReason = source.AboReason;
            target.AboGrouping = source.AboGrouping;
            target.AboRhFactor = source.AboRhFactor;
            target.LipidPanelNeeded = source.LipidPanelNeeded;
            target.LipidPanelReason = source.LipidPanelReason;
            target.LipidPanelRapidTesting = source.LipidPanelRapidTesting;
            target.TotalCholesterol = source.TotalCholesterol;
            target.HdlCholesterol = source.HdlCholesterol;
            target.Triglycerides = source.Triglycerides;
            target.Glucose = source.Glucose;
            target.LdlCholesterol = source.LdlCholesterol;
            target.TotalCholesterolHdlRatio = source.TotalCholesterolHdlRatio;
            target.NonHdlCholesterol = source.NonHdlCholesterol;
            target.HivNeeded = source.HivNeeded;
            target.HivReason = source.HivReason;
            target.HivBarcodeCarebill = source.HivBarcodeCarebill;
            target.PregnancyTestNeeded = source.PregnancyTestNeeded;
            target.PregnancyTestResult = source.PregnancyTestResult;
            target.PregnancyTestReason = source.PregnancyTestReason;
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

    }
}

using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class ImmunizationVaccineInfoService : IImmunizationVaccineInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IFileUploader _fileUploader;

        public ImmunizationVaccineInfoService(IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IFileUploader fileUploader)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _fileUploader = fileUploader;
        }

        public async Task<List<ImmunizationVaccineInfoForPreview>> GetVaccineEntriesByEventIdAsync(string eventId)
        {
            try
            {
                if (string.IsNullOrEmpty(eventId))
                    throw new ArgumentException("EventId cannot be null or empty.", nameof(eventId));

                var records = _unitOfWork.ImmunizationVaccineInfo.GetWithInclude(f => f.EventId == eventId).Include(x => x.Lots);

                return records.Select(x => new ImmunizationVaccineInfoForPreview
                {
                    Id = x.Id,
                    Vaccine = x.Vaccine,
                    Manufacturer = x.Manufacturer,
                    StartingDoses = x.StartingDoses,
                    FinalDoses = x.FinalDoses,
                    LotNumber = string.Join("<br>", x.Lots.Select(l => l.LotNumber)),
                    Expiration = string.Join("<br>",
                x.Lots.Select(l => l.Expiration.ToString("MM-dd-yyyy")))
                }).ToList();
            }
            catch (ArgumentException argEx)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to fetch vaccine records.", ex);
            }
        }

        public async Task<ResponseDto> AddInventoryAsync(ImmunizationVaccineInfo immunizationVaccine, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                immunizationVaccine.AddedBy = loggedinUserName;
                immunizationVaccine.AddedOn = DateTime.Now;
                await _unitOfWork.ImmunizationVaccineInfo.AddAsync(immunizationVaccine);
                responseDto.Success = true;
                responseDto.Message = "Immunization vaccine inventory added successfully!";
            }
            catch (Exception ex)
            {
                // If an exception occurs, set Success to false and provide the error message
                responseDto.Success = false;
                responseDto.Message = $"An error occurred while adding Immunization vaccine inventory: {ex.Message}";
            }

            return responseDto;
        }

        //public async Task<ResponseDto> UpdateInventoryAsync(ImmunizationVaccineInfo immunizationVaccine, string loggedinUserName)
        //{
        //    var responseDto = new ResponseDto();

        //    try
        //    {
        //        immunizationVaccine.UpdatedBy = loggedinUserName;
        //        immunizationVaccine.UpdatedOn = DateTime.Now;
        //        await _unitOfWork.ImmunizationVaccineInfo.UpdateAsync(immunizationVaccine);
        //        responseDto.Success = true;
        //        responseDto.Message = "Immunization vaccine inventory updated successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        responseDto.Success = false;
        //        responseDto.Message = $"An error occurred while updating Immunization vaccine inventory: {ex.Message}";
        //    }

        //    return responseDto;
        //}

        public async Task<ResponseDto> UpdateInventoryAsync(ImmunizationVaccineInfo immunizationVaccine, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                // 1️⃣ Get existing record from DB (including its children)
                
                var existingRecord = await _unitOfWork.ImmunizationVaccineInfo.GetByIdAsync(immunizationVaccine.Id);
                
                if (existingRecord == null)
                {
                    responseDto.Success = false;
                    responseDto.Message = "Record not found.";
                    return responseDto;
                }

                // 2️⃣ Update parent fields
                existingRecord.ImmunizationType = immunizationVaccine.ImmunizationType;
                existingRecord.Vaccine = immunizationVaccine.Vaccine;
                existingRecord.Manufacturer = immunizationVaccine.Manufacturer;
                existingRecord.EventDate = immunizationVaccine.EventDate;
                existingRecord.StartingDoses = immunizationVaccine.StartingDoses;
                existingRecord.FinalDoses = immunizationVaccine.FinalDoses;
                existingRecord.UpdatedBy = loggedinUserName;
                existingRecord.UpdatedOn = DateTime.Now;
                await _unitOfWork.ImmunizationVaccineInfo.UpdateAsync(existingRecord);
                // 3️⃣ Sync child collection (Lots)
                // Remove deleted ones
                var lotsToRemove = existingRecord.Lots
                    .Where(existing => !immunizationVaccine.Lots.Any(newLot => newLot.Id == existing.Id))
                    .ToList();

                await _unitOfWork.ImmunizationVaccineLotEntry.DeleteAgainstFieldAsync(immunizationVaccine.Id, "ImmunizationVaccineInfoId");

                foreach (var entry in immunizationVaccine.Lots)
                {
                    entry.ImmunizationVaccineInfoId = immunizationVaccine.Id;
                }

                _unitOfWork.ImmunizationVaccineLotEntry.AddRange(immunizationVaccine.Lots);
                
                // 4️⃣ Save changes
                await _unitOfWork.SaveAsync();

                responseDto.Success = true;
                responseDto.Message = "Immunization vaccine inventory updated successfully!";
            }
            catch (Exception ex)
            {
                responseDto.Success = false;
                responseDto.Message = $"An error occurred while updating Immunization vaccine inventory: {ex.Message}";
            }

            return responseDto;
        }


        public async Task<ResponseDto> GetImmunizationVaccineInfoByIdAsync(long immunizationId)
        {
            var response = new ResponseDto();

            try
            {
                var vaccineInfo = await _unitOfWork.ImmunizationVaccineInfo.GetWithInclude(x => x.Id == immunizationId, x => x.Lots).FirstOrDefaultAsync();

                if (vaccineInfo == null)
                {
                    response.Success = false;
                    response.Message = "Immunization vaccine record not found.";
                    return response;
                }

                response.Success = true;
                response.Message = "Record fetched successfully.";
                response.Data = vaccineInfo;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error occurred in GetImmunizationVaccineInfoByIdAsync");
                response.Success = false;
                response.Message = $"An error occurred while fetching the immunization vaccine record: {ex.Message}";
            }

            return response;
        }


    }
}

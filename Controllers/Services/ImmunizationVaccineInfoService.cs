using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class ImmunizationVaccineInfoService : IImmunizationVaccineInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubmissionTokenService _submissionTokenService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IFileUploader _fileUploader;
        private readonly IContainerMonitoringService _containerMonitoringService;

        public ImmunizationVaccineInfoService(IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, ISubmissionTokenService submissionTokenService, IFileUploader fileUploader, IContainerMonitoringService containerMonitoringService)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _fileUploader = fileUploader;
            _containerMonitoringService = containerMonitoringService;
            _submissionTokenService = submissionTokenService;
        }

        public async Task<List<ImmunizationVaccineInfoForPreview>> GetVaccineEntriesByEventIdAsync(long eventId)
        {
            try
            {
                //if (string.IsNullOrEmpty(eventId))
                //    throw new ArgumentException("EventId cannot be null or empty.", nameof(eventId));

                var records = _unitOfWork.ImmunizationVaccineInfo.GetWithInclude(f => f.EventManagementId == eventId).Include(x => x.Lots).ThenInclude(l => l.Container);

                return records.Select(x => new ImmunizationVaccineInfoForPreview
                {
                    Id = x.Id,
                    Vaccine = x.Vaccine,
                    Manufacturer = x.Manufacturer,
                    StartingDoses = x.StartingDoses,
                    FinalDoses = x.FinalDoses,
                    LotNumber = string.Join("<br>", x.Lots.Select(l => l.LotNumber)),
                    Expiration = string.Join("<br>",
                x.Lots.Select(l => l.Expiration.ToString("MM-dd-yyyy"))),
                    ImmunizationType = x.ImmunizationType,
                    AddedBy = x.AddedBy,
                    AddedOn = x.AddedOn,
                    ContainerName = string.Join(", ",x.Lots.Where(l => l.Container != null).Select(l => l.Container.ContainerName)) ?? string.Empty
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

        public async Task<ResponseDto> AddInventoryAsync(ImmunizationVaccineInfo immunizationVaccine, string SubmissionToken, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                var tokenResult = await _submissionTokenService.ValidateAndSaveAsync(SubmissionToken, loggedinUserName);

                if (!tokenResult.Success)
                {
                    return tokenResult;
                }

                var records = _unitOfWork.ImmunizationVaccineInfo.FindForSearching(f => f.EventManagementId == immunizationVaccine.EventManagementId && f.ImmunizationType == immunizationVaccine.ImmunizationType && f.Vaccine == immunizationVaccine.Vaccine && f.Dose == immunizationVaccine.Dose);

                if (records != null && records.Any())
                {
                    responseDto.Success = false;
                    responseDto.Message = "This vaccine is already present in inventory, Please add different vaccine!";
                    return responseDto;
                }

                if (immunizationVaccine.FinalDoses > immunizationVaccine.StartingDoses)
                {
                    responseDto.Success = false;
                    responseDto.Message = "Final dose cannot be greater than starting dose!";
                    return responseDto;
                }

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

        public async Task<ResponseDto> UpdateInventoryAsync(ImmunizationVaccineInfo immunizationVaccine, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                var records = _unitOfWork.ImmunizationVaccineInfo.FindForSearching(f =>
                f.EventManagementId == immunizationVaccine.EventManagementId &&
                f.ImmunizationType == immunizationVaccine.ImmunizationType &&
                f.Vaccine == immunizationVaccine.Vaccine &&
                f.Dose == immunizationVaccine.Dose &&
                f.Id != immunizationVaccine.Id // 👈 Exclude the record being updated
                );

                if (records != null && records.Any())
                {
                    responseDto.Success = false;
                    responseDto.Message = "This vaccine is already present in inventory, Please add different vaccine!";
                    return responseDto;
                }

                if (immunizationVaccine.FinalDoses > immunizationVaccine.StartingDoses)
                {
                    responseDto.Success = false;
                    responseDto.Message = "Final dose cannot be greater than starting dose!";
                    return responseDto;
                }

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
                existingRecord.Dose = immunizationVaccine.Dose;
                existingRecord.Unit = immunizationVaccine.Unit;
                await _unitOfWork.ImmunizationVaccineInfo.UpdateAsync(existingRecord);

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

        public async Task<ResponseDto> GetContainersByEventIdAsync(long eventId)
        {
            var response = new ResponseDto();

            try
            {
                if (eventId <= 0)
                {
                    response.Success = false;
                    response.Message = "Event ID cannot be null or empty.";
                    return response;
                }

                var containers = await _containerMonitoringService.GetOnlyContainersByEventIdAsync(eventId);

                if (containers == null || !containers.Any())
                {
                    response.Success = false;
                    response.Message = "No containers found for this event.";
                    return response;
                }

                response.Success = true;
                response.Message = "Containers fetched successfully.";
                response.Data = containers;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error fetching containers for EventId {eventId}", eventId);
                response.Success = false;
                response.Message = $"An unexpected error occurred while fetching containers: {ex.Message}";
                response.Data = null;
            }

            return response;
        }

        public async Task<ResponseDto> GetManufacturerByEventIdAsync(long eventId)
        {
            var response = new ResponseDto();

            try
            {
                if (eventId <= 0)
                {
                    response.Success = false;
                    response.Message = "Event ID cannot be null or empty.";
                    return response;
                }

                var vaccineInfo = _unitOfWork.ImmunizationVaccineInfo.GetWithInclude(x => x.EventManagementId == eventId, x => x.Lots);
                
                if (vaccineInfo == null || !vaccineInfo.Any())
                {
                    response.Success = false;
                    response.Message = "No immunization found.";
                    return response;
                }

                var immunizationData = vaccineInfo.Select(v => new
                {
                    v.Id,
                    v.EventManagementId,
                    v.ImmunizationType,
                    v.Vaccine,
                    v.Manufacturer,
                    v.Dose,
                    v.Unit,
                    Lots = v.Lots.Select(l => new
                    {
                        l.Id,
                        l.LotNumber,
                        l.Expiration,
                    }).ToList()
                }).ToList();

                response.Success = true;
                response.Message = "Immunization found.";
                response.Data = immunizationData;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error fetching containers for EventId {eventId}", eventId);
                response.Success = false;
                response.Message = $"An unexpected error occurred while fetching immunizations: {ex.Message}";
                response.Data = null;
            }

            return response;
        }

    }
}

using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.UnitOfWork;
using ExcelToCsv.Models;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class EventManagementService : IEventManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EventManagement>> GetAllEventManagements()
        {
            var responseDto = new ResponseDto();
            List<EventManagement> eventManagements = new List<EventManagement>();

            try
            {
                eventManagements = (await _unitOfWork.EventManagement.GetAllAsync()).OrderByDescending(c => c.Id).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

            return eventManagements;
        }

        public async Task<ResponseDto> AddEventManagementAsync(EventManagement eventManagement, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                eventManagement.AddedBy = loggedinUserName;
                eventManagement.AddedOn = DateTime.Now;
                await _unitOfWork.EventManagement.AddAsync(eventManagement);

                responseDto.Success = true;
                responseDto.Message = "Event Management added successfully!";
            }
            catch (Exception ex)
            {
                // If an exception occurs, set Success to false and provide the error message
                responseDto.Success = false;
                responseDto.Message = $"An error occurred: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<ResponseDto> UpdateEventManagementAsync(EventManagement eventManagement, string loggedinUserName)
        {
            var responseDto = new ResponseDto();
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var existingEventManagement = await _unitOfWork.EventManagement.GetByIdAsync(eventManagement.Id);
                eventManagement.AddedBy = existingEventManagement.AddedBy;
                eventManagement.AddedOn = existingEventManagement.AddedOn;
                eventManagement.UpdatedBy = loggedinUserName;
                eventManagement.UpdatedOn = DateTime.Now;
                await _unitOfWork.EventManagement.UpdateAsync(eventManagement);

                await _unitOfWork.EventStartEndTimeDayWise.DeleteAgainstFieldAsync(eventManagement.Id, "EventManagementId");

                foreach (var eventStartEndTime in eventManagement.EventStartEndTimeDayWiseList)
                {
                    eventStartEndTime.EventManagementId = eventManagement.Id;
                }

                _unitOfWork.EventStartEndTimeDayWise.AddRange(eventManagement.EventStartEndTimeDayWiseList);

                await _unitOfWork.EventServiceDetail.DeleteAgainstFieldAsync(eventManagement.Id, "EventManagementId");

                foreach (var eventServiceDetail in eventManagement.EventServiceDetailList)
                {
                    eventServiceDetail.EventManagementId = eventManagement.Id;
                }

                _unitOfWork.EventServiceDetail.AddRange(eventManagement.EventServiceDetailList);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                responseDto.Success = true;
                responseDto.Message = "EventStaff updated successfully!";
            }
            catch (Exception ex)
            {
                // Step 7: Rollback in case of any error
                await transaction.RollbackAsync();
                responseDto.Success = false;
                responseDto.Message = $"An error occurred while updating contract: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<string> GetNextEventManagementId()
        {
            try
            {
                var allEventManagement = await _unitOfWork.EventManagement.GetAllAsync();

                if (allEventManagement == null || !allEventManagement.Any())
                {
                    return "0001"; 
                }

                var lastEventManagement = allEventManagement
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                var eventManagementId = lastEventManagement.EventID;
                int numericPart = Convert.ToInt32(eventManagementId.Substring(5));

                numericPart++;

                return numericPart.ToString("D4");
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching the next Event Management Id.", ex);
            }
        }

        public async Task<CombinedEventManagementAndContractDetails> GetEventManagementById(long id)
        {
            try
            {
                var eventManagement = await _unitOfWork.EventManagement.GetWithIncludeAsync(
                    x => x.Id == id,
                    x => x.EventServiceDetailList,
                    x => x.EventStartEndTimeDayWiseList
                );

                if (eventManagement != null)
                {
                    var firstEventManagement = eventManagement.FirstOrDefault();

                    if (firstEventManagement == null)
                    {
                        throw new Exception($"EventManagement with ID {id} not found.");
                    }

                    var contractDetails = await _unitOfWork.ContractDetails.GetByIdAsync(firstEventManagement.ContractId);

                    if (contractDetails == null)
                    {
                        throw new Exception("No contract detail found.");
                    }

                    var eventStaff = await _unitOfWork.EventStaff.GetByNullableIdAsync(firstEventManagement.HIVDropOffStaffId);

                    var combinedDto = new CombinedEventManagementAndContractDetails
                    {
                        EventManagement = firstEventManagement,
                        ContractDetails = contractDetails,
                        EventStaff = eventStaff
                    };

                    return combinedDto;
                }
                else
                {
                    throw new Exception($"EventStaff with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                // Log and rethrow the exception with more context if needed
                throw new Exception("An error occurred while retrieving the EventStaff.", ex);
            }
        }

    }
}

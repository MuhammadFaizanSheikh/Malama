using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.UnitOfWork;

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
                //var existingContractDetails = await repository.FindForSearchingAsync(sc => sc.ContractID == contractDetail.ContractID);

                //if (existingContractDetails != null && existingContractDetails.Any())
                //{
                //    responseDto.Success = false;
                //    responseDto.Message = "ContractID already exist!!";
                //    return responseDto;
                //}

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

                    if (eventStaff == null)
                    {
                        throw new Exception("No Event Staff found.");
                    }

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

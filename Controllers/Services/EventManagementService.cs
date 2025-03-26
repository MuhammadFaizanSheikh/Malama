using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.UnitOfWork;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class EventManagementService : IEventManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public EventManagementService(IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
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

                //EventStartEndTimeDayWise
                await _unitOfWork.EventStartEndTimeDayWise.DeleteAgainstFieldAsync(eventManagement.Id, "EventManagementId");

                foreach (var eventStartEndTime in eventManagement.EventStartEndTimeDayWiseList)
                {
                    eventStartEndTime.EventManagementId = eventManagement.Id;
                }

                _unitOfWork.EventStartEndTimeDayWise.AddRange(eventManagement.EventStartEndTimeDayWiseList);

                //EventServiceDetail
                await _unitOfWork.EventServiceDetail.DeleteAgainstFieldAsync(eventManagement.Id, "EventManagementId");

                foreach (var eventServiceDetail in eventManagement.EventServiceDetailList)
                {
                    eventServiceDetail.EventManagementId = eventManagement.Id;
                }

                _unitOfWork.EventServiceDetail.AddRange(eventManagement.EventServiceDetailList);

                //EventStaffDetail
                await _unitOfWork.EventStaffDetail.DeleteAgainstFieldAsync(eventManagement.Id, "EventManagementId");

                foreach (var eventStaffDetail in eventManagement.EventStaffDetailList)
                {
                    eventStaffDetail.EventManagementId = eventManagement.Id;
                }

                _unitOfWork.EventStaffDetail.AddRange(eventManagement.EventStaffDetailList);

                //EventTaskforce
                await _unitOfWork.EventManagementTaskforces.DeleteAgainstFieldAsync(eventManagement.Id, "EventManagementId");

                foreach (var eventManagementTaskforce in eventManagement.EventManagementTaskforcesList)
                {
                    eventManagementTaskforce.EventManagementId = eventManagement.Id;
                }

                _unitOfWork.EventManagementTaskforces.AddRange(eventManagement.EventManagementTaskforcesList);

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
                var eventManagement = await _unitOfWork.EventManagement.GetWithInclude(
                    x => x.Id == id,
                    x => x.EventServiceDetailList,
                    x => x.EventStartEndTimeDayWiseList,
                    x => x.EventStaffDetailList
                ).Include(x => x.EventStaffDetailList)
                        .ThenInclude(l => l.EventWiseStaffRoleList) // Now second-level include works!
                    .FirstOrDefaultAsync();

                if (eventManagement != null)
                {
                    var firstEventManagement = eventManagement;

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

                    var EventStaffDetailAndAdditionalRoleslist = new List<EventStaffDetailAndAdditionalRoles>();

                    foreach (var eventStaffDetails in firstEventManagement.EventStaffDetailList)
                    {
                        EventStaffDetailAndAdditionalRoles eventStaffDetailAndAdditionalRoles = new EventStaffDetailAndAdditionalRoles();
                        //var eventStaffDetail = await _unitOfWork.EventStaff.GetByNullableIdAsync(eventStaffDetails.EventStaffId);
                        var eventStaffDetail = await _unitOfWork.EventStaff.GetWithInclude(
                        x => x.Id == eventStaffDetails.EventStaffId,
                        x => x.StaffLicense).Include(x => x.StaffLicense).ThenInclude(l => l.StaffLicenseDetails).FirstOrDefaultAsync();


                        if (eventStaffDetail != null)
                        {
                            var roles = await _roleManager.Roles.ToListAsync();
                            var roleDictionary = roles.ToDictionary(r => r.Id, r => r.Name);
                            var roleLicenseMapping = new Dictionary<string, List<string>>();

                            foreach (var staffLicense in eventStaffDetail.StaffLicense)
                            {
                                // Fetch Role Name from RoleManager using RoleId
                                if (roleDictionary.TryGetValue(staffLicense.RoleId, out string roleName))
                                {
                                    if (!roleLicenseMapping.ContainsKey(roleName))
                                    {
                                        roleLicenseMapping[roleName] = new List<string>();
                                    }

                                    // Extract LicenseState & LicenseType from StaffLicenseDetails
                                    foreach (var licenseDetail in staffLicense.StaffLicenseDetails)
                                    {
                                        roleLicenseMapping[roleName].Add($"{licenseDetail.LicenseState}: {licenseDetail.LicenseType}");
                                    }
                                }
                            }

                            var rolesString = string.Join(", ", roleLicenseMapping.Keys); // Comma-separated roles
                            var licensesString = string.Join("<br/>", roleLicenseMapping.Select(kv => string.Join(", ", kv.Value)));

                            eventStaffDetailAndAdditionalRoles.EventStaffRolesNameAndLicense = new CombinedEventStaffRolesNameAndLicense
                            {
                                Id = eventStaffDetail.Id,
                                StaffID = eventStaffDetail.StaffID,
                                StaffLastName = eventStaffDetail.StaffLastName,
                                StaffFirstName = eventStaffDetail.StaffFirstName,
                                PrimaryCity = eventStaffDetail.PrimaryCity,
                                PrimaryState = eventStaffDetail.PrimaryState,
                                PrimaryZip = eventStaffDetail.PrimaryZip,
                                StaffCAC = eventStaffDetail.StaffCAC,
                                Roles = rolesString,
                                LicenseStateAndTypes = licensesString,
                                Status = eventStaffDetail.StaffStatus
                            };
                            eventStaffDetailAndAdditionalRoles.EventStaffDetail = eventStaffDetails;
                            EventStaffDetailAndAdditionalRoleslist.Add(eventStaffDetailAndAdditionalRoles);
                        }
                    }

                    // Combine data into DTO
                    var combinedDto = new CombinedEventManagementAndContractDetails
                    {
                        EventManagement = firstEventManagement,
                        ContractDetails = contractDetails,
                        EventStaffForHIVDropOff = eventStaff,
                        EventStaffDetailAndAdditionalRoleslist = EventStaffDetailAndAdditionalRoleslist // Now contains multiple records
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

        public async Task<EventManagement> GetEventManagementForEventSelectionById(long id)
        {
            try
            {
                var eventManagement = await _unitOfWork.EventManagement.GetWithInclude(
                    x => x.Id == id,
                    x => x.EventServiceDetailList,
                    x => x.EventStartEndTimeDayWiseList,
                    x => x.EventStaffDetailList
                ).Include(x => x.EventStaffDetailList)
                 .ThenInclude(l => l.EventWiseStaffRoleList)
                 .FirstOrDefaultAsync();

                if (eventManagement == null)
                {
                    throw new KeyNotFoundException($"EventManagement with ID {id} not found.");
                }

                return eventManagement;
            }
            catch (KeyNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving event details.", ex);
            }
        }
    }
}

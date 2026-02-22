using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
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

        public async Task<List<EventManagementPreview>> GetAllEventManagements()
        {
            List<EventManagementPreview> eventManagements = new List<EventManagementPreview>();

            try
            {
                var eventData = await _unitOfWork.EventManagement.GetWithIncludeAsync(
                    null,
                    x => x.EventManagementTaskforcesList
                );

                // Group by EventID to handle versions
                eventManagements = eventData
                    .GroupBy(e => e.EventID)
                    .SelectMany(group =>
                    {
                        // Find the highest version for each EventID
                        int maxVersion = group.Max(e => e.EventVersion);

                        return group.OrderByDescending(e => e.EventVersion)
                            .Select(e => new EventManagementPreview
                            {
                                Id = e.Id,
                                EventID = e.EventID,
                                EventVersion = e.EventVersion,
                                SubEventID = e.SubEventID,
                                EventStatus = e.EventStatus,
                                EventState = e.EventState,
                                EventCity = e.EventCity,
                                EventZipCode = e.EventZipCode,
                                StatusDescription = e.StatusDescription,
                                EventStartDate = e.EventStartDate,
                                EventEndDate = e.EventEndDate,
                                TaskForce = e.EventManagementTaskforcesList != null
                                    ? string.Join(", ", e.EventManagementTaskforcesList.Select(t => t.Taskforce))
                                    : string.Empty,
                                // Logic for CanEdit:
                                // Allow edit if not cancelled OR if it's the latest version
                                CanEdit = !string.Equals(e.EventStatus, "Canceled", StringComparison.OrdinalIgnoreCase)
                                          || e.EventVersion == maxVersion
                            })
                            .ToList();
                    })
                    .OrderByDescending(e => e.Id)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

            return eventManagements;
        }


        public async Task<List<EventManagementPreview>> GetAllEventID()
        {
            try
            {
                // Fetch only the necessary data
                return await _unitOfWork.EventManagement
                .FindForSearching(e => e.EventStatus != "Canceled") // filtered in DB
                .OrderByDescending(e => e.Id)
                .Select(e => new EventManagementPreview
                {
                    Id = e.Id,
                    EventID = e.EventID
                })
                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<ResponseDto> AddEventManagementAsync(EventManagement eventManagement, string submissionToken, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                var existingToken = await _unitOfWork.SubmissionTokenRecord.FindAsync(t => t.Token == submissionToken);
                if (existingToken != null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "This form has already been submitted."
                    };
                }

                // 2️⃣ Save token first
                await _unitOfWork.SubmissionTokenRecord.AddAsync(new SubmissionTokenRecord
                {
                    Token = submissionToken,
                    CreatedAt = DateTime.Now
                });


                eventManagement.AddedBy = loggedinUserName;
                eventManagement.AddedOn = DateTime.Now;
                await _unitOfWork.EventManagement.AddAsync(eventManagement);

                responseDto.Success = true;
                responseDto.Message = "Event added successfully!";
            }
            catch (Exception ex)
            {
                // If an exception occurs, set Success to false and provide the error message
                responseDto.Success = false;
                responseDto.Message = $"An error occurred: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<ResponseDto> UpdateEventManagementAsync(EventManagement eventManagement, string loggedinUserName, string action)
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

                //EventStaff Availability Date
                await _unitOfWork.EventManagementStaffAvailability.DeleteAgainstFieldAsync(eventManagement.Id, "EventStaffDetailId");

                foreach (var eventStaffDetail in eventManagement.EventStaffDetailList)
                {
                    foreach (var staffAvailabilityDate in eventStaffDetail.AvailabilityDatesList)
                    {
                        staffAvailabilityDate.EventStaffDetailId = eventStaffDetail.Id;
                    }

                    _unitOfWork.EventManagementStaffAvailability.AddRange(eventStaffDetail.AvailabilityDatesList);
                }

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

                if (action == "Update")
                {
                    responseDto.Message = "Event updated successfully!";
                }
                else if (action == "UpdateAndDuplicate")
                {
                    responseDto.Message = "Event duplicated successfully!";
                }
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
                    x => x.EventManagementTaskforcesList,
                    x => x.EventStaffDetailList
                        ).Include(x => x.EventStaffDetailList)
                        .ThenInclude(l => l.EventWiseStaffRoleList)
                        .Include(x => x.EventStaffDetailList)
                        .ThenInclude(l => l.EventWiseStaffSecondaryRoleList)// Now second-level include works!
                        .Include(x => x.EventStaffDetailList)
                        .ThenInclude(l => l.AvailabilityDatesList)
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
                        if ( !string.IsNullOrEmpty(eventStaffDetails.SelectedSecondaryStation))
                        {
                            eventStaffDetails.SelectedSecondaryStationList = eventStaffDetails.SelectedSecondaryStation.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                        }

                        EventStaffDetailAndAdditionalRoles eventStaffDetailAndAdditionalRoles = new EventStaffDetailAndAdditionalRoles();
                        //var eventStaffDetail = await _unitOfWork.EventStaff.GetByNullableIdAsync(eventStaffDetails.EventStaffId);
                        var eventStaffDetail = await _unitOfWork.EventStaff.GetWithInclude(
                        x => x.Id == eventStaffDetails.EventStaffId,
                        x => x.StaffQualification).Include(x => x.StaffQualification).ThenInclude(l => l.StaffLicenseDetails)
                        .Include(x => x.StaffQualification)
                        .ThenInclude(l => l.StaffAttributeDetails).FirstOrDefaultAsync();


                        if (eventStaffDetail != null)
                        {
                            var roleLicenseMapping = new Dictionary<string, List<string>>();
                            var attributeList = new List<string>();

                            foreach (var staffLicense in eventStaffDetail.StaffQualification)
                            {
                                var roleName = staffLicense.QualificationName?.Trim();

                                if (!string.IsNullOrWhiteSpace(roleName))
                                {
                                    if (!roleLicenseMapping.ContainsKey(roleName))
                                    {
                                        roleLicenseMapping[roleName] = new List<string>();
                                    }

                                    // Add LicenseState & LicenseType
                                    foreach (var licenseDetail in staffLicense.StaffLicenseDetails)
                                    {
                                        roleLicenseMapping[roleName].Add($"{licenseDetail.LicenseState}: {licenseDetail.LicenseType}");
                                    }

                                    // Add Attributes
                                    if (staffLicense.StaffAttributeDetails != null)
                                    {
                                        foreach (var attrDetail in staffLicense.StaffAttributeDetails)
                                        {
                                            if (!string.IsNullOrWhiteSpace(attrDetail.Attribute))
                                            {
                                                attributeList.Add(attrDetail.Attribute.Trim());
                                            }
                                        }
                                    }
                                }
                            }

                            // Prepare output strings
                            var rolesString = string.Join(", ", roleLicenseMapping.Keys);

                            var licensesString = string.Join("<br/>",
                                roleLicenseMapping.Select(kv => string.Join(", ", kv.Value))
                            );

                            var attributesString = string.Join(", ",
                                attributeList
                                    .Where(a => !string.IsNullOrWhiteSpace(a))
                                    .Select(a => a.Trim())
                                    .Distinct(StringComparer.OrdinalIgnoreCase)
                                    .OrderBy(a => a)
                            );

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
                                Status = eventStaffDetail.StaffStatus,
                                Attributes = attributesString
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
                 .Include(x => x.EventStaffDetailList)
                 .ThenInclude(l => l.EventWiseStaffSecondaryRoleList)
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

        public async Task<EventManagement> GetEventManagementForEventSelectionByIdWithoutInclude(long id)
        {
            try
            {
                var eventManagement = await _unitOfWork.EventManagement.FindAsync(x => x.Id == id);

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

        public async Task<EventManagement> GetEventManagementByEventIdWithoutInclude(string eventId)
        {
            try
            {
                var eventManagement = await _unitOfWork.EventManagement.FindAsync(x => x.EventID == eventId);

                if (eventManagement == null)
                {
                    throw new KeyNotFoundException($"EventManagement with ID {eventId} not found.");
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

        public async Task<(DateTime StartDate, DateTime EndDate, int Version)>GetEventDetailsById(long eventId)
        {
            var result = await _unitOfWork.EventManagement
                .FindForSearching(x => x.Id == eventId)
                .Select(x => new
                {
                    x.EventStartDate,
                    x.EventEndDate,
                    x.EventVersion
                })
                .FirstOrDefaultAsync();

            if (result == null)
                throw new KeyNotFoundException($"Event not found. EventId: {eventId}");

            return (result.EventStartDate, result.EventEndDate, result.EventVersion);
        }
    }
}

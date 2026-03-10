using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class EventManagementService : IEventManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubmissionTokenService _submissionTokenService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<EventManagementService> _logger;
        private const string CLASSNAME = "ContractService";

        public EventManagementService(ILogger<EventManagementService> logger, IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, ISubmissionTokenService submissionTokenService)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _logger = logger;
            _submissionTokenService = submissionTokenService;
        }

        public async Task<List<EventManagementPreview>> GetAllEventManagements(long? eventIdFilter = null)
        {
            const string methodName = "GetAllEventManagements";

            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventIdFilter: {EventIdFilter}",
                CLASSNAME, methodName, eventIdFilter);

            List<EventManagementPreview> eventManagements = new();

            try
            {
                Expression<Func<EventManagement, bool>> predicate = null;

                if (eventIdFilter.HasValue)
                {
                    long id = eventIdFilter.Value; // avoid closure issues
                    predicate = e => e.Id == id;

                    _logger.LogInformation("{ClassName}, {MethodName}, Applying filter for EventId: {EventId}",
                        CLASSNAME, methodName, id);
                }

                var eventData = await _unitOfWork.EventManagement.GetWithIncludeAsync(
                    predicate,
                    x => x.EventManagementTaskforcesList
                );

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved records count: {Count}",
                    CLASSNAME, methodName, eventData?.Count() ?? 0);

                // Group by EventID to handle versions
                eventManagements = eventData
                    .GroupBy(e => e.EventID)
                    .SelectMany(group =>
                    {
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

                                // Same CanEdit logic (unchanged)
                                CanEdit = !string.Equals(e.EventStatus, "Canceled", StringComparison.OrdinalIgnoreCase)
                                          || e.EventVersion == maxVersion
                            })
                            .ToList();
                    })
                    .OrderByDescending(e => e.Id)
                    .ToList();

                _logger.LogInformation("{ClassName}, {MethodName}, Returning preview count: {PreviewCount}",
                    CLASSNAME, methodName, eventManagements.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while retrieving EventManagements. EventIdFilter: {EventIdFilter}",
                    CLASSNAME, methodName, eventIdFilter);

                throw;
            }

            return eventManagements;
        }


        public async Task<List<EventManagementPreview>> GetAllEventID()
        {
            const string methodName = "GetAllEventID";

            _logger.LogInformation("{ClassName}, {MethodName}, Called",
                CLASSNAME, methodName);

            try
            {
                // Fetch only the necessary data
                var result = await _unitOfWork.EventManagement
                    .FindForSearching(e => e.EventStatus != "Canceled") // filtered in DB
                    .OrderByDescending(e => e.Id)
                    .Select(e => new EventManagementPreview
                    {
                        Id = e.Id,
                        EventID = $"{e.EventID} (V{e.EventVersion})"
                    })
                    .ToListAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved EventID count: {Count}",
                    CLASSNAME, methodName, result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while retrieving EventIDs",
                    CLASSNAME, methodName);

                throw;
            }
        }


        public async Task<ResponseDto> AddEventManagementAsync(EventManagement eventManagement, string submissionToken, string loggedinUserName)
        {
            const string methodName = "AddEventManagementAsync";

            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}, User: {UserName}",
                CLASSNAME, methodName, eventManagement?.EventID, loggedinUserName);

            var responseDto = new ResponseDto();

            try
            {
                var tokenResult = await _submissionTokenService.ValidateAndSaveAsync(submissionToken, loggedinUserName);

                if (!tokenResult.Success)
                {
                    return tokenResult;
                }

                if (!ValidateEventStatus(eventManagement, loggedinUserName, false, out string statusError))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid EventStatus: {Status}, User: {UserName}, Error: {Error}",
                        CLASSNAME, methodName, eventManagement.EventStatus, loggedinUserName, statusError);

                    responseDto.Success = false;
                    responseDto.Message = statusError;
                    return responseDto;
                }

                eventManagement.AddedBy = loggedinUserName;
                eventManagement.AddedOn = DateTime.Now;

                await _unitOfWork.EventManagement.AddAsync(eventManagement);

                _logger.LogInformation("{ClassName}, {MethodName}, Event added successfully. EventID: {EventID}, User: {UserName}",
                    CLASSNAME, methodName, eventManagement.EventID, loggedinUserName);

                responseDto.Success = true;
                responseDto.Message = "Event added successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while adding Event. EventID: {EventID}, User: {UserName}",
                    CLASSNAME, methodName, eventManagement?.EventID, loggedinUserName);

                responseDto.Success = false;
                responseDto.Message = $"An error occurred: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<ResponseDto> UpdateEventManagementAsync(EventManagement updatedModel, string loggedinUserName, string action)
        {
            const string methodName = "UpdateEventManagementAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}, Action: {Action}, User: {UserName}",
                CLASSNAME, methodName, updatedModel?.Id, action, loggedinUserName);

            var responseDto = new ResponseDto();

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1️⃣ Load existing entity WITH children
                var existing = (await _unitOfWork.EventManagement
                    .GetWithIncludeAsync(
                        e => e.Id == updatedModel.Id,
                        e => e.EventServiceDetailList,
                        e => e.EventStartEndTimeDayWiseList,
                        e => e.EventStaffDetailList,
                        e => e.EventManagementTaskforcesList
                    )).FirstOrDefault();

                if (existing == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Event not found. EventID: {EventID}, User: {UserName}",
                        CLASSNAME, methodName, updatedModel.Id, loggedinUserName);

                    responseDto.Success = false;
                    responseDto.Message = "Event not found.";
                    return responseDto;
                }

                if (!ValidateEventStatus(updatedModel, loggedinUserName, true, out string statusError))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid EventStatus: {Status}, EventID: {EventID}, User: {UserName}, Error: {Error}",
                        CLASSNAME, methodName, updatedModel.EventStatus, updatedModel.Id, loggedinUserName, statusError);

                    responseDto.Success = false;
                    responseDto.Message = statusError;
                    return responseDto;
                }

                // 2️⃣ Preserve audit fields
                updatedModel.AddedBy = existing.AddedBy;
                updatedModel.AddedOn = existing.AddedOn;
                updatedModel.UpdatedBy = loggedinUserName;
                updatedModel.UpdatedOn = DateTime.Now;

                // 3️⃣ Update scalar properties
                _unitOfWork.SetValues(existing, updatedModel);

                _logger.LogInformation("{ClassName}, {MethodName}, Updating collections for EventID: {EventID}, User: {UserName}",
                    CLASSNAME, methodName, updatedModel.Id, loggedinUserName);

                // =========================
                // 4️⃣ Replace CHILD COLLECTIONS
                // =========================
                existing.EventServiceDetailList.Clear();
                foreach (var item in updatedModel.EventServiceDetailList)
                {
                    existing.EventServiceDetailList.Add(item);
                }

                existing.EventStartEndTimeDayWiseList.Clear();
                foreach (var item in updatedModel.EventStartEndTimeDayWiseList)
                {
                    existing.EventStartEndTimeDayWiseList.Add(item);
                }

                existing.EventManagementTaskforcesList.Clear();
                foreach (var item in updatedModel.EventManagementTaskforcesList)
                {
                    existing.EventManagementTaskforcesList.Add(item);
                }

                // =========================
                // 5️⃣ EventStaffDetail (Nested Graph)
                // =========================
                existing.EventStaffDetailList.Clear();

                foreach (var staff in updatedModel.EventStaffDetailList)
                {
                    var newStaff = new EventStaffDetail
                    {
                        EventStaffId = staff.EventStaffId,
                        SelectedStation = staff.SelectedStation,
                        PreEventAvailability = staff.PreEventAvailability,
                        ProfileButtonAccess = staff.ProfileButtonAccess,
                        SelectedSecondaryStation = staff.SelectedSecondaryStation
                    };

                    foreach (var date in staff.AvailabilityDatesList)
                    {
                        newStaff.AvailabilityDatesList.Add(
                            new EventManagementStaffAvailability
                            {
                                AvailabilityDate = date.AvailabilityDate
                            });
                    }

                    foreach (var role in staff.EventWiseStaffRoleList)
                    {
                        newStaff.EventWiseStaffRoleList.Add(
                            new EventWiseStaffRole
                            {
                                RoleId = role.RoleId
                            });
                    }

                    foreach (var role in staff.EventWiseStaffSecondaryRoleList)
                    {
                        newStaff.EventWiseStaffSecondaryRoleList.Add(
                            new EventWiseStaffSecondaryRole
                            {
                                RoleId = role.RoleId
                            });
                    }

                    existing.EventStaffDetailList.Add(newStaff);
                }

                // 6️⃣ Save once
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Event saved successfully. EventID: {EventID}, Action: {Action}, User: {UserName}",
                    CLASSNAME, methodName, updatedModel.Id, action, loggedinUserName);

                responseDto.Success = true;
                responseDto.Message = action == "Update"
                    ? "Event updated successfully!"
                    : "Event duplicated successfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while updating Event. EventID: {EventID}, Action: {Action}, User: {UserName}",
                    CLASSNAME, methodName, updatedModel?.Id, action, loggedinUserName);

                responseDto.Success = false;
                responseDto.Message = $"Error updating event: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<string> GetNextEventIdNumber()
        {
            const string methodName = "GetNextEventIdNumber";
            _logger.LogInformation("{ClassName}, {MethodName} called.", CLASSNAME, methodName);

            try
            {
                // Step 1: Get all EventIDs from database
                var eventIds = await _unitOfWork.EventManagement
                    .GetAll()
                    .Where(x => !string.IsNullOrEmpty(x.EventID))
                    .Select(x => x.EventID)
                    .ToListAsync(); // only EventID column

                _logger.LogInformation("{ClassName}, {MethodName}, Total EventIDs fetched: {Count}",
                    CLASSNAME, methodName, eventIds.Count);

                // Step 2: Extract numeric part in memory
                int maxNumber = eventIds
                    .Select(eid =>
                    {
                        var numericPart = new string(eid.Reverse()
                                                        .TakeWhile(char.IsDigit)
                                                        .Reverse()
                                                        .ToArray());
                        return int.TryParse(numericPart, out int n) ? n : 0;
                    })
                    .DefaultIfEmpty(0)
                    .Max();

                _logger.LogInformation("{ClassName}, {MethodName}, Max numeric part found: {MaxNumber}",
                    CLASSNAME, methodName, maxNumber);

                // Step 3: Increment
                int nextNumber = maxNumber + 1;

                // Step 4: Return 4-digit string
                var nextEventIdNumber = nextNumber.ToString("D4");

                _logger.LogInformation("{ClassName}, {MethodName}, Next EventID number to use: {NextEventID}",
                    CLASSNAME, methodName, nextEventIdNumber);

                return nextEventIdNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while fetching next EventId number",
                    CLASSNAME, methodName);

                throw new Exception("Error while fetching the next EventId number.", ex);
            }
        }

        //public async Task<string> GetNextEventManagementId()
        //{
        //    const string methodName = "GetNextEventManagementId";

        //    _logger.LogInformation("{ClassName}, {MethodName}, Called",
        //        CLASSNAME, methodName);

        //    try
        //    {
        //        var allEventManagement = await _unitOfWork.EventManagement.GetAllAsync();

        //        _logger.LogInformation("{ClassName}, {MethodName}",
        //            CLASSNAME, methodName);

        //        if (allEventManagement == null || !allEventManagement.Any())
        //        {
        //            _logger.LogInformation("{ClassName}, {MethodName}, No existing EventManagement found. Returning default ID '0001'",
        //                CLASSNAME, methodName);

        //            return "0001";
        //        }

        //        var lastEventManagement = allEventManagement
        //            .OrderByDescending(c => c.Id)
        //            .FirstOrDefault();

        //        var eventManagementId = lastEventManagement.EventID;
        //        int numericPart = Convert.ToInt32(eventManagementId.Substring(5));

        //        numericPart++;

        //        var nextId = numericPart.ToString("D4");

        //        _logger.LogInformation("{ClassName}, {MethodName}, Last EventID: {LastEventID}, Next EventID: {NextEventID}",
        //            CLASSNAME, methodName, eventManagementId, nextId);

        //        return nextId;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,
        //            "{ClassName}, {MethodName}, Exception occurred while fetching next EventManagement ID",
        //            CLASSNAME, methodName);

        //        throw new Exception("Error while fetching the next Event Management Id.", ex);
        //    }
        //}

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
                        if (!string.IsNullOrEmpty(eventStaffDetails.SelectedSecondaryStation))
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
                        EventRuntimeStatus = GetEventRuntimeStatus(firstEventManagement.EventStartDate, firstEventManagement.EventEndDate, firstEventManagement.EventStartEndTimeDayWiseList),
                        EventManagement = firstEventManagement,
                        ContractDetails = contractDetails,
                        EventStaffForHIVDropOff = eventStaff,
                        EventStaffDetailAndAdditionalRoleslist = EventStaffDetailAndAdditionalRoleslist // Now contains multiple records,
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
            const string methodName = "GetEventManagementForEventSelectionById";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}",
                CLASSNAME, methodName, id);

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
                    _logger.LogWarning("{ClassName}, {MethodName}, EventManagement not found for ID: {EventID}",
                        CLASSNAME, methodName, id);
                    throw new KeyNotFoundException($"EventManagement with ID {id} not found.");
                }

                _logger.LogInformation("{ClassName}, {MethodName}, EventManagement retrieved successfully for EventID: {EventID}",
                    CLASSNAME, methodName, id);

                return eventManagement;
            }
            catch (KeyNotFoundException ex)
            {
                // Already logged above, just rethrow
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while retrieving EventManagement for ID: {EventID}",
                    CLASSNAME, methodName, id);
                throw new ApplicationException("An error occurred while retrieving event details.", ex);
            }
        }

        public async Task<EventManagement> GetEventManagementForEventSelectionByIdWithoutInclude(long id)
        {
            const string methodName = "GetEventManagementForEventSelectionByIdWithoutInclude";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}",
                CLASSNAME, methodName, id);

            try
            {
                var eventManagement = await _unitOfWork.EventManagement.FindAsync(x => x.Id == id);

                if (eventManagement == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, EventManagement not found for ID: {EventID}",
                        CLASSNAME, methodName, id);
                    throw new KeyNotFoundException($"EventManagement with ID {id} not found.");
                }

                _logger.LogInformation("{ClassName}, {MethodName}, EventManagement retrieved successfully for EventID: {EventID}",
                    CLASSNAME, methodName, id);

                return eventManagement;
            }
            catch (KeyNotFoundException)
            {
                // Already logged above, just rethrow
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while retrieving EventManagement for ID: {EventID}",
                    CLASSNAME, methodName, id);
                throw new ApplicationException("An error occurred while retrieving event details.", ex);
            }
        }

        public async Task<EventManagement> GetEventManagementByEventIdWithoutInclude(string eventId)
        {
            const string methodName = "GetEventManagementByEventIdWithoutInclude";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}",
                CLASSNAME, methodName, eventId);

            try
            {
                var eventManagement = await _unitOfWork.EventManagement.FindAsync(x => x.EventID == eventId);

                if (eventManagement == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, EventManagement not found for EventID: {EventID}",
                        CLASSNAME, methodName, eventId);
                    throw new KeyNotFoundException($"EventManagement with ID {eventId} not found.");
                }

                _logger.LogInformation("{ClassName}, {MethodName}, EventManagement retrieved successfully for EventID: {EventID}",
                    CLASSNAME, methodName, eventId);

                return eventManagement;
            }
            catch (KeyNotFoundException)
            {
                // Already logged above, just rethrow
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while retrieving EventManagement for EventID: {EventID}",
                    CLASSNAME, methodName, eventId);
                throw new ApplicationException("An error occurred while retrieving event details.", ex);
            }
        }

        public async Task<(DateTime StartDate, DateTime EndDate, int Version)> GetEventDetailsById(long eventId)
        {
            const string methodName = "GetEventDetailsById";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}",
                CLASSNAME, methodName, eventId);

            try
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
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Event not found for EventID: {EventID}",
                        CLASSNAME, methodName, eventId);
                    throw new KeyNotFoundException($"Event not found. EventId: {eventId}");
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Event retrieved successfully for EventID: {EventID}, StartDate: {StartDate}, EndDate: {EndDate}, Version: {Version}",
                    CLASSNAME, methodName, eventId, result.EventStartDate, result.EventEndDate, result.EventVersion);

                return (result.EventStartDate, result.EventEndDate, result.EventVersion);
            }
            catch (KeyNotFoundException)
            {
                // Already logged above, just rethrow
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while retrieving Event details for EventID: {EventID}",
                    CLASSNAME, methodName, eventId);
                throw new ApplicationException("An error occurred while retrieving event details.", ex);
            }
        }

        public bool ValidateEventStatus(EventManagement model, string loggedinUserName, bool isUpdate, out string errorMessage)
        {
            const string methodName = "ValidateEventStatus";
            errorMessage = string.Empty;

            _logger.LogInformation("{ClassName}, {MethodName}, Called for EventID: {EventID}, User: {UserName}, Status: {Status}",
                CLASSNAME, methodName, model?.EventID, loggedinUserName, model?.EventStatus);

            try
            {
                if (model == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Model is null, User: {UserName}",
                        CLASSNAME, methodName, loggedinUserName);

                    errorMessage = "Invalid request.";
                    return false;
                }

                if (model.EventStartEndTimeDayWiseList == null || !model.EventStartEndTimeDayWiseList.Any())
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Day-wise list empty, EventID: {EventID}, User: {UserName}",
                        CLASSNAME, methodName, model.EventID, loggedinUserName);

                    errorMessage = "Event time data missing.";
                    return false;
                }

                // Build actual start/end datetime
                var firstDay = model.EventStartEndTimeDayWiseList.First();
                var lastDay = model.EventStartEndTimeDayWiseList.Last();

                if (firstDay.EventStartTime == null || lastDay.EventEndTime == null)
                {
                    throw new Exception("Event start or end time is missing.");
                }

                var eventActualStart = model.EventStartDate.Add(firstDay.EventStartTime.Value);
                var eventActualEnd = model.EventEndDate.Add(lastDay.EventEndTime.Value);

                var now = DateTime.Now;

                var allowedStatuses = new List<string>();

                if (!isUpdate)
                {
                    allowedStatuses.Add("Initial Pre-event");
                }
                else
                {
                    if (now < eventActualStart)
                    {
                        allowedStatuses.AddRange(new[]
                        {
                        "Initial Pre-event",
                        "Pre-Event Confirmed",
                        "Pre-Event Complete",
                        "Canceled"
                    });
                    }
                    else if (now >= eventActualStart && now <= eventActualEnd)
                    {
                        allowedStatuses.Add("In Progress");
                    }
                    else if (now > eventActualEnd)
                    {
                        allowedStatuses.AddRange(new[]
                        {
                        "Post Event Processing",
                        "Complete"
                    });
                    }
                }

                // Validate user input
                if (!allowedStatuses.Contains(model.EventStatus))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Status tampering detected. Attempted: {AttemptedStatus}, Allowed: {AllowedStatuses}, EventID: {EventID}, User: {UserName}",
                        CLASSNAME, methodName, model.EventStatus, string.Join(",", allowedStatuses), model.EventID, loggedinUserName);

                    errorMessage = "Invalid status transition.";
                    return false;
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Status validated successfully. EventID: {EventID}, User: {UserName}, Status: {Status}",
                    CLASSNAME, methodName, model.EventID, loggedinUserName, model.EventStatus);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception during status validation. EventID: {EventID}, User: {UserName}, Status: {Status}",
                    CLASSNAME, methodName, model?.EventID, loggedinUserName, model?.EventStatus);

                errorMessage = $"An unexpected error occurred: {ex.Message}";
                return false;
            }
        }

        private string GetEventRuntimeStatus(DateTime eventStartDate, DateTime eventEndDate, List<EventStartEndTimeDayWise> dayWiseList)
        {
            if (dayWiseList == null || !dayWiseList.Any())
                return "Event Not Started";

            var firstDay = dayWiseList.OrderBy(x => x.EventDay).First();
            var lastDay = dayWiseList.OrderBy(x => x.EventDay).Last();

            var actualStart = eventStartDate.Date + (firstDay.EventStartTime ?? TimeSpan.Zero);
            var actualEnd = eventEndDate.Date + (lastDay.EventEndTime ?? TimeSpan.Zero);

            var now = DateTime.Now;

            if (now < actualStart)
                return "Event Not Started";

            if (now >= actualStart && now <= actualEnd)
                return "Event In Progress";

            return "Event Completed";
        }
    }
}

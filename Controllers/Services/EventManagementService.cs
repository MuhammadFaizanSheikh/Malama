using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AutoMapper;
using ExcelFilesCompiler.Utilities;
using Microsoft.Extensions.Logging;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class EventManagementService : IEventManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISubmissionTokenService _submissionTokenService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<EventManagementService> _logger;
        private const string CLASSNAME = "ContractService";

        public EventManagementService(ILogger<EventManagementService> logger, IMapper mapper, IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, ISubmissionTokenService submissionTokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
            _logger = logger;
            _submissionTokenService = submissionTokenService;
        }

        public async Task<List<EventManagementPreview>> GetAllEventManagements(long? eventIdFilter = null)
        {
            const string methodName = "GetAllEventManagements";

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called with EventIdFilter: {EventIdFilter}",
                CLASSNAME, methodName, eventIdFilter);

            try
            {
                Expression<Func<EventManagement, bool>> predicate = null;

                if (eventIdFilter.HasValue)
                {
                    long id = eventIdFilter.Value;

                    predicate = e => e.Id == id;

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Applying filter for EventId: {EventId}",
                        CLASSNAME, methodName, id);
                }

                // ✅ NoTracking + Include
                var eventData = await _unitOfWork.EventManagement
                    .GetWithIncludeNoTracking(predicate, x => x.EventManagementTaskforcesList)
                    .ToListAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved records count: {Count}",
                    CLASSNAME, methodName, eventData.Count);

                // ✅ In-memory grouping (needed due to Max + string.Join)
                var eventManagements = eventData
                    .GroupBy(e => e.EventID)
                    .SelectMany(group =>
                    {
                        int maxVersion = group.Max(e => e.EventVersion);

                        return group
                            .OrderByDescending(e => e.EventVersion)
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
                                EventStartDateUtc = e.EventStartDateUtc,
                                EventEndDateUtc = e.EventEndDateUtc,

                                // ⚠️ Safe: executed in memory
                                TaskForce = e.EventManagementTaskforcesList != null
                                    ? string.Join(", ",
                                        e.EventManagementTaskforcesList
                                            .Select(t => t.Taskforce))
                                    : string.Empty,

                                CanEdit =
                                    !string.Equals(e.EventStatus, "Canceled", StringComparison.OrdinalIgnoreCase)
                                    || e.EventVersion == maxVersion
                            });
                    })
                    .OrderByDescending(e => e.Id)
                    .ToList();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Returning preview count: {PreviewCount}",
                    CLASSNAME, methodName, eventManagements.Count);

                return eventManagements;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception occurred while retrieving EventManagements. EventIdFilter: {EventIdFilter}",
                    CLASSNAME, methodName, eventIdFilter);

                throw;
            }
        }

        public async Task<List<PostEventManagementPreview>> GetAllForPostEventManagements()
        {
            const string methodName = "GetAllEventManagements";

            _logger.LogInformation("{ClassName}, {MethodName}, Called.", CLASSNAME, methodName);

            try
            {
                // 1️⃣ Get all EventManagement records with Taskforces (no tracking)
                var eventData = await _unitOfWork.EventManagement
                    .GetWithIncludeNoTracking(
                        x => x.EventStatus == AppConstants.EventStatus.InProgressComplete,
                        x => x.EventManagementTaskforcesList,
                        x => x.PostEventManagement // Include child table
                    )
                    .ToListAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved records count: {Count}",
                    CLASSNAME, methodName, eventData.Count);

                // 2️⃣ Group by EventID to get latest version
                var eventManagements = eventData
                    .GroupBy(e => e.EventID)
                    .SelectMany(group =>
                    {
                        int maxVersion = group.Max(e => e.EventVersion);

                        return group
                            .OrderByDescending(e => e.EventVersion)
                            .Select(e => new PostEventManagementPreview
                            {
                                Id = e.Id,
                                EventID = e.EventID,
                                EventVersion = e.EventVersion,
                                EventStatus = e.EventStatus,
                                EventState = e.EventState,
                                EventCity = e.EventCity,
                                EventZipCode = e.EventZipCode,
                                StatusDescription = e.PostEventManagement == null
                                ? "Pending"
                                : string.Equals(e.PostEventManagement.PostEventStatus, "Complete", StringComparison.OrdinalIgnoreCase)
                                    ? "Completed"
                                    : "Pending",
                                EventStartDateUtc = e.EventStartDateUtc,
                                EventEndDateUtc = e.EventEndDateUtc,

                                TaskForce = e.EventManagementTaskforcesList != null
                                    ? string.Join(", ", e.EventManagementTaskforcesList.Select(t => t.Taskforce))
                                    : string.Empty,

                                CanEdit = !string.Equals(e.EventStatus, "Canceled", StringComparison.OrdinalIgnoreCase)
                                          || e.EventVersion == maxVersion,

                                // 3️⃣ Get PostEventManagementId from child table if exists
                                PostEventManagementId = e.PostEventManagement != null
                                    ? e.PostEventManagement.Id
                                    : 0
                            });
                    })
                    .OrderByDescending(e => e.Id)
                    .ToList();

                _logger.LogInformation("{ClassName}, {MethodName}, Returning preview count: {PreviewCount}",
                    CLASSNAME, methodName, eventManagements.Count);

                return eventManagements;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception occurred while retrieving EventManagements.",
                    CLASSNAME, methodName);

                throw;
            }
        }

        public async Task<PostEventManagementDto> GetForPostEventManagement(long eventManagementId)
        {
            const string methodName = nameof(GetForPostEventManagement);

            _logger.LogInformation("{ClassName}, {MethodName}, Called. EventManagementId={Id}",
                CLASSNAME, methodName, eventManagementId);

            try
            {
                var em = await _unitOfWork.EventManagement
                    .GetWithIncludeNoTracking(
                        x => x.Id == eventManagementId,
                        x => x.ContractDetails,
                        x => x.EventManagementTaskforcesList,
                        x => x.EventStartEndTimeDayWiseList,
                        x => x.EventServiceDetailList.OrderBy(e => e.Id)
                    )
                    .FirstOrDefaultAsync();

                if (em == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Event not found. Id={Id}",
                        CLASSNAME, methodName, eventManagementId);

                    throw new Exception("EventManagement record not found.");
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Event found. Converting timezone. Timezone={Timezone}",
                    CLASSNAME, methodName, em.Timezone);

                Helper.ConvertEventToLocalTime(em, em.Timezone);

                var dto = new PostEventManagementDto
                {
                    Id = 0, // new PostEvent record
                    EventManagementId = em.Id,

                    EventStartDateUtc = em.EventStartDateUtc,
                    EventEndDateUtc = em.EventEndDateUtc,

                    PostEventNotes = null,

                    // ✅ Day-wise mapping
                    PostEventStartEndTimeDayWiseDto = em.EventStartEndTimeDayWiseList != null
                        ? em.EventStartEndTimeDayWiseList.Select(x => new PostEventStartEndTimeDayWiseDto
                        {
                            Id = x.Id,
                            EventDay = x.EventDay,
                            EventStartTime = x.EventStartTime,
                            EventEndTime = x.EventEndTime,
                            ServiceMemberPercentPerDay = x.ServiceMemberPercentPerDay
                        }).ToList()
                        : new List<PostEventStartEndTimeDayWiseDto>(),

                    // ✅ Services mapping (ONLY selected)
                    EventServices = em.EventServiceDetailList != null
                        ? em.EventServiceDetailList
                            .Where(x => x.IsSelected)
                            .Select(x => new PostEventServiceDetailDto
                            {
                                EventServiceDetailId = x.Id,
                                EventService = x.EventService,
                                FinalPreEventConfirmedNumbers = x.FinalPreEventConfirmedNumbers
                            }).ToList()
                        : new List<PostEventServiceDetailDto>(),

                    // ✅ Contract details (FULL mapping restored)
                    ContractDetails = em.ContractDetails == null
                        ? null
                        : new ContractDetails
                        {
                            Id = em.ContractDetails.Id,
                            ContractID = em.ContractDetails.ContractID,
                            ContractName = em.ContractDetails.ContractName,
                            ContractClient = em.ContractDetails.ContractClient,
                            SiteId = em.ContractDetails.SiteId,
                            ContractAgency = em.ContractDetails.ContractAgency,
                            ContractComponent = em.ContractDetails.ContractComponent,
                            ClientName = em.ContractDetails.ClientName,
                            ContractType = em.ContractDetails.ContractType,
                            ContractStartDate = em.ContractDetails.ContractStartDate,
                            ContractEndDate = em.ContractDetails.ContractEndDate,
                            DawsonProjectManagerFirstName = em.ContractDetails.DawsonProjectManagerFirstName,
                            ContractServiceBranch = em.ContractDetails.ContractServiceBranch
                        },

                    // ✅ Event fields (FULL mapping restored)
                    SubEventID = em.SubEventID,
                    EventAddress1 = em.EventAddress1,
                    EventAddress2 = em.EventAddress2,
                    EventState = em.EventState,
                    EventCity = em.EventCity,
                    EventZipCode = em.EventZipCode,
                    MOBDate = em.MOBDate,
                    RegardingSites = em.RegardingSites,
                    Timezone = em.Timezone,
                    EventHelpLine = em.EventHelpLine,

                    TotalServiceMember = em.TotalRequestedServiceMembers,

                    // ✅ Taskforce mapping
                    TaskForce = em.EventManagementTaskforcesList != null
                        ? string.Join(", ", em.EventManagementTaskforcesList.Select(t => t.Taskforce))
                        : null
                };

                _logger.LogInformation("{ClassName}, {MethodName}, DTO prepared successfully. EventManagementId={Id}",
                    CLASSNAME, methodName, eventManagementId);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred. EventManagementId={Id}",
                    CLASSNAME, methodName, eventManagementId);

                throw;
            }
        }


        public async Task<List<EventManagementPreview>> GetAllEventID(bool includeVersion = true)
        {
            const string methodName = "GetAllEventID";

            _logger.LogInformation("{ClassName}, {MethodName}, Called",
                CLASSNAME, methodName);

            try
            {
                // Fetch only the necessary data
                var result = await _unitOfWork.EventManagement
                    .GetAllWithConditionNoTracking(e => e.EventStatus != "Canceled") // filtered in DB
                    .OrderByDescending(e => e.Id)
                    .Select(e => new EventManagementPreview
                    {
                        Id = e.Id,
                        EventID = includeVersion
                            ? $"{e.EventID} (V{e.EventVersion})"
                            : e.EventID
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

                responseDto = Helper.ConvertEventTimesToUtc(eventManagement, eventManagement.Timezone);

                if (!responseDto.Success)
                {
                    return responseDto;
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
                await _unitOfWork.SaveAsync();

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
                // 🔹 1. Load tracked entity with children
                var existing = await _unitOfWork.EventManagement
                    .GetWithIncludeTracking(e => e.Id == updatedModel.Id,
                        e => e.EventServiceDetailList,
                        e => e.EventStartEndTimeDayWiseList,
                        e => e.EventStaffDetailList,
                        e => e.EventManagementTaskforcesList)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Event not found."
                    };
                }

                responseDto = Helper.ConvertEventTimesToUtc(updatedModel, updatedModel.Timezone);

                if (!responseDto.Success) return responseDto;

                if (!ValidateEventStatus(updatedModel, loggedinUserName, true, out string statusError))
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = statusError
                    };
                }

                // 🔹 3. Preserve audit fields
                var addedBy = existing.AddedBy;
                var addedOn = existing.AddedOn;

                // 🔹 4. Update parent properties
                _mapper.Map(updatedModel, existing);
                existing.AddedBy = addedBy;
                existing.AddedOn = addedOn;
                existing.UpdatedBy = loggedinUserName;
                existing.UpdatedOn = DateTime.Now;

                // 🔹 5. Update child collections safely
                Helper.UpdateCollection(existing.EventServiceDetailList, updatedModel.EventServiceDetailList, x => new { x.EventService, x.Type }, _mapper);

                Helper.UpdateCollection(existing.EventStartEndTimeDayWiseList, updatedModel.EventStartEndTimeDayWiseList, x => new { x.EventDay, x.ServiceMemberPercentPerDay },_mapper);

                Helper.UpdateCollection(existing.EventManagementTaskforcesList, updatedModel.EventManagementTaskforcesList, x => x.Taskforce, _mapper);

                existing.EventStaffDetailList.Clear();
                foreach (var staff in updatedModel.EventStaffDetailList)
                {
                    var newStaff = new EventStaffDetail
                    {
                        EventStaffId = staff.EventStaffId,
                        SelectedStation = staff.SelectedStation,
                        PreEventAvailability = staff.PreEventAvailability,
                        ProfileButtonAccess = staff.ProfileButtonAccess,
                        SelectedSecondaryStation = staff.SelectedSecondaryStation,
                        AvailabilityDatesList = staff.AvailabilityDatesList
                            .Select(d => new EventManagementStaffAvailability
                            {
                                AvailabilityDate = d.AvailabilityDate
                            }).ToList(),
                        EventWiseStaffRoleList = staff.EventWiseStaffRoleList
                            .Select(r => new EventWiseStaffRole { RoleId = r.RoleId })
                            .ToList(),
                        EventWiseStaffSecondaryRoleList = staff.EventWiseStaffSecondaryRoleList
                            .Select(r => new EventWiseStaffSecondaryRole { RoleId = r.RoleId })
                            .ToList()
                    };

                    existing.EventStaffDetailList.Add(newStaff);
                }

                // 🔹 7. Save changes once
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
                    "{ClassName}, {MethodName}, Error updating EventID: {EventID}, User: {UserName}",
                    CLASSNAME, methodName, updatedModel?.Id, loggedinUserName);

                responseDto.Success = false;
                responseDto.Message = $"Error updating event: {ex.Message}";
            }

            return responseDto;
        }

        
        private bool IsDefaultKey<TKey>(TKey key)
        {
            if (key == null) return true;

            if (key is int i) return i == 0;
            if (key is long l) return l == 0L;
            if (key is Guid g) return g == Guid.Empty;

            return false;
        }

        public async Task<string> GetNextEventIdNumber()
        {
            const string methodName = "GetNextEventIdNumber";
            _logger.LogInformation("{ClassName}, {MethodName} called.", CLASSNAME, methodName);

            try
            {
                // Step 1: Get all EventIDs from database
                var eventIds = await _unitOfWork.EventManagement
                    .GetAllNoTracking()
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
                string eventStatus = string.Empty;

                var eventManagement = await _unitOfWork.EventManagement.GetWithIncludeNoTracking(
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

                    eventStatus = GetEventStatus(firstEventManagement);//this method will be called before ConvertEventToLocalTime because date is being used before from TUC to localtime convertion
                    Helper.ConvertEventToLocalTime(firstEventManagement, firstEventManagement.Timezone);

                    var contractDetails = await _unitOfWork.ContractDetails.GetByIdAsync(firstEventManagement.ContractId);

                    if (contractDetails == null)
                    {
                        throw new Exception("No contract detail found.");
                    }

                    var eventStaff = await _unitOfWork.EventStaff.GetByIdAsync(firstEventManagement.HIVDropOffStaffId);

                    var EventStaffDetailAndAdditionalRoleslist = new List<EventStaffDetailAndAdditionalRoles>();

                    foreach (var eventStaffDetails in firstEventManagement.EventStaffDetailList)
                    {
                        if (!string.IsNullOrEmpty(eventStaffDetails.SelectedSecondaryStation))
                        {
                            eventStaffDetails.SelectedSecondaryStationList = eventStaffDetails.SelectedSecondaryStation.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                        }

                        EventStaffDetailAndAdditionalRoles eventStaffDetailAndAdditionalRoles = new EventStaffDetailAndAdditionalRoles();
                        //var eventStaffDetail = await _unitOfWork.EventStaff.GetByNullableIdAsync(eventStaffDetails.EventStaffId);
                        var eventStaffDetail = await _unitOfWork.EventStaff.GetWithIncludeNoTracking(
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
                        EventRuntimeStatus = eventStatus,
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
                var eventManagement = await _unitOfWork.EventManagement.GetWithIncludeNoTracking(
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
                var eventManagement = await _unitOfWork.EventManagement.GetFirstOrDefaultWithConditionNoTracking(x => x.Id == id);

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
                var eventManagement = await _unitOfWork.EventManagement.GetFirstOrDefaultWithConditionNoTracking(x => x.EventID == eventId);

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
                    .GetAllWithConditionNoTracking(x => x.Id == eventId)
                    .Select(x => new
                    {
                        x.EventStartDateUtc,
                        x.EventEndDateUtc,
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
                    CLASSNAME, methodName, eventId, result.EventStartDateUtc, result.EventEndDateUtc, result.EventVersion);

                return (result.EventStartDateUtc, result.EventEndDateUtc, result.EventVersion);
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
            const string methodName = nameof(ValidateEventStatus);
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

                // Use full UTC datetime stored in main table
                var eventStartUtc = model.EventStartDateUtc; // already UTC
                var eventEndUtc = model.EventEndDateUtc;     // already UTC
                var nowUtc = DateTime.UtcNow;

                var allowedStatuses = new List<string>();

                if (!isUpdate)
                {
                    allowedStatuses.Add("Initial Pre-event");
                }
                else
                {
                    if (nowUtc < eventStartUtc)
                    {
                        allowedStatuses.AddRange(new[]
                        {
                    "Initial Pre-event",
                    "Pre-Event Confirmed",
                    "Pre-Event Complete",
                    "Canceled"
                });
                    }
                    else if (nowUtc >= eventStartUtc && nowUtc <= eventEndUtc)
                    {
                        allowedStatuses.Add("In Progress");
                    }
                    else // nowUtc > eventEndUtc
                    {
                        allowedStatuses.AddRange(new[]
                        {
                    "In Progress Complete"
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

        public static string GetEventStatus(EventManagement eventManagement)
        {
            if (eventManagement == null)
                throw new ArgumentNullException(nameof(eventManagement));

            if (string.IsNullOrWhiteSpace(eventManagement.Timezone))
                throw new ArgumentException("Event timezone is required.", nameof(eventManagement.Timezone));

            try
            {
                DateTime currentEventTime = DateTime.UtcNow;

                if (currentEventTime < eventManagement.EventStartDateUtc)
                    return "Event Not Started";
                else if (currentEventTime >= eventManagement.EventStartDateUtc && currentEventTime < eventManagement.EventEndDateUtc)
                    return "Event In Progress";
                else
                    return "Event Completed";
            }
            catch
            {
                return "Initialized"; // default if something goes wrong
            }
        }

        public async Task<bool> HasServiceMembersAsync(string eventId)
        {
            return await _unitOfWork.ServiceMembersParent
                .GetAllWithConditionNoTracking(x => x.EventManagement.EventID == eventId &&
                                       !x.isDeleted.GetValueOrDefault())
                .AnyAsync();
        }

        public async Task<List<FileDataDto>> GetServiceMembersByEventAsync(long eventId)
        {
            string methodName = nameof(GetServiceMembersByEventAsync);

            try
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Fetching data for EventId: {EventId}",
                    CLASSNAME, methodName, eventId);

                var query = _unitOfWork.EventManagement
    .GetWithIncludeNoTracking(em => em.Id == eventId);

                var data = await query
                    .SelectMany(em => em.ServiceMembersParents
                        .Where(parent => !(parent.isDeleted ?? false)) // filter out deleted parents
                        .SelectMany(parent => parent.ServiceMembersChildren
                            .Select(c => new FileDataDto
                            {
                                Id = c.Id,

                                // Parent
                                VisionWin = parent.VisionWin,
                                DentalWin = parent.DentalWin,
                                PhaWin = parent.PhaWin,
                                HivWin = parent.HivWin,
                                HearingWin = parent.HearingWin,
                                isDeleted = parent.isDeleted,

                                // Event
                                EventId = em.EventID,

                                // Child
                                SmId = c.SmId,
                                FullName = c.FullName,
                                FullSsn = c.FullSsn,
                                Last4 = c.Last4,
                                DodId = c.DodId,
                                Rank = c.Rank,
                                Age = c.Age,
                                Sex = c.Sex,
                                Mos = c.Mos,
                                Agr = c.Agr,
                                Uic = c.Uic,
                                Mrc = c.Mrc,
                                Dob = c.Dob,
                                Over40 = c.Over40,

                                DentalDue = c.DentalDue,
                                DentalExam = c.DentalExam,
                                DentalNeeded = c.DentalNeeded,
                                PanoNeeded = c.PanoNeeded,
                                BwxNeeded = c.BwxNeeded,
                                Drc = c.Drc,

                                PhaDate = c.PhaDate,
                                PhaDue = c.PhaDue,
                                Pha = c.Pha,
                                Pulhes = c.Pulhes,

                                VisionDate = c.VisionDate,
                                Vision = c.Vision,
                                NearVision = c.NearVision,
                                Vrc = c.Vrc,
                                Vision2pg = c.Vision2pg,
                                Vision1mi = c.Vision1mi,

                                HearingDate = c.HearingDate,
                                Hearing = c.Hearing,
                                Hrc = c.Hrc,
                                HearingProfile = c.HearingProfile,

                                Quest = c.Quest,
                                LabNeeded = c.LabNeeded,

                                Abo = c.Abo,
                                AboNeeded = c.AboNeeded,
                                Dna = c.Dna,

                                SickleDate = c.SickleDate,
                                Sickle = c.Sickle,
                                G6pd = c.G6pd,
                                G6pdDate = c.G6pdDate,
                                G6pdStatus = c.G6pdStatus,

                                HivNextTestDate = c.HivNextTestDate,
                                Hiv = c.Hiv,

                                LipidNeeded = c.LipidNeeded,
                                LipidPanel = c.LipidPanel,
                                CholesterolHdlCholesterol = c.CholesterolHdlCholesterol,
                                Framingham = c.Framingham,

                                Ekg = c.Ekg,
                                EkgNeeded = c.EkgNeeded,
                                PregnancyTestNeeded = c.PregnancyTestNeeded,

                                Imm = c.Imm,
                                HepB = c.HepB,
                                HepA = c.HepA,
                                Flu = c.Flu,
                                TetTdp = c.TetTdp,
                                Mmr = c.Mmr,
                                Varicella = c.Varicella,

                                TaskForce = c.TaskForce,
                                Notes = c.Notes,
                                Over44 = c.Over44,

                                EventDate = c.EventDate,
                                EventEndDate = c.EventEndDate,

                                CheckIn = c.CheckIn,
                                CheckInBy = c.CheckInBy,
                                CheckInTime = c.CheckInTime,
                                CheckOut = c.CheckOut,
                                CheckOutBy = c.CheckOutBy,
                                CheckOutTime = c.CheckOutTime,

                                WalkInServiceMember = c.WalkInServiceMember,
                                Barcode = c.Barcode,

                                // Navigation properties
                                ImmunizationRecord = c.ImmunizationRecord,
                                LabStationRecord = c.LabStationRecord
                            })
                        )
                    )
                    .ToListAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Fetched {Count} records for EventId: {EventId}",
                    CLASSNAME, methodName, data.Count, eventId);

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error fetching data for EventId: {EventId}",
                    CLASSNAME, methodName, eventId);
                throw;
            }
        }
    }
}

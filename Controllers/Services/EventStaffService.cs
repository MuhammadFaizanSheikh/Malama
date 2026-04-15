using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Malama.Utilities;
using AutoMapper;
using ExcelFilesCompiler.Utilities;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class EventStaffService : IEventStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAccountRegistrationService _registrationService;
        private readonly IRoleService _roleService;
        private readonly ISubmissionTokenService _submissionTokenService;
        private readonly ILogger<EventStaffService> _logger;
        private const string CLASSNAME = "EventStaffService";

        public EventStaffService(ILogger<EventStaffService> logger, IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IAccountRegistrationService registrationService, RoleManager<ApplicationRole> roleManager, IRoleService roleService, ISubmissionTokenService submissionTokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _registrationService = registrationService;
            _roleManager = roleManager;
            _roleService = roleService;
            _submissionTokenService = submissionTokenService;
            _logger = logger;
        }

        public async Task<ResponseDto> AddEventStaffAsync(EventStaff eventStaff, string submissionToken, string loggedinUserName)
        {
            const string methodName = "AddContractAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called by User: {UserName}, Email: {Email}",
                CLASSNAME, methodName, loggedinUserName, eventStaff?.UserEmail);

            var responseDto = new ResponseDto();
            bool userCreated = false;
            ApplicationUser createdUser = null;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var tokenResult = await _submissionTokenService.ValidateAndSaveAsync(submissionToken, loggedinUserName);

                    if (!tokenResult.Success)
                    {
                        _logger.LogWarning("{ClassName}, {MethodName}, Submission token invalid. User: {UserName}", CLASSNAME, methodName, loggedinUserName);
                        return tokenResult;
                    }

                    foreach (var affiliation in eventStaff.StaffContractAffiliation)
                    {
                        var subContractor = await _unitOfWork.SubContractors.GetFirstOrDefaultWithConditionNoTracking(c => c.ContractId == affiliation.ContractId && c.CompanyMainName == affiliation.SubContractorName);
                        affiliation.SubContractorId = subContractor?.Id ?? 0;

                        _logger.LogInformation("{ClassName}, {MethodName}, Affiliation mapped. ContractId: {ContractId}, SubContractorId: {SubContractorId}",
                            CLASSNAME, methodName, affiliation.ContractId, affiliation.SubContractorId);
                    }

                    eventStaff.AddedBy = loggedinUserName;
                    eventStaff.AddedOn = DateTime.Now;

                    var rvm = new RegisterViewModel
                    {
                        Email = eventStaff.UserEmail,
                        Password = eventStaff.UserPassword,
                        SelectedRoles = new List<string>()
                    };

                    responseDto = await _registrationService.RegisterUserAsync(rvm, true);

                    if (responseDto.Success)
                    {
                        createdUser = responseDto.Data?.GetType().GetProperty("user")?.GetValue(responseDto.Data) as ApplicationUser;

                        if (createdUser != null)
                        {
                            eventStaff.UserId = createdUser.Id;
                            userCreated = true;
                            _logger.LogInformation("{ClassName}, {MethodName}, User created successfully. UserID: {UserId}", CLASSNAME, methodName, createdUser.Id);
                        }

                        await _unitOfWork.EventStaff.AddAsync(eventStaff);
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("{ClassName}, {MethodName}, Event Staff added successfully. StaffID: {StaffId}, AddedBy: {UserName}",
                            CLASSNAME, methodName, eventStaff.Id, loggedinUserName);

                        return new ResponseDto { Success = true, Message = "Event Staff added successfully!" };
                    }
                    else
                    {
                        _logger.LogWarning("{ClassName}, {MethodName}, User registration failed. Message: {Message}", CLASSNAME, methodName, responseDto.Message);
                        return new ResponseDto { Success = false, Message = responseDto.Message };
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    if (userCreated && createdUser != null)
                    {
                        await _userManager.DeleteAsync(createdUser);
                        _logger.LogInformation("{ClassName}, {MethodName}, Rolled back user creation. UserID: {UserId}", CLASSNAME, methodName, createdUser.Id);
                    }

                    _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while adding Event Staff. User: {UserName}", CLASSNAME, methodName, loggedinUserName);
                    return new ResponseDto { Success = false, Message = $"An error occurred: {ex.Message}" };
                }
            }
        }


        public async Task<List<EventStaff>> GetAllEventStaff()
        {
            const string methodName = "GetAllEventStaff";
            _logger.LogInformation("{ClassName}, {MethodName}, Fetching all EventStaff.", CLASSNAME, methodName);

            List<EventStaff> eventStaff = new List<EventStaff>();

            try
            {
                eventStaff = _unitOfWork.EventStaff.GetAllNoTracking().OrderByDescending(c => c.Id).ToList();
                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {Count} EventStaff records.", CLASSNAME, methodName, eventStaff.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while fetching EventStaff.", CLASSNAME, methodName);
                throw;
            }

            return eventStaff;
        }

        public async Task<List<CombinedEventStaffRolesNameAndLicense>> GetAllEventStaffWithRolesAndLicenses()
        {
            const string methodName = "GetAllEventStaffWithRolesAndLicenses";
            _logger.LogInformation("{ClassName}, {MethodName}, Fetching all EventStaff with roles and licenses.", CLASSNAME, methodName);

            try
            {
                var eventStaffList = await _unitOfWork.EventStaff.GetWithIncludeNoTracking()
                    .Include(x => x.StaffQualification)
                        .ThenInclude(l => l.StaffLicenseDetails)
                    .Include(x => x.StaffContractAffiliation)
                    .Include(x => x.TravelHonorList)
                    .Include(x => x.StaffQualification)
                        .ThenInclude(l => l.StaffAttributeDetails)
                    .ToListAsync();

                if (!eventStaffList.Any())
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, No Event Staff records found.", CLASSNAME, methodName);
                    return new List<CombinedEventStaffRolesNameAndLicense>();
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {Count} EventStaff records.", CLASSNAME, methodName, eventStaffList.Count);

                // Fetch completed events
                var completedEventList = _unitOfWork.EventManagement.GetAllWithConditionNoTracking(c => c.EventStatus == AppConstants.EventStatus.InProgressComplete).ToList();
                var eventIds = completedEventList.Select(e => e.Id).ToList();

                _logger.LogInformation("{ClassName}, {MethodName}, Found {Count} completed events.", CLASSNAME, methodName, eventIds.Count);

                // Map staff to completed event counts
                var groupedResult = new Dictionary<long, int>();

                foreach (var eventId in eventIds)
                {
                    var eventStaffDetailList = await _unitOfWork.EventStaffDetail
                        .GetWithIncludeNoTracking()
                        .Where(esd => esd.EventManagementId == eventId)
                        .ToListAsync();

                    foreach (var staff in eventStaffDetailList)
                    {
                        if (groupedResult.ContainsKey(staff.EventStaffId))
                        {
                            groupedResult[staff.EventStaffId]++;
                        }
                        else
                        {
                            groupedResult[staff.EventStaffId] = 1;
                        }
                    }
                }

                var roles = await _roleManager.Roles.ToListAsync();
                var roleDictionary = roles.ToDictionary(r => r.Id, r => r.Name);

                var model = new List<CombinedEventStaffRolesNameAndLicense>();

                foreach (var staff in eventStaffList)
                {
                    var roleLicenseMapping = new Dictionary<string, List<string>>();
                    var attributeList = new List<string>();

                    foreach (var staffLicense in staff.StaffQualification)
                    {
                        string qualificationName = staffLicense.QualificationName;
                        roleLicenseMapping[qualificationName] = new List<string>();

                        foreach (var licenseDetail in staffLicense.StaffLicenseDetails)
                        {
                            roleLicenseMapping[qualificationName].Add($"{licenseDetail.LicenseState}: {licenseDetail.LicenseType}");
                        }

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

                    // Format roles, licenses, attributes
                    var rolesString = string.Join(", ", roleLicenseMapping.Keys);
                    var licensesString = string.Join("<br/>", roleLicenseMapping.Select(kv => string.Join(", ", kv.Value)));
                    var attributesString = string.Join(", ",
                        attributeList
                            .Select(a => a.Trim())
                            .Where(a => !string.IsNullOrWhiteSpace(a))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(a => a));

                    int completedEventCount = groupedResult.ContainsKey(staff.Id) ? groupedResult[staff.Id] : 0;

                    model.Add(new CombinedEventStaffRolesNameAndLicense
                    {
                        Id = staff.Id,
                        StaffID = staff.StaffID,
                        StaffLastName = staff.StaffLastName,
                        StaffFirstName = staff.StaffFirstName,
                        PrimaryCity = staff.PrimaryCity,
                        PrimaryState = staff.PrimaryState,
                        PrimaryZip = staff.PrimaryZip,
                        StaffCAC = staff.StaffCAC,
                        Roles = rolesString,
                        LicenseStateAndTypes = licensesString,
                        Status = staff.StaffStatus,
                        CountOfStaffEnrolledInEvent = completedEventCount,
                        Attributes = attributesString
                    });
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Prepared final combined EventStaff model with {Count} entries.", CLASSNAME, methodName, model.Count);

                return model;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "{ClassName}, {MethodName}, No Event Staff records found.", CLASSNAME, methodName);
                return new List<CombinedEventStaffRolesNameAndLicense>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected error while fetching Event Staff with roles and licenses.", CLASSNAME, methodName);
                throw new Exception("An internal error occurred while processing your request.", ex);
            }
        }

        public async Task<CombinedEventStaffSubContractorAndContractDto> GetEventStaffById(long id)
        {
            const string methodName = "GetEventStaffById";
            _logger.LogInformation("{ClassName}, {MethodName}, Fetching EventStaff with ID {EventStaffId}", CLASSNAME, methodName, id);

            try
            {
                var eventStaff = await _unitOfWork.EventStaff.GetWithIncludeNoTracking(
                        x => x.Id == id,
                        x => x.StaffQualification,
                        x => x.StaffContractAffiliation,
                        x => x.TravelHonorList
                    )
                    .Include(x => x.StaffQualification)
                        .ThenInclude(l => l.StaffLicenseDetails)
                    .Include(x => x.StaffQualification)
                        .ThenInclude(l => l.StaffAttributeDetails)
                    .FirstOrDefaultAsync();

                if (eventStaff == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, EventStaff with ID {EventStaffId} not found.", CLASSNAME, methodName, id);
                    throw new Exception($"EventStaff with ID {id} not found.");
                }

                if (eventStaff.StaffContractAffiliation == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, EventStaff with ID {EventStaffId} has no contract affiliations.", CLASSNAME, methodName, id);
                    throw new Exception($"StaffContractAffiliation for EventStaff with ID {id} not found.");
                }

                var subContractorAffiliations = new List<StaffSubContractorAffiliationDto>();

                foreach (var affiliation in eventStaff.StaffContractAffiliation)
                {
                    var contract = await _unitOfWork.ContractDetails.GetByIdAsync(affiliation.ContractId);
                    var subContractor = await _unitOfWork.SubContractors.GetByIdAsync(affiliation.SubContractorId);

                    if (contract == null)
                    {
                        _logger.LogWarning("{ClassName}, {MethodName}, Contract with ID {ContractId} not found.", CLASSNAME, methodName, affiliation.ContractId);
                        throw new Exception($"Contract with ID {affiliation.ContractId} not found.");
                    }

                    if (subContractor == null)
                    {
                        _logger.LogWarning("{ClassName}, {MethodName}, SubContractor with ID {SubContractorId} not found.", CLASSNAME, methodName, affiliation.SubContractorId);
                        throw new Exception($"SubContractor with ID {affiliation.SubContractorId} not found.");
                    }

                    var contractDto = new StaffContractAffiliationDto
                    {
                        ContractId = contract.Id,
                        ContractName = contract.ContractName
                    };

                    var existingSubContractorDto = subContractorAffiliations
                        .FirstOrDefault(x => x.SubContractorId == subContractor.Id);

                    if (existingSubContractorDto == null)
                    {
                        subContractorAffiliations.Add(new StaffSubContractorAffiliationDto
                        {
                            SubContractorId = subContractor.Id,
                            SubContractorName = subContractor.CompanyMainName,
                            StaffContractAffiliation = new List<StaffContractAffiliationDto> { contractDto }
                        });
                    }
                    else
                    {
                        existingSubContractorDto.StaffContractAffiliation.Add(contractDto);
                    }
                }

                var combinedDto = new CombinedEventStaffSubContractorAndContractDto
                {
                    EventStaff = eventStaff,
                    StaffSubContractorAffiliation = subContractorAffiliations,
                    TravelHonor = eventStaff.TravelHonorList
                };

                _logger.LogInformation("{ClassName}, {MethodName}, Successfully retrieved EventStaff with ID {EventStaffId}.", CLASSNAME, methodName, id);

                return combinedDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error retrieving EventStaff with ID {EventStaffId}.", CLASSNAME, methodName, id);
                throw new Exception("An error occurred while retrieving the EventStaff.", ex);
            }
        }

        public async Task<EventStaff> GetEventStaffWithoutIncludeById(long id)
        {
            try
            {
                var eventStaff = await _unitOfWork.EventStaff.GetByIdAsync(id);

                if (eventStaff == null)
                {
                    throw new KeyNotFoundException($"EventStaff with ID {id} not found.");
                }

                return eventStaff;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving EventStaff without includes for ID {EventStaffId}", id);
                throw new Exception("An error occurred while retrieving the EventStaff.", ex);
            }
        }

        //public async Task<ResponseDto> UpdateEventStaffAsync(EventStaff eventStaff, string loggedinUserName)
        //{
        //    var responseDto = new ResponseDto();

        //    using var transaction = await _unitOfWork.BeginTransactionAsync();

        //    try
        //    {
        //        // Update sub-contractor IDs
        //        foreach (var affiliation in eventStaff.StaffContractAffiliation)
        //        {
        //            var subContractor = await _unitOfWork.SubContractors.GetFirstOrDefaultWithConditionNoTracking(
        //                c => c.ContractId == affiliation.ContractId && c.CompanyMainName == affiliation.SubContractorName
        //            );

        //            if (subContractor != null)
        //                affiliation.SubContractorId = subContractor.Id;
        //        }

        //        // Preserve original metadata
        //        var existingEvent = await _unitOfWork.EventStaff.GetByIdAsync(eventStaff.Id);
        //        eventStaff.AddedBy = existingEvent.AddedBy;
        //        eventStaff.AddedOn = existingEvent.AddedOn;
        //        eventStaff.UpdatedBy = loggedinUserName;
        //        eventStaff.UpdatedOn = DateTime.Now;
        //        eventStaff.UserId = existingEvent.UserId;

        //        // Update EventStaff entity
        //        await _unitOfWork.EventStaff.UpdateAsync(eventStaff);

        //        // Refresh StaffQualification
        //        await _unitOfWork.StaffQualification.DeleteAgainstFieldAsync(eventStaff.Id, "EventStaffId");
        //        foreach (var license in eventStaff.StaffQualification)
        //        {
        //            license.EventStaffId = eventStaff.Id;
        //        }
        //        await _unitOfWork.StaffQualification.AddRangeAsync(eventStaff.StaffQualification);

        //        // Refresh StaffContractAffiliation
        //        await _unitOfWork.StaffContractAffiliation.DeleteAgainstFieldAsync(eventStaff.Id, "EventStaffId");
        //        foreach (var affiliation in eventStaff.StaffContractAffiliation)
        //        {
        //            affiliation.EventStaffId = eventStaff.Id;
        //        }
        //        await _unitOfWork.StaffContractAffiliation.AddRangeAsync(eventStaff.StaffContractAffiliation);

        //        // Refresh TravelHonorList
        //        await _unitOfWork.TravelHonor.DeleteAgainstFieldAsync(eventStaff.Id, "EventStaffId");
        //        foreach (var travelHonor in eventStaff.TravelHonorList)
        //        {
        //            travelHonor.EventStaffId = eventStaff.Id;
        //        }
        //        await _unitOfWork.TravelHonor.AddRangeAsync(eventStaff.TravelHonorList);

        //        // Update related user roles / identity if needed
        //        var result = await UpdateUser(eventStaff); // Or _roleService.UpdateUserEventStaffRolesAsync(eventStaff);

        //        if (!result.Success)
        //        {
        //            await transaction.RollbackAsync();
        //            return result;
        //        }

        //        await _unitOfWork.SaveAsync();
        //        await transaction.CommitAsync();

        //        responseDto.Success = true;
        //        responseDto.Message = "EventStaff updated successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Error updating EventStaff with ID {EventStaffId}", eventStaff.Id);
        //        responseDto.Success = false;
        //        responseDto.Message = $"An error occurred while updating contract: {ex.Message}";
        //    }

        //    return responseDto;
        //}

        public async Task<ResponseDto> UpdateEventStaffAsync(EventStaff updatedStaff, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1️⃣ Load tracked entity with related child collections
                var existing = await _unitOfWork.EventStaff
                    .GetWithIncludeTracking(
                        e => e.Id == updatedStaff.Id,
                        e => e.StaffQualification,
                        e => e.StaffContractAffiliation,
                        e => e.TravelHonorList)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "EventStaff not found."
                    };
                }

                foreach (var affiliation in updatedStaff.StaffContractAffiliation)
                {
                    var subContractor = await _unitOfWork.SubContractors
                        .GetFirstOrDefaultWithConditionNoTracking(
                            c => c.ContractId == affiliation.ContractId && c.CompanyMainName == affiliation.SubContractorName
                        );

                    if (subContractor != null)
                        affiliation.SubContractorId = subContractor.Id;
                }

                string addedBy = existing.AddedBy;
                DateTime addedOn = existing.AddedOn;

                
                //updatedStaff.UserId = existing.UserId;

                _mapper.Map(updatedStaff, existing);

                existing.AddedBy = addedBy;
                existing.AddedOn = addedOn;
                existing.UpdatedBy = loggedinUserName;
                existing.UpdatedOn = DateTime.Now;

                //Helper.UpdateCollection(existing.StaffQualification, updatedStaff.StaffQualification, x => x.Id, _mapper);

                Helper.UpdateCollection(existing.StaffQualification, updatedStaff.StaffQualification, x => x.QualificationId,_mapper,
                (existingItem, updatedItem) =>
                {
                    Helper.UpdateCollection(
                        existingItem.StaffAttributeDetails,
                        updatedItem.StaffAttributeDetails,
                        x => x.Attribute,   // ✅ FIX
                        _mapper);

                    Helper.UpdateCollection(
                        existingItem.StaffLicenseDetails,
                        updatedItem.StaffLicenseDetails,
                        x => new { x.LicenseNumber, x.LicenseType, x.LicenseState }, // ✅ FIX
                        _mapper);
                });
                Helper.UpdateCollection(existing.StaffContractAffiliation, updatedStaff.StaffContractAffiliation, x => new { x.ContractId, x.SubContractorId, x.SubContractorName },_mapper);
                Helper.UpdateCollection(existing.TravelHonorList, updatedStaff.TravelHonorList, x => new { x.Type, x.Name, x.Rewards }, _mapper);

                var userUpdateResult = await UpdateUser(existing);

                if (!userUpdateResult.Success)
                {
                    await transaction.RollbackAsync();
                    return userUpdateResult;
                }

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                responseDto.Success = true;
                responseDto.Message = "EventStaff updated successfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating EventStaff with ID {EventStaffId}", updatedStaff.Id);
                responseDto.Success = false;
                responseDto.Message = $"An error occurred while updating EventStaff: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<string> GetNextStaffId()
        {
            try
            {
                var allEventStaff = _unitOfWork.EventStaff.GetAllNoTracking();

                if (allEventStaff == null || !allEventStaff.Any())
                {
                    return "0001"; // Default starting code
                }

                var lastEventStaff = allEventStaff
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                var staffId = lastEventStaff.StaffID;

                if (string.IsNullOrEmpty(staffId) || staffId.Length < 4)
                    throw new Exception("Invalid last StaffID format.");

                var numericPart = int.Parse(staffId.Substring(staffId.Length - 4)); // Last 4 digits
                numericPart++;

                return numericPart.ToString("D4");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching next StaffID");
                throw new Exception("Error while fetching the next StaffID.", ex);
            }
        }

        public IQueryable<EventStaff> GetEventStaffForSearchingByStaffId(string staffId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(staffId))
                {
                    return _unitOfWork.EventStaff.GetAllWithConditionNoTracking(c => true);
                }

                return _unitOfWork.EventStaff.GetAllWithConditionNoTracking(
                    c => c.StaffID.ToLower().Contains(staffId.Trim().ToLower())
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching EventStaff for searching with StaffID {StaffID}", staffId);
                throw new Exception("Error while fetching contract details.", ex);
            }
        }

        public async Task<EventStaff> GetEventStaffWithAttributesByUserId(string userId)
        {
            try
            {
                var eventStaff = await _unitOfWork.EventStaff.GetWithIncludeNoTracking()
                    .Include(es => es.StaffQualification)
                        .ThenInclude(sl => sl.StaffAttributeDetails)
                    .FirstOrDefaultAsync(es => es.UserId == userId);

                if (eventStaff == null)
                    throw new KeyNotFoundException($"EventStaff with UserId {userId} not found.");

                return eventStaff;
            }
            catch (KeyNotFoundException)
            {
                throw; // preserve KeyNotFound for controller handling
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving EventStaff with attributes for UserId {UserId}", userId);
                throw new ApplicationException("An error occurred while retrieving event staff and its attributes.", ex);
            }
        }

        public async Task<bool> CheckSSNExistsAsync(string ssn)
        {
            if (string.IsNullOrWhiteSpace(ssn))
                return false;

            try
            {
                var staff = await _unitOfWork.EventStaff.GetFirstOrDefaultWithConditionNoTracking(es => es.StaffSSN == ssn);
                return staff != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking SSN existence: {SSN}", ssn);
                return false;
            }
        }

        private async Task<ResponseDto> UpdateUser(EventStaff eventStaff)
        {
            try
            {
                var responseDto = new ResponseDto();
                var user = await _userManager.FindByIdAsync(eventStaff.UserId);

                if (user == null)
                {
                    responseDto.Success = true;
                    responseDto.Message = "User not found!";
                    return responseDto;
                }

                var userUpdateDto = new UserUpdateDto
                {
                    Id = eventStaff.UserId,
                    Email = eventStaff.UserEmail,
                    SelectedRoles = eventStaff.StaffQualification
                        .Select(l => l.QualificationName)
                        .ToList()
                };

                return await _registrationService.UpdateUserAsync(userUpdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user for EventStaffId {EventStaffId}", eventStaff.Id);
                return new ResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating user. Please try again later or contact your administrator."
                };
            }
        }

        public async Task<List<EventStaffDetail>> GetAllEventStaffByEventId(long eventId)
        {
            try
            {
                return await _unitOfWork.EventStaffDetail
                    .GetWithIncludeNoTracking()
                    .Where(x => x.EventManagementId == eventId)
                    .Include(x => x.EventStaff)
                        .ThenInclude(s => s.StaffQualification)
                            .ThenInclude(q => q.StaffAttributeDetails)
                    .Include(x => x.EventWiseStaffRoleList)
                    .Include(x => x.EventWiseStaffSecondaryRoleList)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving EventStaffDetail for EventId {EventId}", eventId);
                throw new Exception("An error occurred while retrieving the EventStaffDetail.", ex);
            }
        }
    }
}

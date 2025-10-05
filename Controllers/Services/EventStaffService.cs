using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Intrinsics.Arm;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class EventStaffService : IEventStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAccountRegistrationService _registrationService;
        private readonly IRoleService _roleService;

        public EventStaffService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IAccountRegistrationService registrationService, RoleManager<ApplicationRole> roleManager, IRoleService roleService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _registrationService = registrationService;
            _roleManager = roleManager;
            _roleService = roleService;
        }

        public async Task<ResponseDto> AddContractAsync(EventStaff eventStaff, string loggedinUserName)
        {
            var responseDto = new ResponseDto();
            bool userCreated = false; // Track if user was created
            ApplicationUser createdUser = null; // Store the created user for deletion if needed

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    foreach (var affiliation in eventStaff.StaffContractAffiliation)
                    {
                        var subContractor = await _unitOfWork.SubContractors.FindAsync(c => c.ContractId == affiliation.ContractId && c.CompanyMainName == affiliation.SubContractorName);
                        affiliation.SubContractorId = subContractor?.Id ?? 0;
                    }

                    eventStaff.AddedBy = loggedinUserName;
                    eventStaff.AddedOn = DateTime.Now;

                    RegisterViewModel rvm = new RegisterViewModel
                    {
                        Email = eventStaff.UserEmail,
                        Password = eventStaff.UserPassword,
                        SelectedRoles = eventStaff.StaffLicense?.Select(l => l.RoleId).ToList() ?? new List<string>()
                    };

                    if (!rvm.SelectedRoles.Any())
                    {
                        return new ResponseDto { Success = false, Message = "Role not selected." };
                    }

                    responseDto = await _registrationService.RegisterUserAsync(rvm, true);

                    if (responseDto.Success)
                    {
                        createdUser = responseDto.Data?.GetType().GetProperty("user")?.GetValue(responseDto.Data) as ApplicationUser;

                        if (createdUser != null)
                        {
                            eventStaff.UserId = createdUser.Id;
                            userCreated = true; // Mark that user was successfully created
                        }

                        await _unitOfWork.EventStaff.AddAsync(eventStaff);
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync(); // ✅ Commit only if everything is successful

                        return new ResponseDto { Success = true, Message = "Event Staff added successfully!" };
                    }
                    else
                    {
                        return new ResponseDto { Success = false, Message = responseDto.Message };
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    // ✅ Delete the user only if it was actually created
                    if (userCreated && createdUser != null)
                    {
                        await _userManager.DeleteAsync(createdUser);
                    }

                    return new ResponseDto { Success = false, Message = $"An error occurred: {ex.Message}" };
                }
            }
        }

        public async Task<List<EventStaff>> GetAllEventStaff()
        {
            var responseDto = new ResponseDto();
            List<EventStaff> eventStaff = new List<EventStaff>(); // Initialize contracts outside try-catch

            try
            {
                eventStaff = (await _unitOfWork.EventStaff.GetAllAsync()).OrderByDescending(c => c.Id).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

            return eventStaff;
        }

        public async Task<List<CombinedEventStaffRolesNameAndLicense>> GetAllEventStaffWithRolesAndLicenses()
        {
            try
            {
                var eventStaffList = await _unitOfWork.EventStaff.GetWithInclude()
                    .Include(x => x.StaffLicense)
                        .ThenInclude(l => l.StaffLicenseDetails)
                    .Include(x => x.StaffContractAffiliation)
                    .Include(x => x.TravelHonorList)
                    .Include(x => x.StaffLicense)
                        .ThenInclude(l => l.StaffAttributeDetails)
                    .ToListAsync();

                if (eventStaffList == null || !eventStaffList.Any())
                {
                    throw new KeyNotFoundException("No Event Staff records found.");
                }

                // ✅ Fetch Completed Events first and ensure it's a List
                var completedEventList = (await _unitOfWork.EventManagement.FindForSearchingAsync(c => c.EventStatus == "Complete")).ToList();

                // ✅ Extract only Event IDs
                var eventIds = completedEventList.Select(e => e.Id).ToList();


                var groupedResult = new Dictionary<long, int>(); // Dictionary to store StaffId and CompletedEventCount

                foreach (var eventId in eventIds)
                {
                    var eventStaffDetailList = await _unitOfWork.EventStaffDetail
                        .GetWithInclude()
                        .Where(esd => esd.EventManagementId == eventId)
                        .ToListAsync(); // Get records for this Event ID

                    foreach (var staff in eventStaffDetailList)
                    {
                        if (groupedResult.ContainsKey(staff.EventStaffId))
                        {
                            groupedResult[staff.EventStaffId]++; // Increment count if staff exists
                        }
                        else
                        {
                            groupedResult[staff.EventStaffId] = 1; // Initialize count
                        }
                    }
                }

                var roles = await _roleManager.Roles.ToListAsync();
                var roleDictionary = roles.ToDictionary(r => r.Id, r => r.Name);
                var attributeList = new List<string>();

                List<CombinedEventStaffRolesNameAndLicense> model = new List<CombinedEventStaffRolesNameAndLicense>();

                foreach (var staff in eventStaffList)
                {
                    var roleLicenseMapping = new Dictionary<string, List<string>>();

                    foreach (var staffLicense in staff.StaffLicense)
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

                    // Generate formatted strings
                    var rolesString = string.Join(", ", roleLicenseMapping.Keys); // Comma-separated roles
                    var licensesString = string.Join("<br/>", roleLicenseMapping.Select(kv => string.Join(", ", kv.Value))); // Line-separated licenses per role
                    var attributesString = string.Join(", ", attributeList.OrderBy(a => a));

                    int completedEventCount = groupedResult.ContainsKey(staff.Id) ? groupedResult[staff.Id] : 0;

                    // Create a new instance for each staff
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
                        Roles = rolesString,  // Roles in one line, comma-separated
                        LicenseStateAndTypes = licensesString,
                        Status = staff.StaffStatus,
                        CountOfStaffEnrolledInEvent = completedEventCount,
                        Attributes = attributesString
                    });
                }

                return model;
            }
            catch (KeyNotFoundException ex)
            {
                return new List<CombinedEventStaffRolesNameAndLicense>(); // Return empty list
            }
            catch (Exception ex)
            {
                throw new Exception("An internal error occurred while processing your request.");
            }
        }

        public async Task<CombinedEventStaffSubContractorAndContractDto> GetEventStaffById(long id)
        {
            try
            {
                var eventStaff = await _unitOfWork.EventStaff.GetWithInclude(
                    x => x.Id == id,
                    x => x.StaffLicense,
                    x => x.StaffContractAffiliation,
                    x => x.TravelHonorList
                )
                    .Include(x => x.StaffLicense)
                        .ThenInclude(l => l.StaffLicenseDetails).Include(x => x.StaffLicense).ThenInclude(l => l.StaffAttributeDetails) // Now second-level include works!
                    .FirstOrDefaultAsync();


                if (eventStaff != null)
                {
                    var firstEventStaff = eventStaff;

                    if (firstEventStaff == null)
                    {
                        throw new Exception($"EventStaff with ID {id} not found.");
                    }

                    if (firstEventStaff.StaffContractAffiliation == null)
                    {
                        throw new Exception($"EventStaff Staff Contract Affiliation with ID {id} not found.");
                    }

                    var result = new List<StaffSubContractorAffiliationDto>();

                    foreach (var info in firstEventStaff.StaffContractAffiliation)
                    {
                        var filteredContracts = await _unitOfWork.ContractDetails.GetByIdAsync(info.ContractId);
                        var filteredSubContractor = await _unitOfWork.SubContractors.GetByIdAsync(info.SubContractorId);

                        if (filteredContracts == null)
                        {
                            throw new Exception($"SubContractor not found for EventStaff with ID {id}.");
                        }

                        if (filteredSubContractor == null)
                        {
                            throw new Exception($"SubContracts against ContractIds with ID {id} not found.");
                        }

                        var contractAffilication = new StaffContractAffiliationDto
                        {
                            ContractId = info.ContractId,
                            ContractName = filteredContracts.ContractName // Assuming ContractID is the name for contract
                        };

                        var subContractorAffiliation = result.FirstOrDefault(x => x.SubContractorName == filteredSubContractor.CompanyMainName);

                        if (subContractorAffiliation == null)
                        {
                            subContractorAffiliation = new StaffSubContractorAffiliationDto
                            {
                                SubContractorId = info.SubContractorId,
                                SubContractorName = filteredSubContractor.CompanyMainName,
                                StaffContractAffiliation = new List<StaffContractAffiliationDto> { contractAffilication }
                            };

                            result.Add(subContractorAffiliation);
                        }
                        else
                        {
                            subContractorAffiliation.StaffContractAffiliation.Add(contractAffilication);
                        }
                    }

                    var combinedDto = new CombinedEventStaffSubContractorAndContractDto
                    {
                        EventStaff = firstEventStaff,
                        StaffSubContractorAffiliation = result,
                        TravelHonor = firstEventStaff.TravelHonorList
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

        public async Task<EventStaff> GetEventStaffWithoutIncludeById(long id)
        {
            try
            {
                var eventStaff = await _unitOfWork.EventStaff.GetByIdAsync(id);

                if (eventStaff != null)
                {
                    return eventStaff;
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



        public async Task<ResponseDto> UpdateContract(EventStaff eventStaff, string loggedinUserName)
        {
            var responseDto = new ResponseDto();
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var affiliation in eventStaff.StaffContractAffiliation)
                {
                    var subContractor = await _unitOfWork.SubContractors.FindAsync(c => c.ContractId == affiliation.ContractId && c.CompanyMainName == affiliation.SubContractorName);
                    affiliation.SubContractorId = subContractor.Id;
                }

                var existingEvent = await _unitOfWork.EventStaff.GetByIdAsync(eventStaff.Id);
                eventStaff.AddedBy = existingEvent.AddedBy;
                eventStaff.AddedOn = existingEvent.AddedOn;
                eventStaff.UpdatedBy = loggedinUserName;
                eventStaff.UpdatedOn = DateTime.Now;
                eventStaff.UserId = existingEvent.UserId;
                await _unitOfWork.EventStaff.UpdateAsync(eventStaff);

                await _unitOfWork.StaffLicense.DeleteAgainstFieldAsync(eventStaff.Id, "EventStaffId");

                foreach (var license in eventStaff.StaffLicense)
                {
                    license.EventStaffId = eventStaff.Id;
                }

                _unitOfWork.StaffLicense.AddRange(eventStaff.StaffLicense);

                await _unitOfWork.StaffContractAffiliation.DeleteAgainstFieldAsync(eventStaff.Id, "EventStaffId");

                foreach (var affiliation in eventStaff.StaffContractAffiliation)
                {
                    affiliation.EventStaffId = eventStaff.Id;
                }

                _unitOfWork.StaffContractAffiliation.AddRange(eventStaff.StaffContractAffiliation);

                await _unitOfWork.TravelHonor.DeleteAgainstFieldAsync(eventStaff.Id, "EventStaffId");

                foreach (var travelHonor in eventStaff.TravelHonorList)
                {
                    travelHonor.EventStaffId = eventStaff.Id;
                }

                _unitOfWork.TravelHonor.AddRange(eventStaff.TravelHonorList);


                var result = await UpdateUser(eventStaff);
                //var result = await _roleService.UpdateUserEventStaffRolesAsync(eventStaff);

                if (!result.Success)
                {
                    await transaction.RollbackAsync();
                    return result;
                }

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

        public async Task<string> GetNextStaffId()
        {
            try
            {
                var allEventStaff = await _unitOfWork.EventStaff.GetAllAsync();

                if (allEventStaff == null || !allEventStaff.Any())
                {
                    return "0001"; // Default starting code
                }

                var lastEventStaff = allEventStaff
                    .OrderByDescending(c => c.Id) // Sort by Id or another property as necessary
                    .FirstOrDefault();

                var staffId = lastEventStaff.StaffID; // Extract the last StaffID
                var numericPart = int.Parse(staffId.Substring(3)); // Get the numeric part (e.g., "0001")

                numericPart++;

                return numericPart.ToString("D4"); // Return incremented value in "0001" format
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching the next StaffID.", ex);
            }
        }

        public async Task<IEnumerable<EventStaff>> GetEventStaffForSearchingByStaffId(string staffId)
        {
            try
            {
                if (string.IsNullOrEmpty(staffId))
                {
                    return await _unitOfWork.EventStaff.FindForSearchingAsync(c => true);
                }

                //return await _unitOfWork.EventStaff.FindForSearchingAsync(c => c.StaffID.Contains(staffId));
                return await _unitOfWork.EventStaff.FindForSearchingAsync(
            c => c.StaffID.ToLower().Contains(staffId.ToLower())
        );
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching contract details.", ex);
            }
        }

        public async Task<EventStaff> GetEventStaffByColumn(string userId)
        {
            try
            {
                var eventStaff = await _unitOfWork.EventStaff.FindAsync(es => es.UserId == userId);
                if (eventStaff == null)
                {
                    throw new KeyNotFoundException($"EventStaff with UserId {userId} not found.");
                }
                return eventStaff;
            }
            catch (KeyNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving event staff details.", ex);
            }
        }


        public async Task<bool> CheckSSNExistsAsync(string ssn)
        {
            try
            {
                var staff = await _unitOfWork.EventStaff.FindAsync(es => es.StaffSSN == ssn);
                return staff != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<ResponseDto> UpdateUser(EventStaff eventStaff)
        {
            var responseDto = new ResponseDto();

            try
            {
                UserUpdateDto userUpdateDto = new UserUpdateDto();

                var user = await _userManager.FindByIdAsync(eventStaff.UserId);

                if (user == null)
                {
                    responseDto.Success = true;
                    responseDto.Message = "User not found!";
                    return responseDto;
                }

                var existingRoleNames = await _userManager.GetRolesAsync(user);
                userUpdateDto.Id = eventStaff.UserId;
                userUpdateDto.Email = eventStaff.UserEmail;
                userUpdateDto.Password = eventStaff.UserPassword;
                userUpdateDto.SelectedRoles = eventStaff.StaffLicense.Select(l => l.RoleId).ToList();

                return await _registrationService.UpdateUserAsync(userUpdateDto);
            }
            catch (Exception)
            {
                return new ResponseDto { Success = false, Message = "An unexpected error occurred while updating user. Please try again later or contact your administrator." };
            }
        }
    }
}

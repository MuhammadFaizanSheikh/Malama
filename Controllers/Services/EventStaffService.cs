using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class EventStaffService : IEventStaffService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventStaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto> AddContractAsync(EventStaff eventStaff, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                // Attempt to add the contract detail to the repository

                foreach (var affiliation in eventStaff.StaffContractAffiliation)
                {
                    var subContractor = await _unitOfWork.SubContractors.FindAsync(c => c.ContractId == affiliation.ContractId);
                    affiliation.SubContractorId = subContractor.Id;
                }

                eventStaff.AddedBy = loggedinUserName;
                eventStaff.AddedOn = DateTime.Now;

                await _unitOfWork.EventStaff.AddAsync(eventStaff);

                responseDto.Success = true;
                responseDto.Message = "Event Staff added successfully!";
            }
            catch (Exception ex)
            {
                responseDto.Success = false;
                responseDto.Message = $"An error occurred: {ex.Message}";
            }

            return responseDto;
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


        //public async Task<EventStaffDto> GetEventStaffById(long id)
        //{
        //    EventStaffDto eventStaff = null;

        //    try
        //    {
        //        eventStaff = await _unitOfWork.EventStaff.GetByIdAsync(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //    return eventStaff;
        //}
        public async Task<CombinedEventStaffSubContractorAndContractDto> GetEventStaffById(long id)
        {
            try
            {
                var eventStaff = await _unitOfWork.EventStaff.GetWithIncludeAsync(
                    x => x.Id == id,
                    x => x.Licenses,
                    x => x.StaffContractAffiliation,
                    x => x.TravelHonorList
                );

                if (eventStaff != null)
                {
                    var firstEventStaff = eventStaff.FirstOrDefault();

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
                            ContractName = filteredContracts.ContractID // Assuming ContractID is the name for contract
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



        public async Task<ResponseDto> UpdateContract(EventStaff eventStaff, string loggedinUserName)
        {
            var responseDto = new ResponseDto();
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var affiliation in eventStaff.StaffContractAffiliation)
                {
                    var subContractor = await _unitOfWork.SubContractors.FindAsync(c => c.ContractId == affiliation.ContractId);
                    affiliation.SubContractorId = subContractor.Id;
                }

                var existingEvent = await _unitOfWork.EventStaff.GetByIdAsync(eventStaff.Id);
                eventStaff.AddedBy = existingEvent.AddedBy;
                eventStaff.AddedOn = existingEvent.AddedOn;
                eventStaff.UpdatedBy = loggedinUserName;
                eventStaff.UpdatedOn = DateTime.Now;
                await _unitOfWork.EventStaff.UpdateAsync(eventStaff);

                await _unitOfWork.StaffLicenses.DeleteAgainstFieldAsync(eventStaff.Id, "EventStaffId");

                foreach (var license in eventStaff.Licenses)
                {
                    license.EventStaffId = eventStaff.Id;
                }

                _unitOfWork.StaffLicenses.AddRange(eventStaff.Licenses);

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
        //public async Task<IEnumerable<ContractDetails>> GetContractForSearchingByContractId(string contractId)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(contractId))
        //        {
        //            return await repository.FindForSearchingAsync(c => true);
        //        }

        //        return await repository.FindForSearchingAsync(c => c.ContractID.Contains(contractId));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error while fetching contract details.", ex);
        //    }
        //}
    }
}

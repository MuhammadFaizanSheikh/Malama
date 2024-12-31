using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
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

        public async Task<ResponseDto> AddContractAsync(EventStaff evebtStaff, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                // Attempt to add the contract detail to the repository
                evebtStaff.AddedBy = loggedinUserName;
                evebtStaff.AddedOn = DateTime.Now;
                await _unitOfWork.EventStaff.AddAsync(evebtStaff);

                // If successful, set Success to true and provide a success message
                responseDto.Success = true;
                responseDto.Message = "Event Staff added successfully!";
            }
            catch (Exception ex)
            {
                // If an exception occurs, set Success to false and provide the error message
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
                    x => x.Licenses
                );

                if (eventStaff != null)
                {
                    var firstEventStaff = eventStaff.FirstOrDefault();
                    var subContractorId = firstEventStaff.SubContractorId;  // Access SubContractorId directly

                    var subContractor = await _unitOfWork.SubContractors.FindByColumnAsync<SubContractorInfoDto>("Id", subContractorId);
                    var firstSubContractor = subContractor.FirstOrDefault();

                    var combinedDto = new CombinedEventStaffSubContractorAndContractDto
                    {
                        SubContractor = firstSubContractor,
                        ContractDetails = null,
                        EventStaff = firstEventStaff
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
                var existingEvent = await _unitOfWork.EventStaff.GetByIdAsync(eventStaff.Id);
                eventStaff.AddedBy = existingEvent.AddedBy;
                eventStaff.AddedOn = existingEvent.AddedOn;
                eventStaff.UpdatedBy = loggedinUserName;
                eventStaff.UpdatedOn = DateTime.Now;
                await _unitOfWork.EventStaff.UpdateAsync(eventStaff);

                // Step 2: Remove old licenses
                await _unitOfWork.StaffLicenses.DeleteAgainstFieldAsync(eventStaff.Id, "EventStaffId");

                // Step 3: Assign EventStaffId to each new license
                foreach (var license in eventStaff.Licenses)
                {
                    license.EventStaffId = eventStaff.Id;
                }

                // Step 4: Add new licenses
                _unitOfWork.StaffLicenses.AddRange(eventStaff.Licenses);

                // Step 5: Save changes inside the transaction
                await _unitOfWork.SaveAsync();

                // Step 6: Commit the transaction
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

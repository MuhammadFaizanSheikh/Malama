using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;

namespace ExcelFilesCompiler.Controllers
{
    public class EventStaffController : Controller
    {
        private readonly IEventStaffService _eventStaffService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventStaffController(IEventStaffService eventStaffService, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _eventStaffService = eventStaffService;
        }

        public async Task<IActionResult> Index()
        {
            List<EventStaff> eventStaffList = new List<EventStaff>();

            try
            {
                eventStaffList = await _eventStaffService.GetAllEventStaff();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "We encountered an issue while loading subcontractors. Please try again later.";
                // Optionally log the exception
            }

            var eventStaff = new EventStaff
            {
                //StartDate = DateTime.Now,
                //StaffLastName = "Doe",
                //StaffFirstName = "John",
                //StaffMiddleInitial = "A",
                //StaffSSN = "123-45-6789",
                //EventOnCallStaff = "Yes",
                //EventOnCallStaffEvent = "EventX",
                //NPI = "1234567890",
                //DAE = "12345",
                //CredentialingProcessDate = DateTime.Now.AddDays(-30),
                //HistoricalCredentialingDate = DateTime.Now.AddDays(-60),
                //DAWSONInternalCredentialingCompleteDate = DateTime.Now.AddDays(-15),
                //OutstandingTrainings = "None",
                //BackgroundCheckConcerns = "None",
                //BLSCertDate = DateTime.Now.AddDays(-100),
                //BLSCertNumber = "BLS12345",
                //ACLSCertDate = DateTime.Now.AddDays(-90),
                //ACLSCertNumber = "ACLS67890",
                //StaffCAC = "Yes",
                //StaffDoDID = "1234567890",
                //StaffCellNumber = "555-123-4567",
                //StaffPhone2 = "555-987-6543",
                //StaffEmail = "john.doe@example.com",
                //PrimaryAddress1 = "123 Main St",
                //PrimaryAddress2 = "Apt 4B",
                //PrimaryCity = "Springfield",
                //PrimaryState = "IL",
                //PrimaryZip = "62701",
                //SecondaryAddress1 = "456 Elm St",
                //SecondaryAddress2 = "Suite 101",
                //SecondaryCity = "Chicago",
                //SecondaryState = "IL",
                //SecondaryZip = "60601",
                //StaffInfoEnteredBy = "AdminUser"
            };

            var viewModel = new EventStaffViewModel
            {
                EventStaff = eventStaffList,
                SingleEventStaff = null
            };

            // Pass contracts data to the view
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEventStaff()
        {
            try
            {
                var eventStaffs = await _eventStaffService.GetAllEventStaff();
                if (eventStaffs == null)
                {
                    return Json(new { success = false, message = "EVent Staff not found." });
                }

                return Json(new { success = true, eventStaffs = eventStaffs });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEventStaff(EventStaffViewModel eventStaffDto, string action)
        {
            try
            {
                ResponseDto res = new ResponseDto();

                if (eventStaffDto.SingleEventStaff.Id == 0)
                {
                    ModelState.Remove("SingleEventStaff.Id");
                }

                if (!ModelState.IsValid)
                {
                    // Log the validation errors for debugging
                    foreach (var error in ModelState.Values)
                    {
                        foreach (var err in error.Errors)
                        {
                            Console.WriteLine($"Error: {err.ErrorMessage}");
                        }
                    }

                    return View("Index", eventStaffDto);
                }

                var user = _userManager.GetUserAsync(User).Result;

                if (user != null)
                {
                    if (eventStaffDto.SingleEventStaff.StaffContractAffiliation != null)
                    {
                        // Create a temporary list to hold the transformed entities
                        var updatedAffiliations = new List<StaffContractAffiliation>();

                        foreach (var affiliation in eventStaffDto.SingleEventStaff.StaffContractAffiliation)
                        {
                            if (affiliation.StaffContractAffiliationTemp != null)
                            {
                                foreach (var contractId in affiliation.StaffContractAffiliationTemp)
                                {
                                    // Add each transformed entity to the temporary list
                                    updatedAffiliations.Add(new StaffContractAffiliation
                                    {
                                        EventStaffId = affiliation.EventStaffId,
                                        SubContractorId = affiliation.SubContractorId,
                                        ContractId = contractId,
                                        SubContractorName = affiliation.SubContractorName
                                    });
                                }
                            }
                        }

                        // Replace the original collection with the updated list
                        eventStaffDto.SingleEventStaff.StaffContractAffiliation = updatedAffiliations;
                    }
                    else
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Error";
                        TempData["ResponseMessage"] = "Contract affiliation not set";
                        return RedirectToAction("Index");
                    }



                    if (action == "Add")
                    {
                        res = await _eventStaffService.AddContractAsync(eventStaffDto.SingleEventStaff, user.UserName);
                    }
                    else if (action == "Update")
                    {
                        res = await _eventStaffService.UpdateContract(eventStaffDto.SingleEventStaff, user.UserName);
                    }
                }
                else
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    return RedirectToAction("Index");
                }

                TempData["ResponseStatus"] = res.Success ? "success" : "error"; // SweetAlert2 icon
                TempData["ResponseTitle"] = res.Success ? "Success" : "Error";
                TempData["ResponseMessage"] = res.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";
                return RedirectToAction("Index", eventStaffDto);
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> GetEventStaffById(long id)
        //{
        //    try
        //    {
        //        var eventStaff = await _eventStaffService.GetEventStaffById(id);
        //        if (eventStaff == null)
        //        {
        //            return Json(new { success = false, message = "Contract not found." });
        //        }

        //        return Json(new { success = true, eventStaff });
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { success = false, message = "An error occurred while retrieving the contract." });
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetEventStaffById(long id)
        {
            try
            {
                var combinedData = await _eventStaffService.GetEventStaffById(id);
                if (combinedData == null)
                {
                    return Json(new { success = false, message = "Contract not found." });
                }

                return Json(new { success = true, combinedData = combinedData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEventStaffWithoutIncludeById(long id)
        {
            try
            {
                var eventStaff = await _eventStaffService.GetEventStaffWithoutIncludeById(id);

                if (eventStaff == null)
                {
                    return Json(new { success = false, message = "EventStaff not found." });
                }

                return Json(new { success = true, eventStaff = eventStaff });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNextStaffId()
        {
            try
            {
                var staffId = await _eventStaffService.GetNextStaffId();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        staffId = staffId
                    }
                });
            }
            catch (Exception ex)
            {
                // Return an error response if something goes wrong
                return StatusCode(500, new { message = "An error occurred while fetching sequence of StaffID." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEventStaffForSearching(string staffId)
        {
            try
            {
                var eventStaff = await _eventStaffService.GetEventStaffForSearchingByStaffId(staffId);

                var result = eventStaff.Select(c => new
                {
                    id = c.Id,
                    text = c.StaffID
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching Event Staff." });
            }
        }
    }
}

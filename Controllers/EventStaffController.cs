using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Authorization;
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

        [RoleAttributeAuthorizeFromConfig("EventStaff_View")]

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
            }

            var viewModel = new EventStaffViewModel
            {
                EventStaff = eventStaffList,
                SingleEventStaff = null
            };

            // Pass contracts data to the view
            return View(viewModel);
        }

        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("EventStaff_View")]
        public async Task<IActionResult> GetAllEventStaff()
        {
            try
            {
                var eventStaffs = await _eventStaffService.GetAllEventStaffWithRolesAndLicenses();

                if (eventStaffs == null || !eventStaffs.Any())
                {
                    return Json(new { success = false, message = "No Event Staff found." });
                }

                return Json(new { success = true, eventStaffs });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An internal server error occurred. Please try again later." });
            }
        }


        [HttpPost]
        [RoleAttributeAuthorizeFromConfig("EventStaff_Save")]
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

                    if (eventStaffDto.SingleEventStaff.StaffQualification != null)
                    {
                        var attributeDetails = new List<StaffAttributeDetails>();

                        foreach (var role in eventStaffDto.SingleEventStaff.StaffQualification)
                        {
                            if (role.StaffAttributeTemp != null && role.StaffAttributeTemp.Count > 0)
                            {
                                role.StaffAttributeDetails = new List<StaffAttributeDetails>();

                                foreach (var attribute in role.StaffAttributeTemp)
                                {
                                    role.StaffAttributeDetails.Add(new StaffAttributeDetails
                                    {
                                        Attribute = attribute
                                    });


                                }
                            }
                        }
                    }

                    if (action == "Add")
                    {
                        res = await _eventStaffService.AddContractAsync(eventStaffDto.SingleEventStaff, eventStaffDto.SubmissionToken, user.UserName);
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
                    //return RedirectToAction("Index");
                    return RedirectToAction("Index");
                }

                TempData["ResponseStatus"] = res.Success ? "success" : "error"; // SweetAlert2 icon
                TempData["ResponseTitle"] = res.Success ? "Success" : "Error";
                TempData["ResponseMessage"] = res.Message;

                if (res.Success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";
                return RedirectToAction("Index");
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
        [RoleAttributeAuthorizeFromConfig("EventStaff_View")]
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
        //[RoleAttributeAuthorizeFromConfig("EventStaff_View")]
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
        //[RoleAttributeAuthorizeFromConfig("EventStaff_View")]
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
        //[RoleAttributeAuthorizeFromConfig("EventStaff_View")]
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

        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("EventStaff_View")]
        public async Task<IActionResult> CheckSSNExists(string ssn)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ssn))
                {
                    return BadRequest(new { exists = false, message = "SSN cannot be empty." });
                }

                bool exists = await _eventStaffService.CheckSSNExistsAsync(ssn);

                if (exists)
                {
                    return Ok(new { exists = true, message = "This SSN already exists." });
                }
                else
                {
                    return Ok(new { exists = false });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exists = false, message = "Server error. Please try again later." });
            }
        }

    }
}

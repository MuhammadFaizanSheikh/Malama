using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class EventStaffController : Controller
    {
        private readonly IEventStaffService _eventStaffService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EventStaffController> _logger;
        private const string CLASSNAME = "EventStaffController";

        public EventStaffController(ILogger<EventStaffController> logger, IEventStaffService eventStaffService, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _eventStaffService = eventStaffService;
            _logger = logger;
        }

        [RoleAttributeAuthorizeFromConfig("EventStaff_View")]
        public async Task<IActionResult> Index()
        {
            const string methodName = "Index";
            _logger.LogInformation("{ClassName}, {MethodName}, Loading EventStaff list.", CLASSNAME, methodName);

            List<EventStaff> eventStaffList = new List<EventStaff>();

            try
            {
                eventStaffList = await _eventStaffService.GetAllEventStaff();
                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {Count} EventStaff records.", CLASSNAME, methodName, eventStaffList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to load EventStaff list.", CLASSNAME, methodName);
                TempData["ErrorMessage"] = "We encountered an issue while loading subcontractors. Please try again later.";
            }

            var viewModel = new EventStaffViewModel
            {
                EventStaff = eventStaffList,
                SingleEventStaff = null
            };

            return View(viewModel);
        }


        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("EventStaff_View")]
        public async Task<IActionResult> GetAllEventStaff()
        {
            const string methodName = "GetAllEventStaff";
            _logger.LogInformation("{ClassName}, {MethodName}, Fetching all EventStaff with roles and licenses.", CLASSNAME, methodName);

            try
            {
                var eventStaffs = await _eventStaffService.GetAllEventStaffWithRolesAndLicenses();

                if (eventStaffs == null || !eventStaffs.Any())
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, No EventStaff found.", CLASSNAME, methodName);
                    return Json(new { success = false, message = "No Event Staff found." });
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {Count} EventStaff records.", CLASSNAME, methodName, eventStaffs.Count);
                return Json(new { success = true, eventStaffs });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "{ClassName}, {MethodName}, Key not found while fetching EventStaff.", CLASSNAME, methodName);
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected error while fetching EventStaff.", CLASSNAME, methodName);
                return Json(new { success = false, message = "An internal server error occurred. Please try again later." });
            }
        }


        [HttpPost]
        [RoleAttributeAuthorizeFromConfig("EventStaff_Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEventStaff(EventStaffViewModel eventStaffDto, string action)
        {
            const string methodName = "CreateEventStaff";
            _logger.LogInformation("{ClassName}, {MethodName}, Called for action: {Action}", CLASSNAME, methodName, action);

            try
            {
                ResponseDto res = new ResponseDto();

                if (eventStaffDto.SingleEventStaff.Id == 0)
                {
                    ModelState.Remove("SingleEventStaff.Id");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, ModelState invalid.", CLASSNAME, methodName);
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogWarning("{ClassName}, {MethodName}, Model error: {Error}", CLASSNAME, methodName, error.ErrorMessage);
                    }

                    return View("Index", eventStaffDto);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not logged in.", CLASSNAME, methodName);
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    return RedirectToAction("Index");
                }

                // Transform contract affiliations
                if (eventStaffDto.SingleEventStaff.StaffContractAffiliation != null)
                {
                    var updatedAffiliations = new List<StaffContractAffiliation>();

                    foreach (var affiliation in eventStaffDto.SingleEventStaff.StaffContractAffiliation)
                    {
                        if (affiliation.StaffContractAffiliationTemp != null)
                        {
                            foreach (var contractId in affiliation.StaffContractAffiliationTemp)
                            {
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

                    eventStaffDto.SingleEventStaff.StaffContractAffiliation = updatedAffiliations;
                }
                else
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, StaffContractAffiliation not set.", CLASSNAME, methodName);
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Contract affiliation not set";
                    return RedirectToAction("Index");
                }

                // Transform staff qualifications
                if (eventStaffDto.SingleEventStaff.StaffQualification != null)
                {
                    foreach (var role in eventStaffDto.SingleEventStaff.StaffQualification)
                    {
                        if (role.StaffAttributeTemp != null && role.StaffAttributeTemp.Count > 0)
                        {
                            role.StaffAttributeDetails = role.StaffAttributeTemp
                                .Select(a => new StaffAttributeDetails { Attribute = a })
                                .ToList();
                        }
                    }
                }

                // Call service
                if (action == "Add")
                {
                    res = await _eventStaffService.AddContractAsync(eventStaffDto.SingleEventStaff, eventStaffDto.SubmissionToken, user.UserName);
                }
                else if (action == "Update")
                {
                    res = await _eventStaffService.UpdateContract(eventStaffDto.SingleEventStaff, user.UserName);
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Service result: Success={Success}, Message={Message}",
                    CLASSNAME, methodName, res.Success, res.Message);

                TempData["ResponseStatus"] = res.Success ? "success" : "error";
                TempData["ResponseTitle"] = res.Success ? "Success" : "Error";
                TempData["ResponseMessage"] = res.Message;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected exception while creating/updating EventStaff.", CLASSNAME, methodName);
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
            const string methodName = "GetEventStaffById";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with ID: {Id}", CLASSNAME, methodName, id);

            try
            {
                var combinedData = await _eventStaffService.GetEventStaffById(id);
                if (combinedData == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Contract not found. ID: {Id}", CLASSNAME, methodName, id);
                    return Json(new { success = false, message = "Contract not found." });
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Contract retrieved successfully. ID: {Id}", CLASSNAME, methodName, id);
                return Json(new { success = true, combinedData = combinedData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while fetching contract. ID: {Id}", CLASSNAME, methodName, id);
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetEventStaffWithoutIncludeById(long id)
        {
            const string methodName = "GetEventStaffWithoutIncludeById";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with ID: {Id}", CLASSNAME, methodName, id);

            try
            {
                var eventStaff = await _eventStaffService.GetEventStaffWithoutIncludeById(id);

                if (eventStaff == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, EventStaff not found. ID: {Id}", CLASSNAME, methodName, id);
                    return Json(new { success = false, message = "EventStaff not found." });
                }

                _logger.LogInformation("{ClassName}, {MethodName}, EventStaff retrieved successfully. ID: {Id}", CLASSNAME, methodName, id);
                return Json(new { success = true, eventStaff = eventStaff });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while fetching EventStaff. ID: {Id}", CLASSNAME, methodName, id);
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetNextStaffId()
        {
            const string methodName = "GetNextStaffId";
            _logger.LogInformation("{ClassName}, {MethodName}, Fetching next StaffID.", CLASSNAME, methodName);

            try
            {
                var staffId = await _eventStaffService.GetNextStaffId();
                _logger.LogInformation("{ClassName}, {MethodName}, Next StaffID fetched: {StaffId}", CLASSNAME, methodName, staffId);

                return Ok(new
                {
                    success = true,
                    data = new { staffId = staffId }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while fetching next StaffID.", CLASSNAME, methodName);
                return StatusCode(500, new { message = "An error occurred while fetching sequence of StaffID." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetEventStaffForSearching(string staffId)
        {
            const string methodName = "GetEventStaffForSearching";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with staffId: {StaffId}", CLASSNAME, methodName, staffId);

            try
            {
                var eventStaff = await _eventStaffService.GetEventStaffForSearchingByStaffId(staffId);

                var result = eventStaff.Select(c => new
                {
                    id = c.Id,
                    text = c.StaffID
                });

                _logger.LogInformation("{ClassName}, {MethodName}, {Count} records found for staffId: {StaffId}", CLASSNAME, methodName, result.Count(), staffId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while searching EventStaff. staffId: {StaffId}", CLASSNAME, methodName, staffId);
                return StatusCode(500, new { message = "An error occurred while fetching Event Staff." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> CheckSSNExists(string ssn)
        {
            const string methodName = "CheckSSNExists";
            _logger.LogInformation("{ClassName}, {MethodName}, Checking SSN: {SSN}", CLASSNAME, methodName, ssn);

            try
            {
                if (string.IsNullOrWhiteSpace(ssn))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, SSN is empty.", CLASSNAME, methodName);
                    return BadRequest(new { exists = false, message = "SSN cannot be empty." });
                }

                bool exists = await _eventStaffService.CheckSSNExistsAsync(ssn);

                if (exists)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, SSN already exists: {SSN}", CLASSNAME, methodName, ssn);
                    return Ok(new { exists = true, message = "This SSN already exists." });
                }
                else
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, SSN does not exist: {SSN}", CLASSNAME, methodName, ssn);
                    return Ok(new { exists = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while checking SSN: {SSN}", CLASSNAME, methodName, ssn);
                return StatusCode(500, new { exists = false, message = "Server error. Please try again later." });
            }
        }

    }
}

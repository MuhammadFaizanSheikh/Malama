using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;

namespace ExcelFilesCompiler.Controllers
{
    public class EventManagementController : Controller
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventManagementController(IEventManagementService eventManagementService, UserManager<ApplicationUser> userManager)
        {
            _eventManagementService = eventManagementService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var responseDto = new ResponseDto();
            List<EventManagement> eventManagementList = new List<EventManagement>();

            try
            {
                eventManagementList = await _eventManagementService.GetAllEventManagements();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "We encountered an issue while loading event managements. Please try again later.";
            }

            var viewModel = new EventManagementViewModel
            {
                EventManagements = eventManagementList,
                SingleEventManagement = null
            };
            // Pass contracts data to the view
            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEventManagement(EventManagementViewModel eventManagement, string action, string completedSections)
        {
            try
            {
                ResponseDto res = new ResponseDto();

                if (eventManagement.SingleEventManagement.Id == 0)
                {
                    ModelState.Remove("SingleEventManagement.Id");
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

                    return View("Index", eventManagement);
                }

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    if (action == "Add")
                    {
                        res = await _eventManagementService.AddEventManagementAsync(eventManagement.SingleEventManagement, user.UserName);
                    }
                    else if (action == "Update")
                    {
                        //res = await _eventManagementService.UpdateContract(eventManagement.SingleEventManagement, user.UserName);
                    }
                }
                else
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    TempData["ContractDto"] = eventManagement;
                    //return RedirectToAction("Index", contractDto);
                    return RedirectToAction("Index");
                }

                TempData["ResponseStatus"] = res.Success ? "success" : "error"; // SweetAlert2 icon
                TempData["ResponseTitle"] = res.Success ? "Success" : "Error";
                TempData["ResponseMessage"] = res.Message;
                return RedirectToAction("Index");
                //return RedirectToAction("Index", contractDto);
            }
            catch (Exception ex)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";
                TempData["ContractDto"] = eventManagement;
                return RedirectToAction("Index", eventManagement);
            }
        }
    }
}

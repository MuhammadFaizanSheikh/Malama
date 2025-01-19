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
            //var responseDto = new ResponseDto();
            //List<ContractDetails> contractsList = new List<ContractDetails>();

            //try
            //{
            //    contractsList = await _eventManagementService.GetAllContracts();
            //}
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = "We encountered an issue while loading contracts. Please try again later.";
            //}

            //var viewModel = new ContractViewModel
            //{
            //    Contracts = contractsList,
            //    SingleContract = null
            //};
            //// Pass contracts data to the view
            return View();

        }
    }
}

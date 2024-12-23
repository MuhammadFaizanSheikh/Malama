using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace ExcelFilesCompiler.Controllers
{
    public class ContractDetailsController : Controller
    {
        private readonly IContractService _contractService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContractDetailsController(IContractService contractService, UserManager<ApplicationUser> userManager)
        {
            _contractService = contractService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var responseDto = new ResponseDto();
            List<ContractDetails> contractsList = new List<ContractDetails>();

            try
            {
                contractsList = await _contractService.GetAllContracts();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "We encountered an issue while loading contracts. Please try again later.";
            }

            var viewModel = new ContractViewModel
            {
                Contracts = contractsList,
                SingleContract = null
            };
            // Pass contracts data to the view
            return View(viewModel);



            //var dummyData = new ContractDetails
            //{
            //    ContractID = "CONTRACT12345",
            //    ContractAgency = "Agency Name",
            //    ContractServiceBranch = "Branch Name",
            //    ContractComponent = "Component Name",
            //    ContractClient = "Client Name",
            //    ContractType = "Type Name",
            //    DawsonRoleOnContract = "Role Name",
            //    ContractStatus = "Active",
            //    ContractStartDate = DateTime.Now,
            //    ContractEndDate = DateTime.Now.AddYears(1),
            //    KoLastName = "Smith",
            //    KoFirstName = "John",
            //    KOPhone = "1234567890",
            //    KOPhone2 = "0987654321",
            //    KOEmail = "ko@example.com",
            //    KONotes = "Sample notes for KO",
            //    CORLastName = "Doe",
            //    CORPrefix = "Mr",
            //    CORFirstName = "Jane",
            //    CORKORank = "Rank 1",
            //    CORPhone = "2345678901",
            //    CORPhone2 = "5678901234",
            //    COREmail = "cor@example.com",
            //    CORNotes = "Sample notes for COR",
            //    DawsonProgramManagerLastName = "Adams",
            //    DawsonProgramManagerFirstName = "Tom",
            //    DawsonDeputyProgramManagerLastName = "Lee",
            //    DawsonDeputyProgramManagerFirstName = "Henry",
            //    DawsonProjectManagerLastName = "Jackson",
            //    DawsonProjectManagerFirstName = "Emily"
            //};

            //// Pass the dummy data to the view
            //return View(dummyData);

        }



        [HttpPost]
        public async Task<IActionResult> CreateContractDetails(ContractViewModel contractDto, string action)
        {
            try
            {
                ResponseDto res = new ResponseDto();

                if (contractDto.SingleContract.Id == 0) // Adding a new record
                {
                    ModelState.Remove("SingleContract.Id");
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

                    return View("Index", contractDto);
                }

                var user = _userManager.GetUserAsync(User).Result;

                if (user != null)
                {
                    if (action == "Add")
                    {
                        res = await _contractService.AddContractAsync(contractDto.SingleContract, user.UserName);
                    }
                    else if (action == "Update")
                    {
                        res = await _contractService.UpdateContract(contractDto.SingleContract, user.UserName);
                    }
                }
                else
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] ="Error";
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
                return RedirectToAction("Index", contractDto);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetContractById(long id)
        {
            try
            {
                var contractDetails = await _contractService.GetContractById(id);
                if (contractDetails == null)
                {
                    return Json(new { success = false, message = "Contract not found." });
                }

                return Json(new { success = true, contractDetails });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while retrieving the contract." });
            }
        }
    }
}

using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace ExcelFilesCompiler.Controllers
{
    public class ContractDetailsController : Controller
    {
        private readonly IContractService _contractService;

        public ContractDetailsController(IContractService contractService)
        {
            _contractService = contractService;
        }

        public async Task<IActionResult> Index()
        {
            var responseDto = new ResponseDto();
            List<ContractDetails> contractsList = new List<ContractDetails>();

            try
            {
                // Correct the tuple destructuring to match the return values
                var (res, contracts) = await _contractService.GetAllContracts();

                // You can also use `res` to check for success message or error
                responseDto.Success = res.Success;
                responseDto.Message = res.Message;
                contractsList = contracts;
            }
            catch (Exception ex)
            {
                responseDto.Success = false;
                responseDto.Message = $"An error occurred while fetching contracts: {ex.Message}";
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
        public async Task<IActionResult> CreateContractDetails(ContractViewModel contractDto)
        {
            try
            {
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

                var res = await _contractService.AddContractAsync(contractDto.SingleContract);

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
                var (res, contractDetails) = await _contractService.GetContractById(id);
                if (!res.Success)
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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ContractDetails contractDto)
        {
            try
            {
                var res = await _contractService.UpdateContract(contractDto);
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
    }
}

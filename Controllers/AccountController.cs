using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Hardcoded credentials
            string hardcodedUsername = "DAWSON";
            string hardcodedPassword = "123456";

            if (model.Username != null && model.Password != null)
            {
                if (model.Username.ToUpper() == hardcodedUsername && model.Password == hardcodedPassword)
                {
                    // Redirect to Home page if login is successful
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    // Return an error message if login fails
                    ViewBag.ErrorMessage = "Invalid username or password";
                    return View();
                }
            }
            else
            {
                // Return an error message if login fails
                ViewBag.ErrorMessage = "Invalid username or password";
                return View();
            }
        }
    }
}
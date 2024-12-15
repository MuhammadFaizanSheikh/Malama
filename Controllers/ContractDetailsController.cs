using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class ContractDetailsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

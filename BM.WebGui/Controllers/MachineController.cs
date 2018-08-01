using Microsoft.AspNetCore.Mvc;

namespace BM.WebGui
{
    public class MachineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
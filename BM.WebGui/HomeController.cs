using Microsoft.AspNetCore.Mvc;

namespace BM.WebGui
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
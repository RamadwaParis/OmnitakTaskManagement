using Microsoft.AspNetCore.Mvc;

namespace OmintakProduction.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

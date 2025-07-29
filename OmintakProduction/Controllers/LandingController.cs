using Microsoft.AspNetCore.Mvc;

namespace OmintakProduction.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

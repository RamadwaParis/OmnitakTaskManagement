using Microsoft.AspNetCore.Mvc;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Diagnostics;

namespace OmintakProduction.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;

        }

        private readonly ILogger<HomeController> _logger;

        public IActionResult Index()
        {
            return View();
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

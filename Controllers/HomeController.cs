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

        public IActionResult Index()
        {
            var teams = _context.Team.ToList();
            var projects = _context.Project.ToList();
            var tasks = _context.Tasks.ToList();
            var users = _context.User.ToList();
            var model = new HomePageViewModel
            {
                Teams = teams,
                Projects = projects,
                Tasks = tasks,
                Users = users
            };
            return View(model);
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

    public class HomePageViewModel
    {
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Models.Task> Tasks { get; set; } = new List<Models.Task>();
        public List<User> Users { get; set; } = new List<User>();
    }
}

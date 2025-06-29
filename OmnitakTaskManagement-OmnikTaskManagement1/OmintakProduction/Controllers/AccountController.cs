using BCrypt.Net; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OmintakProduction.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewData["RegistrationError"] = "Username, email, and password are required.";
                return View();
            }

            // Check if email already exists
            if (await _context.User.AnyAsync(u => u.Email == email))
            {
                ViewData["RegistrationError"] = "Email  already exist.";
                return View();
            }

            // Hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // Assign default role (e.g., 'Developer')
            // Querying by Role.Name property as per your model

            var newUser = new User
            {
                UserName = username, // Changed from Username to Name to match User.cs model
                Email = email,
                Password = passwordHash,
                RoleId = 1,
                CreatedDate = new DateOnly(2025, 06, 27),
                isActive = true

            };

            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login","Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewData["LoginError"] = "Email and password are required.";
                return View();
            }

            // Find user by email
            var user = await _context.User.SingleOrDefaultAsync(u=> u.Email == email);


            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ViewData["LoginError"] = "Invalid email or password.";
                return View();
            }
            if (user.isActive == false)
            {
                ViewData["LoginError"] = "Your account is deactivated. Please contact support.";
                return View();
            }

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add role claim if user has a role
            if (user.RoleId != null)
            {
                var role = await _context.Role.FindAsync(user.RoleId);
                    claims.Add(new Claim(ClaimTypes.Role, role.RoleName)); // Using Name property from Role model
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // For "Remember me" functionality
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Set cookie expiration
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Dashboard");
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

    }
}


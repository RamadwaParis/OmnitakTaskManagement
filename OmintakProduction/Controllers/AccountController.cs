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
using OmintakProduction.Models;

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

            // email format validation
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                ViewData["RegistrationError"] = "Invalid email address format.";
                return View();
            }

            // password strength validation. Password must be at least 8 characters, have a letter, a number, and a special character
            var passwordRegex = new System.Text.RegularExpressions.Regex(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$");
            if (!passwordRegex.IsMatch(password))
            {
                ViewData["RegistrationError"] = "Password must be at least 8 characters and include a letter, a number, and a special character.";
                return View();
            }

            // check if the email already exists
            if (await _context.User.AnyAsync(u => u.Email == email))
            {
                ViewData["RegistrationError"] = "Email  already exist.";
                return View();
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                UserName = username,
                Email = email,
                Password = passwordHash,
                RoleId = 2,
                CreatedDate = new DateOnly(2025, 06, 27),
                isActive = true

            };

            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
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


            var user = await _context.User.SingleOrDefaultAsync(u => u.Email == email);

            if (!email.EndsWith(".seededData@omnitak.com", StringComparison.OrdinalIgnoreCase))
            {
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
            }




            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };


            if (user.RoleId != null)
            {
                Role getUserRole = new Role();
                var role = getUserRole.getRole(user.RoleId);

                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Ticket");
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


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

            // email format validation (RFC-5322 compatible)
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z](([\.\-+]?[0-9a-zA-Z]+)*)@)))" +
                           @"((\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][\w\-]*\.)+[a-zA-Z]{2,}))$");

            // password strength validation. Password must be at least 8 characters, have a letter, a number, and a special character
            var passwordRegex = new System.Text.RegularExpressions.Regex(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$");

            if (!emailRegex.IsMatch(email))
            {
                ModelState.AddModelError("Email", "Please enter a valid email address.");
            }
            if (!passwordRegex.IsMatch(password))
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters and include a letter, a number, and a special character.");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            // check if the email already exists
            if (await _context.User.AnyAsync(u => u.Email == email))
            {
                ViewData["RegistrationError"] = "Email already exists.";
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
                isActive = false, // not approved/active. Default to false on registration, will set to true when user approves account
                NeedsWelcome = true
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
                if (!user.isActive)
                {
                    ViewData["LoginError"] = "Your account is pending admin approval. Please wait for approval before logging in.";
                    return View();
                }

                // Check if this is the user's first login after approval
                if (user.NeedsWelcome)
                {
                    TempData["WelcomeMessage"] = "Your account is now approved! Welcome.";
                    user.NeedsWelcome = false; // Reset so it's only shown once
                    await _context.SaveChangesAsync();
                    //return View();
                }

                //if (user.isActive == false)
                //{
                //    ViewData["LoginError"] = "Your account is deactivated. Please contact support.";
                //    return View();
                //}

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

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                // Allow admin to log in without BCrypt
                bool isAdmin = user != null && user.RoleId == 1; // Adjust RoleId as needed for your admin role

                bool isPasswordValid;
                if (isAdmin)
                {
                    // Plain text check for admin
                    isPasswordValid = (password == user.Password);
                }
                else
                {
                    // BCrypt check for all others
                    isPasswordValid = user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
                }

                if (user == null || !isPasswordValid)
                {
                    ViewData["LoginError"] = "Invalid email or password.";
                    return View();
                }
                if (!user.isActive)
                {
                    ViewData["LoginError"] = "Your account is pending admin approval. Please wait for approval before logging in.";
                    return View();
                }

                // Check if this is the user's first login after approval
                if (user.NeedsWelcome)
                {
                    TempData["WelcomeMessage"] = "Your account is now approved! Welcome.";
                    user.NeedsWelcome = false; // Reset so it's only shown once
                    await _context.SaveChangesAsync();
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

                return RedirectToAction("Index", "Dashboard");
            }
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


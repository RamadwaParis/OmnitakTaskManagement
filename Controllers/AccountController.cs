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

            if (password.StartsWith("#") || password.StartsWith("@"))
            {
                ModelState.AddModelError("Password", "Password cannot start with # or @.");
            }
            if (password.Length < 8)
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters long.");
            }

            // email format validation (simple version)
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

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
            if (password.StartsWith("#") || password.StartsWith("@"))
            {
                ViewData["LoginError"] = "Password cannot start with # or @.";
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
                    // Create a notification for the user
                    var roleTitle = new Role().getRole(user.RoleId);
                    var welcomeMsg = $"Welcome to Omnitak! Your role is: {roleTitle}.";
                    var notification = new Notification
                    {
                        UserId = user.UserId,
                        Title = "Welcome to Omnitak!",
                        Message = welcomeMsg,
                        Type = NotificationType.Success,
                        CreatedAt = DateTime.Now
                    };
                    _context.Notification.Add(notification);
                    user.NeedsWelcome = false; // Only show once
                    await _context.SaveChangesAsync();
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            };


                if (user.RoleId > 0)
                {
                    Role getUserRole = new Role();
                    var role = getUserRole.getRole(user.RoleId);

                    claims.Add(new Claim(ClaimTypes.Role, role ?? string.Empty));
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
                    isPasswordValid = !string.IsNullOrEmpty(user?.Password) && (password == user.Password);
                }
                else
                {
                    // BCrypt check for all others
                    isPasswordValid = user != null && BCrypt.Net.BCrypt.Verify(password, user.Password ?? "");
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
                // if (user.NeedsWelcome)
                // {
                //     // Store user info in session for welcome screen
                //     HttpContext.Session.SetString("WelcomeUserName", $"{user.FirstName} {user.LastName}");
                //     HttpContext.Session.SetString("WelcomeUserRole", GetUserRoleTitle(user.RoleId));
                //     HttpContext.Session.SetString("WelcomeUserEmail", user.Email ?? "");
                //     user.NeedsWelcome = false; // Reset so it's only shown once
                //     await _context.SaveChangesAsync();
                // }

                var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
    };

                if (user.RoleId > 0)
                {
                    Role getUserRole = new Role();
                    var role = getUserRole.getRole(user.RoleId);
                    claims.Add(new Claim(ClaimTypes.Role, role ?? string.Empty));
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

        // Welcome screen action
        [HttpGet]
        public IActionResult Welcome()
        {
            var userName = HttpContext.Session.GetString("WelcomeUserName");
            var userRole = HttpContext.Session.GetString("WelcomeUserRole");
            var userEmail = HttpContext.Session.GetString("WelcomeUserEmail");

            ViewData["UserName"] = userName;
            ViewData["UserRole"] = userRole;
            ViewData["UserEmail"] = userEmail;

            // Clear session data after use
            HttpContext.Session.Remove("WelcomeUserName");
            HttpContext.Session.Remove("WelcomeUserRole");
            HttpContext.Session.Remove("WelcomeUserEmail");

            return View();
        }

        // Helper method to get user role title
        private string GetUserRoleTitle(int roleId)
        {
            var role = _context.Role.FirstOrDefault(r => r.RoleId == roleId);
            return role?.RoleName switch
            {
                "SystemAdmin" => "System Administrator",
                "ProjectLead" => "Project Lead",
                "Developer" => "Developer",
                "Tester" => "Tester",
                "DepartmentHead" => "Department Head",
                "OperationsManager" => "Operations Manager",
                "BusinessAnalyst" => "Business Analyst",
                "ProductOwner" => "Product Owner",
                "ResourceManager" => "Resource Manager",
                "DepartmentLead" => "Department Lead",
                "ProcessImprovementSpecialist" => "Process Improvement Specialist",
                _ => "Team Member"
            };
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveUsers()
        {
            var pendingUsers = _context.User.Where(u => !u.isActive).ToList();
            ViewBag.Teams = _context.Team.ToList();
            return View(pendingUsers);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveUser(int userId, string newUsername, int teamId, string firstName, string lastName)
        {
            var user = _context.User.Find(userId);
            var team = _context.Team.Find(teamId);
            if (user != null && !string.IsNullOrWhiteSpace(newUsername) && team != null && !string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                user.UserName = newUsername;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.isActive = true;
                user.TeamId = teamId;
                user.Team = team;
                _context.User.Update(user);
                _context.SaveChanges();
                TempData["Success"] = $"User {firstName} {lastName} approved, assigned to team {team.TeamName}, and username updated.";

                // Create notification for user
                var notification = new Notification {
                    UserId = user.UserId,
                    Title = "Welcome to Omnitak!",
                    Message = $"You have been approved and assigned to team <b>{team.TeamName}</b>.",
                    Type = NotificationType.Success,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    RelatedEntityId = team.TeamId,
                    RelatedEntityType = "Team"
                };
                _context.Notification.Add(notification);
                _context.SaveChanges();
            }
            return RedirectToAction("ApproveUsers");
        }
    }
}


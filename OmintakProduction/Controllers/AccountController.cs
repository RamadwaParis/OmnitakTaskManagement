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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


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
                RoleId = 2, // Default role - will be changed by admin during approval
                CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                isActive = false, // Must be approved by admin first
                IsApproved = false, // Requires admin approval
                NeedsWelcome = true,
                IsDeleted = false
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

            // JWT claims
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

            var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var jwtKey = config["Jwt:Key"] ?? "YourSuperSecretKeyHere";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtIssuer = config["Jwt:Issuer"] ?? "OmnitakIssuer";
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Sign in the user with cookies for web authentication
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Store user info in session for welcome message
            HttpContext.Session.SetString("WelcomeUserName", user.FirstName + " " + user.LastName);
            if (user.RoleId > 0)
            {
                Role getUserRole = new Role();
                var role = getUserRole.getRole(user.RoleId);
                HttpContext.Session.SetString("WelcomeUserRole", role ?? "User");
            }

            // Return JWT token as JSON for API clients
            if (Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                return Json(new { token = jwtToken });
            }

            // For web clients, redirect to dashboard
            return RedirectToAction("Index", "Dashboard");
            // (Legacy admin login logic removed; all logins now use JWT)
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
                // "ProjectLead" removed
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
        [Authorize(Roles = "SystemAdmin")]
        public IActionResult ApproveUsers()
        {
            var pendingUsers = _context.User.Where(u => !u.IsApproved && !u.IsDeleted).ToList();
            ViewBag.Teams = _context.Team.ToList();
            ViewBag.Roles = _context.Role.ToList();
            return View(pendingUsers);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public IActionResult ApproveUser(int userId, string newUsername, int roleId, int? teamId, int? projectId, string firstName, string lastName)
        {
            var user = _context.User.Find(userId);
            var team = teamId.HasValue ? _context.Team.Find(teamId.Value) : null;
            var role = _context.Role.Find(roleId);
            
            if (user != null && !string.IsNullOrWhiteSpace(newUsername) && role != null && !string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                user.UserName = newUsername;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.RoleId = roleId;
                user.isActive = true;
                user.IsApproved = true;
                user.TeamId = teamId;
                user.ProjectId = projectId;
                
                if (team != null)
                {
                    user.Team = team;
                }

                _context.User.Update(user);
                _context.SaveChanges();

                string teamMessage = team != null ? $" and assigned to team {team.TeamName}" : "";
                TempData["Success"] = $"User {firstName} {lastName} approved with role {role.RoleName}{teamMessage}.";

                // Create notification for user
                var notification = new Notification {
                    UserId = user.UserId,
                    Title = "Welcome to Omnitak!",
                    Message = $"You have been approved with role <b>{role.RoleName}</b>{(team != null ? $" and assigned to team <b>{team.TeamName}</b>" : "")}.",
                    Type = NotificationType.Success,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    RelatedEntityId = roleId,
                    RelatedEntityType = "Role"
                };
                _context.Notification.Add(notification);
                _context.SaveChanges();
            }
            return RedirectToAction("ApproveUsers");
        }
    }
}


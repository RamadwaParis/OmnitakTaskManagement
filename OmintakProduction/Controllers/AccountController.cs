using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmintakProduction.Data;
using OmintakProduction.Models;
using OmintakProduction.Services;
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
        private readonly IUserApprovalService _userApprovalService;

        public AccountController(AppDbContext context, IUserApprovalService userApprovalService)
        {
            _context = context;
            _userApprovalService = userApprovalService;
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

            // Notify admins of new registration
            await _userApprovalService.NotifyAdminsOfNewRegistration(newUser);

            // Set success message for approval pending
            TempData["RegistrationSuccess"] = "Registration successful! Your account is pending admin approval. You will be notified once approved.";
            ViewData["ShowApprovalMessage"] = true;

            return View("Login");
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
            // Add flag for SystemAdmin to show approval modal
            if (user.RoleId > 0)
            {
                Role getUserRole = new Role();
                var role = getUserRole.getRole(user.RoleId);
                if (role == "SystemAdmin")
                {
                    return RedirectToAction("Index", "Dashboard", new { fromLogin = "true" });
                }
            }
            
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
        public async Task<IActionResult> ApproveUsers()
        {
            var pendingUsers = await _userApprovalService.GetPendingApprovalUsers();
            ViewBag.Teams = _context.Team.ToList();
            ViewBag.Roles = _context.Role.ToList();
            return View(pendingUsers);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> ApproveUser(int userId, string newUsername, int roleId, int? teamId, int? projectId, string firstName, string lastName)
        {
            var success = await _userApprovalService.ApproveUser(userId, newUsername, roleId, teamId, projectId, firstName, lastName);
            
            if (success)
            {
                var role = await _context.Role.FindAsync(roleId);
                var team = teamId.HasValue ? await _context.Team.FindAsync(teamId.Value) : null;
                string teamMessage = team != null ? $" and assigned to team {team.TeamName}" : "";
                TempData["Success"] = $"User {firstName} {lastName} approved with role {role?.RoleName}{teamMessage}.";
            }
            else
            {
                TempData["Error"] = "Failed to approve user. Please try again.";
            }
            
            return RedirectToAction("ApproveUsers");
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> RejectUser(int userId, string reason = "")
        {
            var success = await _userApprovalService.RejectUser(userId, reason);
            
            if (success)
            {
                TempData["Success"] = "User registration has been rejected.";
            }
            else
            {
                TempData["Error"] = "Failed to reject user. Please try again.";
            }
            
            return RedirectToAction("ApproveUsers");
        }

        // API endpoints for modal functionality
        [HttpGet]
        [Route("api/checkPendingApprovals")]
        public async Task<IActionResult> CheckPendingApprovals()
        {
            if (!User.IsInRole("SystemAdmin"))
            {
                return Json(new { hasPendingApprovals = false });
            }

            var pendingCount = await _context.User
                .CountAsync(u => !u.IsApproved && !u.IsDeleted);

            return Json(new { hasPendingApprovals = pendingCount > 0, count = pendingCount });
        }

        [HttpPost]
        [Route("Account/ApproveUserQuick")]
        public async Task<IActionResult> ApproveUserQuick([FromBody] ApprovalRequest request)
        {
            if (!User.IsInRole("SystemAdmin"))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var user = await _context.User.FindAsync(request.UserId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Simple approval - just mark as approved
                user.IsApproved = true;
                user.isActive = true;
                
                // Assign default role if RoleId is 0 (assuming 0 means unassigned)
                if (user.RoleId == 0)
                {
                    user.RoleId = 2; // Developer role
                }
                
                await _context.SaveChangesAsync();
                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Account/RejectUserQuick")]
        public async Task<IActionResult> RejectUserQuick([FromBody] ApprovalRequest request)
        {
            if (!User.IsInRole("SystemAdmin"))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var success = await _userApprovalService.RejectUser(request.UserId, "Rejected by SystemAdmin");
                
                if (success)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to reject user" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class ApprovalRequest
    {
        public int UserId { get; set; }
    }
}

